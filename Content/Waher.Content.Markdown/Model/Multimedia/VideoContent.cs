﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Waher.Script;

namespace Waher.Content.Markdown.Model.Multimedia
{
	/// <summary>
	/// Video content.
	/// </summary>
	public class VideoContent : IMultimediaContent
	{
		/// <summary>
		/// Video content.
		/// </summary>
		public VideoContent()
		{
		}

		/// <summary>
		/// Checks how well the handler supports multimedia content of a given type.
		/// </summary>
		/// <param name="Url">URL to content.</param>
		/// <returns>How well the handler supports the content.</returns>
		public Grade Supports(string Url)
		{
			string Extension = Path.GetExtension(Url);
			string ContentType;

			if (InternetContent.TryGetContentType(Extension, out ContentType) && ContentType.StartsWith("video/"))
				return Grade.Ok;
			else
				return Grade.NotAtAll;
		}

		/// <summary>
		/// Generates HTML for the markdown element.
		/// </summary>
		/// <param name="Output">HTML will be output here.</param>
		/// <param name="Url">URL</param>
		/// <param name="Title">Optional title.</param>
		/// <param name="Width">Optional width.</param>
		/// <param name="Height">Optional height.</param>
		/// <param name="ChildNodes">Child nodes.</param>
		public void GenerateHTML(StringBuilder Output, string Url, string Title, int? Width, int? Height, IEnumerable<MarkdownElement> ChildNodes)
		{
			Output.Append("<video autoplay=\"autoplay\" controls=\"controls\" src=\"");
			Output.Append(MarkdownDocument.HtmlEncode(Url));

			if (Width.HasValue)
			{
				Output.Append("\" width=\"");
				Output.Append(Width.Value.ToString());
			}

			if (Height.HasValue)
			{
				Output.Append("\" height=\"");
				Output.Append(Height.Value.ToString());
			}

			Output.Append("\">");

			foreach (MarkdownElement E in ChildNodes)
				E.GenerateHTML(Output);

			Output.Append("</video>");
		}
	}
}