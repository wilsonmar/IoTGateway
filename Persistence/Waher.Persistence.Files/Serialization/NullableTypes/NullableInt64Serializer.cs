﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waher.Persistence.Files.Serialization.NullableTypes
{
	public class NullableInt64Serializer : IObjectSerializer
	{
		public NullableInt64Serializer()
		{
		}

		public Type ValueType
		{
			get
			{
				return typeof(long?);
			}
		}

		public bool IsNullable
		{
			get { return true; }
		}

		public object Deserialize(BinaryDeserializer Reader, uint? DataType, bool Embedded)
		{
			if (!DataType.HasValue)
				DataType = Reader.ReadBits(6);

			switch (DataType.Value)
			{
				case ObjectSerializer.TYPE_BOOLEAN: return Reader.ReadBoolean() ? (long?)1 : (long?)0;
				case ObjectSerializer.TYPE_BYTE: return (long?)Reader.ReadByte();
				case ObjectSerializer.TYPE_INT16: return (long?)Reader.ReadInt16();
				case ObjectSerializer.TYPE_INT32: return (long?)Reader.ReadInt32();
				case ObjectSerializer.TYPE_INT64: return (long?)Reader.ReadInt64();
				case ObjectSerializer.TYPE_SBYTE: return (long?)Reader.ReadSByte();
				case ObjectSerializer.TYPE_UINT16: return (long?)Reader.ReadUInt16();
				case ObjectSerializer.TYPE_UINT32: return (long?)Reader.ReadUInt32();
				case ObjectSerializer.TYPE_UINT64: return (long?)Reader.ReadUInt64();
				case ObjectSerializer.TYPE_DECIMAL: return (long?)Reader.ReadDecimal();
				case ObjectSerializer.TYPE_DOUBLE: return (long?)Reader.ReadDouble();
				case ObjectSerializer.TYPE_SINGLE: return (long?)Reader.ReadSingle();
				case ObjectSerializer.TYPE_STRING: return (long?)long.Parse(Reader.ReadString());
				case ObjectSerializer.TYPE_NULL: return null;
				default: throw new Exception("Expected a nullable Int64 value.");
			}
		}

		public void Serialize(BinarySerializer Writer, bool WriteTypeCode, bool Embedded, object Value)
		{
			long? Value2 = (long?)Value;

			if (WriteTypeCode)
			{
				if (!Value2.HasValue)
				{
					Writer.WriteBits(ObjectSerializer.TYPE_NULL, 6);
					return;
				}
				else
					Writer.WriteBits(ObjectSerializer.TYPE_INT64, 6);
			}
			else if (!Value2.HasValue)
				throw new NullReferenceException("Value cannot be null.");

			Writer.Write(Value2.Value);
		}

	}
}
