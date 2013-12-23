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
using System.Collections.Generic;
using System.Linq;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestCategory = NUnit.Framework.CategoryAttribute;
#endif  

namespace Telerik.JustMock.Tests
{
	[TestClass]
	public class BehaviorFixture
	{
		[TestMethod, TestCategory("Lite"), TestCategory("Behavior")]
		public void ShouldReturnDefaultValueOnLoose()
		{
			var foo = Mock.Create<IFoo>(Behavior.Loose);
			Assert.Equal(0, foo.GetInt32());
			Assert.Null(foo.GetObject());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior")]
		public void ShouldReturnMockOnRecursiveLoose()
		{
			var foo = Mock.Create<IFoo>(Behavior.RecursiveLoose);
			var foo2 = foo.IFoo.IFoo;
			Assert.NotNull(foo2);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior")]
		public void ShouldThrowForNoSetupOnStrict()
		{
			var foo = Mock.Create<IFoo>(Behavior.Strict);
			Assert.Throws<MockException>(() => foo.GetGuid());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior")]
		public void ShouldAssertMessageForNoSetupOnString()
		{
			var foo = Mock.Create<IFoo>(Behavior.Strict);
			
			string expected = "All calls on Telerik.JustMock.Tests.BehaviorFixture+IFoo should be arranged first.";
			string actual =   Assert.Throws<MockException>(() => foo.GetGuid()).Message;

			Assert.Equal(expected, actual);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior")]
		public void ShouldReturnDefaultGuidOnLoose()
		{
			var foo = Mock.Create<IFoo>();
			Assert.Equal(default(Guid), foo.GetGuid());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior")]
		public void ShoudReturnEmptyArrayOnLoose()
		{
			var foo = Mock.Create<IFoo>();
			// array should not be null:framework design guidelines.
			var array = foo.GetArray();
			Assert.NotNull(array);
			Assert.Equal(0, array.Length);
			Assert.Same(array, foo.GetArray());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior")]
		public void ShouldReturnEmptyEnumerableOnLoose()
		{
			var foo = Mock.Create<IFoo>();
			var e = foo.GetEnumerable();
			Assert.NotNull(e);
			Assert.Equal(e.Cast<string>().Count(), 0);
			Assert.Same(e, foo.GetEnumerable());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior")]
		public void SHouldReturnEmptyDictionaryOnLoose()
		{
			var foo = Mock.Create<IFoo>();

			var dict = foo.GetDictionary();

			Assert.NotNull(dict);
			Assert.Equal(dict.Count, 0);
			Assert.Same(dict, foo.GetDictionary());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior")]
		public void ShouldReturnEmptyListOnLoose()
		{
			var foo = Mock.Create<IFoo>();
			IList<string> list = foo.GetList();

			Assert.NotNull(list);
			Assert.Equal(list.Count, 0);
			Assert.Same(list, foo.GetList());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior")]
		public void ShouldAbleToInsertListItemOnLoose()
		{
			var foo = Mock.Create<IFoo>();
			IList<string> list = foo.GetList();

			list.Add("pong");

			Assert.Equal(list[0], "pong");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior")]
		public void ShouldReturnNullStringOnLoose()
		{
			var foo = Mock.Create<IFoo>(Behavior.Loose);
			Assert.Equal(foo.GetString(), null);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior")]
		public void ShouldReturnDefaultForAbstractOnLoose()
		{
			var foo = Mock.Create<Foo>();
			Assert.Equal(0, foo.GetInt());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior")]
		public void ShouldThrowForNoReturnOnStrict()
		{
			var foo = Mock.Create<IFoo>(Behavior.Strict);
			Mock.Arrange(() => foo.GetString());
			Assert.Throws<MockException>(() => foo.GetString());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior")]
		public void ShouldAssertSetWUnArrangedPropertyOnLoose()
		{
			var foo = Mock.Create<IFoo>();

			Mock.ArrangeSet(() => { foo.StrValue = string.Empty; }).Throws(new ArgumentException());

			foo.StrValue = "Should not Throw";

			Assert.Throws<ArgumentException>(() => foo.StrValue = string.Empty);
		}


#if !NUNIT
		// BCL issue - Reflection.Emit fails for multidimensional arrays until .NET4
		// with System.TypeLoadException : Signature of the body and declaration in a method implementation do not match.

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior")]
		public void ShouldReturnEmptyMultidimensionalArray()
		{
			var matrix = Mock.Create<IMatrix>();
			var array = matrix.GetMultidimensionalArray();
			Assert.NotNull(array);
			Assert.Equals(0, array.GetLength(0));
			Assert.Equals(0, array.GetLength(1));
			Assert.Same(array, matrix.GetMultidimensionalArray());
		}

		public interface IMatrix
		{
			int[,,] GetMultidimensionalArray();
		}
#endif

		public interface IFoo
		{
			Guid GetGuid();
			int GetInt32();
			object GetObject();
			string[] GetArray();
			IList<string> GetList();
			IEnumerable<string> GetEnumerable();
			IDictionary<string, string> GetDictionary();
			string GetString();
			string StrValue { get; set; }
			IFoo IFoo { get; set; }
		}

		public abstract class Foo
		{
			public abstract int GetInt();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior"), TestCategory("CallOriginal")]
		public void ShouldNotCallOriginalImplementationIfReturnValueArranged()
		{
			var mock = Mock.Create<DontCallOriginal>(Behavior.CallOriginal);
			Mock.Arrange(() => mock.CallMe()).Returns(1);           
			Assert.Equal(1, mock.CallMe());
		}

		public class DontCallOriginal
		{
			public virtual int CallMe()
			{
				throw new InvalidOperationException();
			}
		}
	}
}
