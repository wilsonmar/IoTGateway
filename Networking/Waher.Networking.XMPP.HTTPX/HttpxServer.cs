﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using Waher.Content;
using Waher.Events;
using Waher.Networking.HTTP;
using Waher.Runtime.Cache;
using Waher.Security;

namespace Waher.Networking.XMPP.HTTPX
{
	/// <summary>
	/// HTTPX server.
	/// </summary>
	public class HttpxServer : IDisposable
	{
		private XmppClient client;
		private HttpServer server;
		private int maxChunkSize;
		private bool requiresE2e = false;

		/// <summary>
		/// HTTPX server.
		/// </summary>
		/// <param name="Client">XMPP Client.</param>
		/// <param name="Server">HTTP Server.</param>
		/// <param name="MaxChunkSize">Max Chunk Size to use.</param>
		public HttpxServer(XmppClient Client, HttpServer Server, int MaxChunkSize)
		{
			this.client = Client;
			this.server = Server;
			this.maxChunkSize = MaxChunkSize;

			HttpxChunks.RegisterChunkReceiver(this.client);

			this.client.RegisterIqSetHandler("req", HttpxClient.Namespace, this.ReqReceived, false);
			this.client.RegisterIqGetHandler("req", HttpxClient.Namespace, this.ReqReceived, false);
			this.client.RegisterMessageHandler("cancel", HttpxClient.Namespace, this.CancelReceived, false);
		}

		/// <summary>
		/// If end-to-end encryption is required to be able to access web content via HTTPX.
		/// Default is false.
		/// </summary>
		public bool RequiresE2e
		{
			get { return this.requiresE2e; }
			set { this.requiresE2e = value; }
		}

		public void Dispose()
		{
			HttpxChunks.UnregisterChunkReceiver(this.client);

			this.client.UnregisterIqSetHandler("req", HttpxClient.Namespace, this.ReqReceived, false);
			this.client.UnregisterIqGetHandler("req", HttpxClient.Namespace, this.ReqReceived, false);
			this.client.UnregisterMessageHandler("cancel", HttpxClient.Namespace, this.CancelReceived, false);
		}

