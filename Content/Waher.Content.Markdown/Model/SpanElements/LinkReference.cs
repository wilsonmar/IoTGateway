﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Waher.Content.Markdown.Model.SpanElements
{
	/// <summary>
	/// Link reference
	/// </summary>
	public class LinkReference : MarkdownElementChildren
	{
		private string label;

		/// <summary>
		/// Link
		/// </summary>
		public LinkReference(MarkdownDocument Document, LinkedList<MarkdownElement> ChildElements, string Label)
			: base(Document, ChildElements)
		{
			this.label = Label;
		}

		/// <summary>
		/// Link label
		/// </summary>
		private string Label
		{
			get { return this.label; }
		}

		/// <summary>
		/// Generates HTML for the markdown element.
		/// </summary>
		/// <param name="Output">HTML will be output here.</param>
		public override void GenerateHTML(StringBuilder Output)
		{
			Link Link = this.Document.GetReference(this.label);

			if (Link != null)
				Link.GenerateHTML(Output, Link.Url, Link.Title, this.Children);
			else
			{
				foreach (MarkdownElement E in this.Children)
					E.GenerateHTML(Output);
			}
		}

	}
}