﻿using System;
using System.Collections.Generic;
using System.Text;
using Waher.Script.Abstraction.Elements;
using Waher.Script.Model;
using Waher.Script.Objects;
using Waher.Script.Objects.Sets;

namespace Waher.Script.Constants
{
    /// <summary>
    /// Current date.
    /// </summary>
    public class Today : IConstant
	{
        /// <summary>
        /// Current date.
        /// </summary>
        public Today()
		{
		}

		/// <summary>
		/// Name of the constant
		/// </summary>
		public string ConstantName
		{
			get { return "Today"; }
		}

		/// <summary>
		/// Optional aliases. If there are no aliases for the constant, null is returned.
		/// </summary>
		public string[] Aliases
		{
			get { return null; }
		}

		/// <summary>
		/// Constant value element.
		/// </summary>
		public IElement ValueElement
		{
			get { return new DateTimeValue(DateTime.Today); }
		}

	}
}
