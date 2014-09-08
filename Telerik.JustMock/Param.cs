/*
 JustMock Lite
 Copyright © 2010-2014 Telerik AD

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
        /// First field.
        /// </summary>
        public static readonly EverythingExcept _1;
        /// <summary>
        /// Second field.
        /// </summary>
        public static readonly EverythingExcept _2;
        /// <summary>
        /// Third field.
        /// </summary>
        public static readonly EverythingExcept _3;
        /// <summary>
        /// Fourth field.
        /// </summary>
        public static readonly EverythingExcept _4;
        /// <summary>
        /// Fifth field.
        /// </summary>
        public static readonly EverythingExcept _5;
        /// <summary>
        /// Sixth field.
        /// </summary>
        public static readonly EverythingExcept _6;
        /// <summary>
        /// Seventh field.
        /// </summary>
        public static readonly EverythingExcept _7;
        /// <summary>
        /// Eighth field.
        /// </summary>
        public static readonly EverythingExcept _8;
        /// <summary>
        /// Ninth field.
        /// </summary>
        public static readonly EverythingExcept _9;
        /// <summary>
        /// Tenth field.
        /// </summary>
        public static readonly EverythingExcept _10;
        /// <summary>
        /// Eleventh field.
        /// </summary>
        public static readonly EverythingExcept _11;
        /// <summary>
        /// Twelfth field.
        /// </summary>
        public static readonly EverythingExcept _12;
        /// <summary>
        /// Thirteenth field.
        /// </summary>
        public static readonly EverythingExcept _13;
        /// <summary>
        /// Fourteenth field.
        /// </summary>
        public static readonly EverythingExcept _14;
        /// <summary>
        /// Fifteenth field.
        /// </summary>
        public static readonly EverythingExcept _15;
        /// <summary>
        /// Sixteenth field.
        /// </summary>
        public static readonly EverythingExcept _16;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public struct EverythingExcept : IDictionary, IList, IAsyncResult, IFormatProvider, IComparer, IConvertible, IEqualityComparer, IDisposable
#if !SILVERLIGHT
            ,
ICloneable
#endif
        {
            public static implicit operator bool(EverythingExcept _) { return default(bool); }
            public static implicit operator byte(EverythingExcept _) { return 0; }
            public static implicit operator sbyte(EverythingExcept _) { return 0; }
            public static implicit operator char(EverythingExcept _) { return ' '; }
            public static implicit operator short(EverythingExcept _) { return 0; }
            public static implicit operator ushort(EverythingExcept _) { return 0; }
            public static implicit operator int(EverythingExcept _) { return 0; }
            public static implicit operator uint(EverythingExcept _) { return 0; }
            public static implicit operator IntPtr(EverythingExcept _) { return default(IntPtr); }
            public static implicit operator UIntPtr(EverythingExcept _) { return default(UIntPtr); }
            public static implicit operator long(EverythingExcept _) { return 0; }
            public static implicit operator ulong(EverythingExcept _) { return 0; }
            public static implicit operator float(EverythingExcept _) { return 0; }
            public static implicit operator double(EverythingExcept _) { return 0; }
            public static implicit operator decimal(EverythingExcept _) { return 0; }

            public static implicit operator string(EverythingExcept _) { return default(String); }
            public static implicit operator DateTime(EverythingExcept _) { return default(DateTime); }
            public static implicit operator TimeSpan(EverythingExcept _) { return default(TimeSpan); }
            public static implicit operator Guid(EverythingExcept _) { return default(System.Guid); }
            public static implicit operator DateTimeOffset(EverythingExcept _) { return default(System.DateTimeOffset); }

            public static implicit operator bool[](EverythingExcept _) { return null; }
            public static implicit operator byte[](EverythingExcept _) { return null; }
            public static implicit operator sbyte[](EverythingExcept _) { return null; }
            public static implicit operator char[](EverythingExcept _) { return null; }
            public static implicit operator short[](EverythingExcept _) { return null; }
            public static implicit operator ushort[](EverythingExcept _) { return null; }
            public static implicit operator int[](EverythingExcept _) { return null; }
            public static implicit operator uint[](EverythingExcept _) { return null; }
            public static implicit operator IntPtr[](EverythingExcept _) { return null; }
            public static implicit operator UIntPtr[](EverythingExcept _) { return null; }
            public static implicit operator long[](EverythingExcept _) { return null; }
            public static implicit operator ulong[](EverythingExcept _) { return null; }
            public static implicit operator float[](EverythingExcept _) { return null; }
            public static implicit operator double[](EverythingExcept _) { return null; }
            public static implicit operator decimal[](EverythingExcept _) { return null; }

            public static implicit operator object[](EverythingExcept _) { return default(Object[]); }
            public static implicit operator string[](EverythingExcept _) { return default(String[]); }
            public static implicit operator Type[](EverythingExcept _) { return default(Type[]); }
            public static implicit operator Attribute[](EverythingExcept _) { return default(Attribute[]); }
            public static implicit operator DateTime[](EverythingExcept _) { return null; }
            public static implicit operator TimeSpan[](EverythingExcept _) { return null; }
            public static implicit operator Guid[](EverythingExcept _) { return null; }
            public static implicit operator DateTimeOffset[](EverythingExcept _) { return null; }

            public static implicit operator Type(EverythingExcept _) { return default(Type); }
            public static implicit operator Enum(EverythingExcept _) { return default(Enum); }
            public static implicit operator System.Runtime.Serialization.StreamingContext(EverythingExcept _) { return default(System.Runtime.Serialization.StreamingContext); }
            public static implicit operator AsyncCallback(EverythingExcept _) { return default(AsyncCallback); }
            public static implicit operator System.Linq.Expressions.Expression(EverythingExcept _) { return default(System.Linq.Expressions.Expression); }
            public static implicit operator System.Globalization.CultureInfo(EverythingExcept _) { return default(System.Globalization.CultureInfo); }
            public static implicit operator System.Reflection.BindingFlags(EverythingExcept _) { return default(System.Reflection.BindingFlags); }
            public static implicit operator Delegate(EverythingExcept _) { return default(Delegate); }
            public static implicit operator System.Reflection.MethodInfo(EverythingExcept _) { return default(System.Reflection.MethodInfo); }
            public static implicit operator System.Xml.XmlReader(EverythingExcept _) { return default(System.Xml.XmlReader); }
            public static implicit operator System.IO.Stream(EverythingExcept _) { return default(System.IO.Stream); }
            public static implicit operator EventHandler(EverythingExcept _) { return default(EventHandler); }
            public static implicit operator System.Xml.XmlWriter(EverythingExcept _) { return default(System.Xml.XmlWriter); }
            public static implicit operator System.Reflection.Binder(EverythingExcept _) { return default(System.Reflection.Binder); }
            public static implicit operator Uri(EverythingExcept _) { return default(Uri); }
            public static implicit operator Exception(EverythingExcept _) { return default(Exception); }
            public static implicit operator System.Reflection.ParameterModifier[](EverythingExcept _) { return default(System.Reflection.ParameterModifier[]); }
            public static implicit operator System.Xml.XmlNameTable(EverythingExcept _) { return default(System.Xml.XmlNameTable); }
            public static implicit operator Array(EverythingExcept _) { return default(Array); }
            public static implicit operator System.IO.TextWriter(EverythingExcept _) { return default(System.IO.TextWriter); }
            public static implicit operator EventArgs(EverythingExcept _) { return default(EventArgs); }
            public static implicit operator System.Xml.XmlQualifiedName(EverythingExcept _) { return default(System.Xml.XmlQualifiedName); }
            public static implicit operator System.Text.StringBuilder(EverythingExcept _) { return default(System.Text.StringBuilder); }
            public static implicit operator System.Collections.BitArray(EverythingExcept _) { return default(System.Collections.BitArray); }

#if !SILVERLIGHT
            public static implicit operator System.Runtime.Serialization.SerializationInfo(EverythingExcept _) { return default(System.Runtime.Serialization.SerializationInfo); }
            public static implicit operator System.Xml.XmlElement(EverythingExcept _) { return default(System.Xml.XmlElement); }
            public static implicit operator System.Xml.XmlNode(EverythingExcept _) { return default(System.Xml.XmlNode); }
            public static implicit operator System.Xml.Schema.XmlSchemaDatatype(EverythingExcept _) { return default(System.Xml.Schema.XmlSchemaDatatype); }
            public static implicit operator System.Xml.Schema.XmlSchemaType(EverythingExcept _) { return default(System.Xml.Schema.XmlSchemaType); }
            public static implicit operator System.Xml.Schema.XmlSchemaObjectCollection(EverythingExcept _) { return default(System.Xml.Schema.XmlSchemaObjectCollection); }
            public static implicit operator System.Xml.Schema.XmlSchemaObject(EverythingExcept _) { return default(System.Xml.Schema.XmlSchemaObject); }
            public static implicit operator System.Xml.XPath.XPathNavigator(EverythingExcept _) { return default(System.Xml.XPath.XPathNavigator); }

            #region ICloneable
            public object Clone()
            {
                throw new NotImplementedException();
            }
            #endregion
#endif

            #region IDictionary
            public void CopyTo(Array array, int index)
            {
                // TODO: Implement this method
                throw new NotImplementedException();
            }

            public int Count { get; private set; }

            public object SyncRoot { get; private set; }

            public bool IsSynchronized { get; private set; }

            public bool Contains(object key)
            {
                // TODO: Implement this method
                throw new NotImplementedException();
            }

            public void Add(object key, object value)
            {
                // TODO: Implement this method
                throw new NotImplementedException();
            }

            public void Clear()
            {
                // TODO: Implement this method
                throw new NotImplementedException();
            }

            public IDictionaryEnumerator GetEnumerator()
            {
                // TODO: Implement this method
                throw new NotImplementedException();
            }

            public void Remove(object key)
            {
                // TODO: Implement this method
                throw new NotImplementedException();
            }

            public object this[object key]
            {
                get
                {
                    // TODO: Implement this indexer getter
                    throw new NotImplementedException();
                }
                set
                {
                    // TODO: Implement this indexer setter
                    throw new NotImplementedException();
                }
            }

            public ICollection Keys { get; private set; }

            public ICollection Values { get; private set; }

            public bool IsReadOnly { get; private set; }

            public bool IsFixedSize { get; private set; }

            IEnumerator IEnumerable.GetEnumerator()
            {
                // TODO: Implement this method
                throw new NotImplementedException();
            }
            #endregion

            #region IList
            public int Add(object value)
            {
                throw new NotImplementedException();
            }

            public int IndexOf(object value)
            {
                throw new NotImplementedException();
            }

            public void Insert(int index, object value)
            {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            public object this[int index]
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
            #endregion

            #region IFormatProvider
            public object GetFormat(Type formatType)
            {
                throw new NotImplementedException();
            }
            #endregion

            #region IAsyncResult
            public object AsyncState
            {
                get { throw new NotImplementedException(); }
            }

            public System.Threading.WaitHandle AsyncWaitHandle
            {
                get { throw new NotImplementedException(); }
            }

            public bool CompletedSynchronously
            {
                get { throw new NotImplementedException(); }
            }

            public bool IsCompleted
            {
                get { throw new NotImplementedException(); }
            }
            #endregion

            #region IComparer
            public int Compare(object x, object y)
            {
                throw new NotImplementedException();
            }
            #endregion

            #region IConvertible
            public TypeCode GetTypeCode()
            {
                throw new NotImplementedException();
            }

            public bool ToBoolean(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public byte ToByte(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public char ToChar(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public DateTime ToDateTime(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public decimal ToDecimal(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public double ToDouble(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public short ToInt16(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public int ToInt32(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public long ToInt64(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public sbyte ToSByte(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public float ToSingle(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public string ToString(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public object ToType(Type conversionType, IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public ushort ToUInt16(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public uint ToUInt32(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public ulong ToUInt64(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }
            #endregion

            #region IEqualityComparer
            public new bool Equals(object x, object y)
            {
                throw new NotImplementedException();
            }

            public int GetHashCode(object obj)
            {
                throw new NotImplementedException();
            }
            #endregion

            #region IDisposable
            public void Dispose()
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