		private void ReqReceived(object Sender, IqEventArgs e)
		{
			if (this.requiresE2e && !e.UsesE2eEncryption)
			{
				e.IqError(new Networking.XMPP.StanzaErrors.ForbiddenException("End-to-end encryption required.", e.IQ));
				return;
			}
			
			string Method = XML.Attribute(e.Query, "method");
			string Resource = XML.Attribute(e.Query, "resource");
			string Version = XML.Attribute(e.Query, "version");
			int MaxChunkSize = XML.Attribute(e.Query, "maxChunkSize", 0);
			bool Sipub = XML.Attribute(e.Query, "sipub", true);
			bool Ibb = XML.Attribute(e.Query, "ibb", true);
			bool Jingle = XML.Attribute(e.Query, "jingle", true);
			Stream DataStream = null;
			List<KeyValuePair<string, string>> HeaderFields = new List<KeyValuePair<string, string>>();
			HttpRequestHeader Header = null;

			if (MaxChunkSize <= 0 || MaxChunkSize > this.maxChunkSize)
				MaxChunkSize = this.maxChunkSize;

			foreach (XmlNode N in e.Query.ChildNodes)
			{
				switch (N.LocalName)
				{
					case "headers":
						foreach (XmlNode N2 in N.ChildNodes)
						{
							if (N2.LocalName == "header")
							{
								string Key = XML.Attribute((XmlElement)N2, "name");
								string Value = N2.InnerText;

								HeaderFields.Add(new KeyValuePair<string, string>(Key, Value));
							}
						}
						break;

					case "data":
						Header = new HttpRequestHeader(Method, Resource, Version, "httpx", HeaderFields.ToArray());

						foreach (XmlNode N2 in N.ChildNodes)
						{
							switch (N2.LocalName)
							{
								case "text":
									MemoryStream ms = new MemoryStream();

									if (Header.ContentType == null)
										Header.Add(new HttpField("Content-Type", "text/plain"));

									byte[] Data = Header.ContentType.Encoding.GetBytes(N2.InnerText);
									ms.Write(Data, 0, Data.Length);
									DataStream = ms;
									break;

								case "xml":
									ms = new MemoryStream();

									if (Header.ContentType == null)
										Header.Add(new HttpField("Content-Type", "text/xml"));

									Data = Header.ContentType.Encoding.GetBytes(N2.InnerText);
									ms.Write(Data, 0, Data.Length);
									ms.Position = 0;
									DataStream = ms;
									break;

								case "base64":
									ms = new MemoryStream();
									Data = Convert.FromBase64String(N2.InnerText);
									ms.Write(Data, 0, Data.Length);
									ms.Position = 0;
									DataStream = ms;
									break;

								case "chunkedBase64":
									TemporaryFile file = new TemporaryFile();
									string StreamId = XML.Attribute((XmlElement)N2, "streamId");
									HttpxChunks.chunkedStreams.Add(e.From + " " + StreamId, new ServerChunkRecord(this, e.Id, e.From, e.To,
										new HttpRequest(Header, DataStream, null), e.E2eEncryption, file, MaxChunkSize, Sipub, Ibb, Jingle));
									return;

								case "sipub":
									// TODO: Implement File Transfer support.
									break;

								case "ibb":
									// TODO: Implement In-band byte streams support.
									break;

								case "jingle":
									// TODO: Implement Jingle support.
									break;
							}
						}
						break;
				}
			}

			if (Header == null)
				Header = new HttpRequestHeader(Method, Resource, Version, "httpx", HeaderFields.ToArray());

			this.Process(e.Id, e.From, e.To, new HttpRequest(Header, DataStream, null), e.E2eEncryption,
				MaxChunkSize, Sipub, Ibb, Jingle);
		}

