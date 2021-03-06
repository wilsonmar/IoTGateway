﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Waher.Networking.XMPP.DataForms.DataTypes
{
	/// <summary>
	/// Time Data Type (xs:time)
	/// </summary>
	public class TimeDataType : DataType
	{
		/// <summary>
		/// Time Data Type (xs:time)
		/// </summary>
		/// <param name="TypeName">Type Name</param>
		public TimeDataType(string DataType)
			: base(DataType)
		{
		}

		/// <summary>
		/// Parses a string.
		/// </summary>
		/// <param name="Value">String value.</param>
		/// <returns>Parsed value, if possible, null otherwise.</returns>
		public override object Parse(string Value)
		{
			TimeSpan Result;

			if (TimeSpan.TryParse(Value, out Result))
				return Result;
			else
				return null;
		}
	}
}
