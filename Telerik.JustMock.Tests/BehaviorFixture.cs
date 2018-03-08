/*
 JustMock Lite
 Copyright © 2010-2015 Telerik AD

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
using System.Threading.Tasks;
using Telerik.JustMock.Core;



#region JustMock Test Attributes
#if NUNIT
using NUnit.Framework;
using TestCategory = NUnit.Framework.CategoryAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using AssertionException = NUnit.Framework.AssertionException;
#elif XUNIT
using Xunit;
using Telerik.JustMock.XUnit.Test.Attributes;
using TestCategory = Telerik.JustMock.XUnit.Test.Attributes.XUnitCategoryAttribute;
using TestClass = Telerik.JustMock.XUnit.Test.Attributes.EmptyTestClassAttribute;
using TestMethod = Xunit.FactAttribute;
using TestInitialize = Telerik.JustMock.XUnit.Test.Attributes.EmptyTestInitializeAttribute;
using TestCleanup = Telerik.JustMock.XUnit.Test.Attributes.EmptyTestCleanupAttribute;
using AssertionException = Xunit.Sdk.AssertException;
#elif VSTEST_PORTABLE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AssertionException = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AssertFailedException;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#endif
#endregion



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

			string expected = "Called unarranged member 'System.Guid GetGuid()' on strict mock of type 'Telerik.JustMock.Tests.BehaviorFixture+IFoo'";
			string actual = Assert.Throws<MockException>(() => foo.GetGuid()).Message;

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
			Assert.Throws<StrictMockException>(() => foo.GetString());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior")]
		public void ShouldAssertSetWUnArrangedPropertyOnLoose()
		{
			var foo = Mock.Create<IFoo>();

			Mock.ArrangeSet(() => { foo.StrValue = string.Empty; }).Throws(new ArgumentException());

			foo.StrValue = "Should not Throw";

			Assert.Throws<ArgumentException>(() => foo.StrValue = string.Empty);
		}

		public interface ICallBool
		{
			void CallBool(System.Linq.Expressions.Expression<Func<ICallBool, bool>> arg);
		}

		[TestMethod]
		public void ShouldCompareConstantExpressions()
		{
			var person = Mock.Create<ICallBool>(Behavior.Strict);
			Mock.Arrange(() => person.CallBool(p => true));
			person.CallBool(p => true); // doesn't throw
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

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior")]
		public void ShouldCreateRecursiveMockInsideConstructor()
		{
			var mock = Mock.Create<CtorMock>(Constructor.NotMocked, Behavior.RecursiveLoose);
			Assert.NotNull(mock.TheFoo);
		}

		public abstract class CtorMock
		{
			protected abstract IFoo Foo { get; }

			public CtorMock()
			{
				TheFoo = Foo;
			}

			public IFoo TheFoo;
		}

		public interface IMatrix
		{
			int[, ,] GetMultidimensionalArray();
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

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior"), TestCategory("Strict")]
		public void ShouldRequireReturnValueInStrictMockArrangements()
		{
			var mock = Mock.Create<IFoo>(Behavior.Strict);
			Mock.Arrange(() => mock.GetInt32()).OccursOnce();
			var strictEx = Assert.Throws<StrictMockException>(() => mock.GetInt32());
			var expected = "Member 'Int32 GetInt32()' on strict mock of type 'Telerik.JustMock.Tests.BehaviorFixture+IFoo' has a non-void return value but no return value given in arrangement.";
			Assert.Equal(strictEx.Message, expected);
		}

		public class DontCallOriginal
		{
			public virtual int CallMe()
			{
				throw new InvalidOperationException();
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior"), TestCategory("Strict")]
		public void ShouldAssertStrictMock()
		{
			var mock = Mock.Create<IFoo>(Behavior.Strict);
			Mock.Assert(mock);

			try
			{
				mock.GetGuid();
			}
			catch (Exception) { }

			var message = Assert.Throws<AssertionException>(() => Mock.Assert(mock)).Message;
			Assert.Equal("Called unarranged member 'System.Guid GetGuid()' on strict mock of type 'Telerik.JustMock.Tests.BehaviorFixture+IFoo'", message.Trim());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior"), TestCategory("Strict")]
		public void ShouldAssertStrictDelegateMock()
		{
			var mock = Mock.Create<Action>(Behavior.Strict);
			Mock.Assert(mock);

			try
			{
				mock();
			}
			catch (Exception) { }

			var message = Assert.Throws<AssertionException>(() => Mock.Assert(mock)).Message;
#if !COREFX || SILVERLIGHT
			Assert.Equal("Called unarranged member 'Void Invoke()' on strict mock of type 'Castle.Proxies.Delegates.System_Action'", message.Trim());
#else
			Assert.Equal("Called unarranged member 'Void Invoke()' on strict mock of type 'Telerik.JustMock.DelegateBackends.System.Action'", message.Trim());
#endif
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior"), TestCategory("Task")]
		public async Task ShouldAutoArrangeResultOfAsyncMethodOnRecursiveLooseMock()
		{
			var mock = Mock.Create<IAsyncTest>();
			var result = await mock.GetAsync();
			Assert.NotNull(result);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior"), TestCategory("Task")]
		public async Task ShouldAutoArrangeResultOfAsyncMethodOnLooseMock()
		{
			var mock = Mock.Create<IAsyncTest>(Behavior.Loose);
			var result = await mock.GetAsync();
			Assert.Null(result);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior"), TestCategory("Task")]
		public async Task ShouldArrangeTaskResultOfAsyncMethod()
		{
			var mock = Mock.Create<IAsyncTest>();
			Mock.Arrange(() => mock.GetIntAsync()).TaskResult(5);
			var result = await mock.GetIntAsync();
			Assert.Equal(5, result);
		}

		public interface IAsyncTest
		{
			Task<IDisposable> GetAsync();
			Task<int> GetIntAsync();
		}
	}
}