		internal void Process(string Id, string From, string To, HttpRequest Request, IEndToEndEncryption E2e,
			int MaxChunkSize, bool Sipub, bool Ibb, bool Jingle)
		{
			HttpAuthenticationScheme[] AuthenticationSchemes;
			HttpResource Resource;
			IUser User;
			string SubPath;
			bool Result;

			try
			{
				if (this.server.TryGetResource(Request.Header.Resource, out Resource, out SubPath))
				{
					this.server.RequestReceived(Request, From, Resource, SubPath);

					AuthenticationSchemes = Resource.GetAuthenticationSchemes(Request);
					if (AuthenticationSchemes != null && AuthenticationSchemes.Length > 0)
					{
						foreach (HttpAuthenticationScheme Scheme in AuthenticationSchemes)
						{
							if (Scheme.IsAuthenticated(Request, out User))
							{
								Request.User = User;
								break;
							}
						}

						if (Request.User == null)
						{
							List<KeyValuePair<string, string>> Challenges = new List<KeyValuePair<string, string>>();

							foreach (HttpAuthenticationScheme Scheme in AuthenticationSchemes)
								Challenges.Add(new KeyValuePair<string, string>("WWW-Authenticate", Scheme.GetChallenge()));
							
							this.SendResponse(Request, E2e, Id, From, To, 401, "Unauthorized", false, MaxChunkSize, Challenges.ToArray());
							Request.Dispose();
							return;
						}
					}

					Request.SubPath = SubPath;
					Resource.Validate(Request);

					if (Request.Header.Expect != null)
					{
						if (Request.Header.Expect.Continue100)
						{
							if (!Request.HasData)
							{
								this.SendResponse(Request, E2e, Id, From, To, 100, "Continue", false, MaxChunkSize);
								return;
							}
						}
						else
						{
							this.SendResponse(Request, E2e, Id, From, To, 417, "Expectation Failed", true, MaxChunkSize);
							Request.Dispose();
							return;
						}
					}

					Task.Run(() =>
					{
						HttpResponse Response = null;

						try
						{
							Response = new HttpResponse(
								new HttpxResponse(this.client, E2e, Id, From, To, MaxChunkSize), 
								this.server, Request);

							Resource.Execute(this.server, Request, Response);
						}
						catch (HttpException ex)
						{
							if (Response == null || !Response.HeaderSent)
								this.SendResponse(Request, E2e, Id, From, To, ex.StatusCode, ex.Message, true, MaxChunkSize, ex.HeaderFields);
						}
						catch (Exception ex)
						{
							Log.Critical(ex);

							if (Response == null || !Response.HeaderSent)
								this.SendResponse(Request, E2e, Id, From, To, 500, "Internal Server Error", true, MaxChunkSize);
						}
						finally
						{
							Request.Dispose();
						}
					});

					return;
				}
				else
				{
					this.server.RequestReceived(Request, From, null, null);
					this.SendResponse(Request, E2e, Id, From, To, 404, "Not Found", false, MaxChunkSize);
					Result = true;
				}
			}
			catch (HttpException ex)
			{
				Result = (Request.Header.Expect == null || !Request.Header.Expect.Continue100 || Request.HasData);
				this.SendResponse(Request, E2e, Id, From, To, ex.StatusCode, ex.Message, !Result, MaxChunkSize, ex.HeaderFields);
			}
			catch (System.NotImplementedException ex)
			{
				Result = (Request.Header.Expect == null || !Request.Header.Expect.Continue100 || Request.HasData);

				Log.Critical(ex);

				this.SendResponse(Request, E2e, Id, From, To, 501, "Not Implemented", !Result, MaxChunkSize);
			}
			catch (IOException ex)
			{
				Log.Critical(ex);

				int Win32ErrorCode = Marshal.GetHRForException(ex) & 0xFFFF;    // TODO: Update to ex.HResult when upgrading to .NET 4.5
				if (Win32ErrorCode == 0x27 || Win32ErrorCode == 0x70)   // ERROR_HANDLE_DISK_FULL, ERROR_DISK_FULL
				{
					this.SendResponse(Request, E2e, Id, From, To, 507, "Insufficient Storage", true, MaxChunkSize);
				}
				else
					this.SendResponse(Request, E2e, Id, From, To, 500, "Internal Server Error", true, MaxChunkSize);

				Result = false;
			}
			catch (Exception ex)
			{
				Result = (Request.Header.Expect == null || !Request.Header.Expect.Continue100 || Request.HasData);

				Log.Critical(ex);

				this.SendResponse(Request, E2e, Id, From, To, 500, "Internal Server Error", !Result, MaxChunkSize);
			}

			Request.Dispose();
		}

		private void SendResponse(HttpRequest Request, IEndToEndEncryption E2e, string Id, string To, string From, 
			int Code, string Message, bool CloseAfterTransmission, int MaxChunkSize, 
			params KeyValuePair<string, string>[] HeaderFields)
		{
			HttpResponse Response = new HttpResponse(new HttpxResponse(this.client, E2e, Id, To, From, MaxChunkSize),
				this.server, Request);

			Response.StatusCode = Code;
			Response.StatusMessage = Message;
			Response.ContentLength = null;
			Response.ContentType = null;
			Response.ContentLanguage = null;

			foreach (KeyValuePair<string, string> P in HeaderFields)
				Response.SetHeader(P.Key, P.Value);

			if (CloseAfterTransmission)
				Response.SetHeader("Connection", "close");

			Response.SendResponse();

			// TODO: Add error message content.
		}

		private void CancelReceived(object Sender, MessageEventArgs e)
		{
			if (this.requiresE2e && !e.UsesE2eEncryption)
				return;

			string StreamId = XML.Attribute(e.Content, "streamId");

			HttpxResponse.CancelChunkedTransfer(e.From, e.To, StreamId);
		}

	}
}
