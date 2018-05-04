/*
 JustMock Lite
 Copyright © 2010-2015 Telerik EAD

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections;
using System.ComponentModel;

namespace Telerik.JustMock
{
	/// <summary>
	/// Defines parameter placeholders when the parameter type is one of the commonly occurring types, e.g. int.
	/// Example: Mock.Create&lt;IEqualityComparer&gt;(me => me.Equals(Arg.AnyObject, Arg.AnyObject) == Equals(Param._1, Param._2));
	/// In the example, Param._1 and Param._2 are implicitly converted to System.Object.
	/// </summary>
	public static class Param
	{
		/// <summary>
		/// First parameter.
		/// </summary>
		public static readonly EverythingExcept _1;
		/// <summary>
		/// Second parameter.
		/// </summary>
		public static readonly EverythingExcept _2;
		/// <summary>
		/// Third parameter.
		/// </summary>
		public static readonly EverythingExcept _3;
		/// <summary>
		/// Fourth parameter.
		/// </summary>
		public static readonly EverythingExcept _4;
		/// <summary>
		/// Fifth parameter.
		/// </summary>
		public static readonly EverythingExcept _5;
		/// <summary>
		/// Sixth parameter.
		/// </summary>
		public static readonly EverythingExcept _6;
		/// <summary>
		/// Seventh parameter.
		/// </summary>
		public static readonly EverythingExcept _7;
		/// <summary>
		/// Eighth parameter.
		/// </summary>
		public static readonly EverythingExcept _8;
		/// <summary>
		/// Ninth parameter.
		/// </summary>
		public static readonly EverythingExcept _9;
		/// <summary>
		/// Tenth parameter.
		/// </summary>
		public static readonly EverythingExcept _10;
		/// <summary>
		/// Eleventh parameter.
		/// </summary>
		public static readonly EverythingExcept _11;
		/// <summary>
		/// Twelfth parameter.
		/// </summary>
		public static readonly EverythingExcept _12;
		/// <summary>
		/// Thirteenth parameter.
		/// </summary>
		public static readonly EverythingExcept _13;
		/// <summary>
		/// Fourteenth parameter.
		/// </summary>
		public static readonly EverythingExcept _14;
		/// <summary>
		/// Fifteenth parameter.
		/// </summary>
		public static readonly EverythingExcept _15;
		/// <summary>
		/// Sixteenth parameter.
		/// </summary>
		public static readonly EverythingExcept _16;

		/// <summary>
		/// This class appears only in compiler errors.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public struct EverythingExcept : IDictionary, IList, IAsyncResult, IFormatProvider, IComparer, IConvertible, IEqualityComparer, IDisposable
#if !SILVERLIGHT && !PORTABLE
, ICloneable
#endif
		{
			#region Implicit conversions
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator bool(EverythingExcept _) { return default(bool); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator byte(EverythingExcept _) { return 0; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator sbyte(EverythingExcept _) { return 0; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator char(EverythingExcept _) { return ' '; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator short(EverythingExcept _) { return 0; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator ushort(EverythingExcept _) { return 0; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator int(EverythingExcept _) { return 0; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator uint(EverythingExcept _) { return 0; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator IntPtr(EverythingExcept _) { return default(IntPtr); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator UIntPtr(EverythingExcept _) { return default(UIntPtr); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator long(EverythingExcept _) { return 0; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator ulong(EverythingExcept _) { return 0; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator float(EverythingExcept _) { return 0; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator double(EverythingExcept _) { return 0; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator decimal(EverythingExcept _) { return 0; }

			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator string(EverythingExcept _) { return default(String); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator DateTime(EverythingExcept _) { return default(DateTime); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator TimeSpan(EverythingExcept _) { return default(TimeSpan); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator Guid(EverythingExcept _) { return default(System.Guid); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator DateTimeOffset(EverythingExcept _) { return default(System.DateTimeOffset); }

			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator bool[](EverythingExcept _) { return null; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator byte[](EverythingExcept _) { return null; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator sbyte[](EverythingExcept _) { return null; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator char[](EverythingExcept _) { return null; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator short[](EverythingExcept _) { return null; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator ushort[](EverythingExcept _) { return null; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator int[](EverythingExcept _) { return null; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator uint[](EverythingExcept _) { return null; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator IntPtr[](EverythingExcept _) { return null; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator UIntPtr[](EverythingExcept _) { return null; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator long[](EverythingExcept _) { return null; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator ulong[](EverythingExcept _) { return null; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator float[](EverythingExcept _) { return null; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator double[](EverythingExcept _) { return null; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator decimal[](EverythingExcept _) { return null; }

			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator object[](EverythingExcept _) { return default(Object[]); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator string[](EverythingExcept _) { return default(String[]); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator Type[](EverythingExcept _) { return default(Type[]); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator Attribute[](EverythingExcept _) { return default(Attribute[]); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator DateTime[](EverythingExcept _) { return null; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator TimeSpan[](EverythingExcept _) { return null; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator Guid[](EverythingExcept _) { return null; }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator DateTimeOffset[](EverythingExcept _) { return null; }

			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator Type(EverythingExcept _) { return default(Type); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator Enum(EverythingExcept _) { return default(Enum); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.Runtime.Serialization.StreamingContext(EverythingExcept _) { return default(System.Runtime.Serialization.StreamingContext); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator AsyncCallback(EverythingExcept _) { return default(AsyncCallback); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.Linq.Expressions.Expression(EverythingExcept _) { return default(System.Linq.Expressions.Expression); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.Globalization.CultureInfo(EverythingExcept _) { return default(System.Globalization.CultureInfo); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.Reflection.BindingFlags(EverythingExcept _) { return default(System.Reflection.BindingFlags); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator Delegate(EverythingExcept _) { return default(Delegate); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.Reflection.MethodInfo(EverythingExcept _) { return default(System.Reflection.MethodInfo); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.Xml.XmlReader(EverythingExcept _) { return default(System.Xml.XmlReader); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.IO.Stream(EverythingExcept _) { return default(System.IO.Stream); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator EventHandler(EverythingExcept _) { return default(EventHandler); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.Xml.XmlWriter(EverythingExcept _) { return default(System.Xml.XmlWriter); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator Uri(EverythingExcept _) { return default(Uri); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator Exception(EverythingExcept _) { return default(Exception); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.Xml.XmlNameTable(EverythingExcept _) { return default(System.Xml.XmlNameTable); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator Array(EverythingExcept _) { return default(Array); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.IO.TextWriter(EverythingExcept _) { return default(System.IO.TextWriter); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator EventArgs(EverythingExcept _) { return default(EventArgs); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.Xml.XmlQualifiedName(EverythingExcept _) { return default(System.Xml.XmlQualifiedName); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.Text.StringBuilder(EverythingExcept _) { return default(System.Text.StringBuilder); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.Collections.BitArray(EverythingExcept _) { return default(System.Collections.BitArray); }
			#endregion

#if !PORTABLE
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.Reflection.Binder(EverythingExcept _) { return default(System.Reflection.Binder); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.Reflection.ParameterModifier[](EverythingExcept _) { return default(System.Reflection.ParameterModifier[]); }
#endif

#if !SILVERLIGHT && !PORTABLE
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.Runtime.Serialization.SerializationInfo(EverythingExcept _) { return default(System.Runtime.Serialization.SerializationInfo); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.Xml.XmlElement(EverythingExcept _) { return default(System.Xml.XmlElement); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.Xml.XmlNode(EverythingExcept _) { return default(System.Xml.XmlNode); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.Xml.Schema.XmlSchemaDatatype(EverythingExcept _) { return default(System.Xml.Schema.XmlSchemaDatatype); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.Xml.Schema.XmlSchemaType(EverythingExcept _) { return default(System.Xml.Schema.XmlSchemaType); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.Xml.Schema.XmlSchemaObjectCollection(EverythingExcept _) { return default(System.Xml.Schema.XmlSchemaObjectCollection); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.Xml.Schema.XmlSchemaObject(EverythingExcept _) { return default(System.Xml.Schema.XmlSchemaObject); }
			/// <summary> </summary>
			/// <param name="_"></param>
			/// <returns></returns>
			public static implicit operator System.Xml.XPath.XPathNavigator(EverythingExcept _) { return default(System.Xml.XPath.XPathNavigator); }

			#region ICloneable
			object ICloneable.Clone()
			{
				throw new NotImplementedException();
			}
			#endregion
#endif

			#region Interface implementations

			void IDictionary.Add(object key, object value)
			{
				throw new NotImplementedException();
			}

			void IDictionary.Clear()
			{
				throw new NotImplementedException();
			}

			bool IDictionary.Contains(object key)
			{
				throw new NotImplementedException();
			}

			IDictionaryEnumerator IDictionary.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			bool IDictionary.IsFixedSize
			{
				get { throw new NotImplementedException(); }
			}

			bool IDictionary.IsReadOnly
			{
				get { throw new NotImplementedException(); }
			}

			ICollection IDictionary.Keys
			{
				get { throw new NotImplementedException(); }
			}

			void IDictionary.Remove(object key)
			{
				throw new NotImplementedException();
			}

			ICollection IDictionary.Values
			{
				get { throw new NotImplementedException(); }
			}

			object IDictionary.this[object key]
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			void ICollection.CopyTo(Array array, int index)
			{
				throw new NotImplementedException();
			}

			int ICollection.Count
			{
				get { throw new NotImplementedException(); }
			}

			bool ICollection.IsSynchronized
			{
				get { throw new NotImplementedException(); }
			}

			object ICollection.SyncRoot
			{
				get { throw new NotImplementedException(); }
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			int IList.Add(object value)
			{
				throw new NotImplementedException();
			}

			void IList.Clear()
			{
				throw new NotImplementedException();
			}

			bool IList.Contains(object value)
			{
				throw new NotImplementedException();
			}

			int IList.IndexOf(object value)
			{
				throw new NotImplementedException();
			}

			void IList.Insert(int index, object value)
			{
				throw new NotImplementedException();
			}

			bool IList.IsFixedSize
			{
				get { throw new NotImplementedException(); }
			}

			bool IList.IsReadOnly
			{
				get { throw new NotImplementedException(); }
			}

			void IList.Remove(object value)
			{
				throw new NotImplementedException();
			}

			void IList.RemoveAt(int index)
			{
				throw new NotImplementedException();
			}

			object IList.this[int index]
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			object IAsyncResult.AsyncState
			{
				get { throw new NotImplementedException(); }
			}

			System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle
			{
				get { throw new NotImplementedException(); }
			}

			bool IAsyncResult.CompletedSynchronously
			{
				get { throw new NotImplementedException(); }
			}

			bool IAsyncResult.IsCompleted
			{
				get { throw new NotImplementedException(); }
			}

			object IFormatProvider.GetFormat(Type formatType)
			{
				throw new NotImplementedException();
			}

			int IComparer.Compare(object x, object y)
			{
				throw new NotImplementedException();
			}

			TypeCode IConvertible.GetTypeCode()
			{
				throw new NotImplementedException();
			}

			bool IConvertible.ToBoolean(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			byte IConvertible.ToByte(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			char IConvertible.ToChar(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			DateTime IConvertible.ToDateTime(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			decimal IConvertible.ToDecimal(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			double IConvertible.ToDouble(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			short IConvertible.ToInt16(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			int IConvertible.ToInt32(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			long IConvertible.ToInt64(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			sbyte IConvertible.ToSByte(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			float IConvertible.ToSingle(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			string IConvertible.ToString(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			object IConvertible.ToType(Type conversionType, IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			ushort IConvertible.ToUInt16(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			uint IConvertible.ToUInt32(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			ulong IConvertible.ToUInt64(IFormatProvider provider)
			{
				throw new NotImplementedException();
			}

			bool IEqualityComparer.Equals(object x, object y)
			{
				throw new NotImplementedException();
			}

			int IEqualityComparer.GetHashCode(object obj)
			{
				throw new NotImplementedException();
			}

			void IDisposable.Dispose()
			{
				throw new NotImplementedException();
			}
			#endregion
		}
	}

	/// <summary>
	/// Defines parameter placeholders when the parameter type is T.
	/// </summary>
	/// <typeparam name="T">Type of the parameter.</typeparam>
	public static class Param<T>
	{
		/// <summary>
		/// First field.
		/// </summary>
		public static readonly T _1;
		/// <summary>
		/// Second field.
		/// </summary>
		public static readonly T _2;
		/// <summary>
		/// Third field.
		/// </summary>
		public static readonly T _3;
		/// <summary>
		/// Fourth field.
		/// </summary>
		public static readonly T _4;
		/// <summary>
		/// Fifth field.
		/// </summary>
		public static readonly T _5;
		/// <summary>
		/// Sixth field.
		/// </summary>
		public static readonly T _6;
		/// <summary>
		/// Seventh field.
		/// </summary>
		public static readonly T _7;
		/// <summary>
		/// Eighth field.
		/// </summary>
		public static readonly T _8;
		/// <summary>
		/// Ninth field.
		/// </summary>
		public static readonly T _9;
		/// <summary>
		/// Tenth field.
		/// </summary>
		public static readonly T _10;
		/// <summary>
		/// Eleventh field.
		/// </summary>
		public static readonly T _11;
		/// <summary>
		/// Twelfth field.
		/// </summary>
		public static readonly T _12;
		/// <summary>
		/// Thirteenth field.
		/// </summary>
		public static readonly T _13;
		/// <summary>
		/// Fourteenth field.
		/// </summary>
		public static readonly T _14;
		/// <summary>
		/// Fifteenth field.
		/// </summary>
		public static readonly T _15;
		/// <summary>
		/// Sixteenth field.
		/// </summary>
		public static readonly T _16;
	}
}
