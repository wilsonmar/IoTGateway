﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Waher.Networking.XMPP.DataForms.DataTypes
{
	/// <summary>
	/// Abstract base class for all data types used in forms.
	/// </summary>
	public abstract class DataType
	{
		private string typeName;

		/// <summary>
		/// Abstract base class for all data types used in forms.
		/// </summary>
		/// <param name="TypeName">Type Name</param>
		public DataType(string TypeName)
		{
			this.typeName = TypeName;
		}

		/// <summary>
		/// Type Name
		/// </summary>
		public string TypeName { get { return this.typeName; } }

		/// <summary>
		/// Parses a string.
		/// </summary>
		/// <param name="Value">String value.</param>
		/// <returns>Parsed value, if possible, null otherwise.</returns>
		public abstract object Parse(string Value);
	}
}
