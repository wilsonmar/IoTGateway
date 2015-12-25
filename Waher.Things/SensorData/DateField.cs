﻿using System;
using System.Collections.Generic;
using System.Text;
using Waher.Networking;

namespace Waher.Things.SensorData
{
	/// <summary>
	/// Represents a date value.
	/// </summary>
	public class DateField : Field
	{
		private DateTime value;

		/// <summary>
		/// Represents a date value.
		/// </summary>
		/// <param name="Name">Field Name.</param>
		/// <param name="Value">Field Value.</param>
		/// <param name="Type">Field Type flags.</param>
		/// <param name="QoS">Quality of Service flags.</param>
		/// <param name="Writable">If the field is writable, i.e. corresponds to a control parameter.</param>
		/// <param name="Module">Language Module for localization purposes.</param>
		/// <param name="StringIdSteps">String ID steps.</param>
		public DateField(string Name, DateTime Value, FieldType Type, FieldQoS QoS, bool Writable, string Module, params LocalizationStep[] StringIdSteps)
			: base(Name, Type, QoS, Writable, Module, StringIdSteps)
		{
			this.value = Value;
		}

		/// <summary>
		/// Represents a date value.
		/// </summary>
		/// <param name="Name">Field Name.</param>
		/// <param name="Value">Field Value.</param>
		/// <param name="Type">Field Type flags.</param>
		/// <param name="QoS">Quality of Service flags.</param>
		/// <param name="Writable">If the field is writable, i.e. corresponds to a control parameter.</param>
		/// <param name="Module">Language Module for localization purposes.</param>
		/// <param name="StringIdSteps">String ID steps.</param>
		public DateField(string Name, DateTime Value, FieldType Type, FieldQoS QoS, bool Writable, string Module, params int[] StringIds)
			: base(Name, Type, QoS, Writable, Module, StringIds)
		{
			this.value = Value;
		}

		/// <summary>
		/// Represents a date value.
		/// </summary>
		/// <param name="Name">Field Name.</param>
		/// <param name="Value">Field Value.</param>
		/// <param name="Type">Field Type flags.</param>
		/// <param name="QoS">Quality of Service flags.</param>
		/// <param name="Module">Language Module for localization purposes.</param>
		/// <param name="StringIdSteps">String ID steps.</param>
		public DateField(string Name, DateTime Value, FieldType Type, FieldQoS QoS, string Module, params LocalizationStep[] StringIdSteps)
			: base(Name, Type, QoS, Module, StringIdSteps)
		{
			this.value = Value;
		}

		/// <summary>
		/// Represents a date value.
		/// </summary>
		/// <param name="Name">Field Name.</param>
		/// <param name="Value">Field Value.</param>
		/// <param name="Type">Field Type flags.</param>
		/// <param name="QoS">Quality of Service flags.</param>
		/// <param name="Module">Language Module for localization purposes.</param>
		/// <param name="StringIdSteps">String ID steps.</param>
		public DateField(string Name, DateTime Value, FieldType Type, FieldQoS QoS, string Module, params int[] StringIds)
			: base(Name, Type, QoS, Module, StringIds)
		{
			this.value = Value;
		}

		/// <summary>
		/// Represents a date value.
		/// </summary>
		/// <param name="Name">Field Name.</param>
		/// <param name="Value">Field Value.</param>
		/// <param name="Type">Field Type flags.</param>
		/// <param name="QoS">Quality of Service flags.</param>
		/// <param name="Writable">If the field is writable, i.e. corresponds to a control parameter.</param>
		public DateField(string Name, DateTime Value, FieldType Type, FieldQoS QoS, bool Writable)
			: base(Name, Type, QoS, Writable)
		{
			this.value = Value;
		}

		/// <summary>
		/// Represents a date value.
		/// </summary>
		/// <param name="Name">Field Name.</param>
		/// <param name="Value">Field Value.</param>
		/// <param name="Type">Field Type flags.</param>
		/// <param name="QoS">Quality of Service flags.</param>
		public DateField(string Name, DateTime Value, FieldType Type, FieldQoS QoS)
			: base(Name, Type, QoS)
		{
			this.value = Value;
		}

		/// <summary>
		/// Field Value
		/// </summary>
		public DateTime Value { get { return this.value; } }

		/// <summary>
		/// String representation of field value.
		/// </summary>
		public override string ValueString
		{
			get { return CommonTypes.XmlEncode(this.value, true); }
		}

	}
}