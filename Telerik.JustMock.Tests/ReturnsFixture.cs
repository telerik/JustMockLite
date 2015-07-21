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

#if NUNIT
using NUnit.Framework;
using TestCategory = NUnit.Framework.CategoryAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using AssertionException = NUnit.Framework.AssertionException;
#elif VSTEST_PORTABLE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AssertionException = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AssertFailedException;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustMock.Core;
using Telerik.JustMock.Helpers;

namespace Telerik.JustMock.Tests
{
	[TestClass]
	public class ReturnsFixture
	{
		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldReturnAValue()
		{
			var target = Mock.Create<ICloneable>();

			var clone = new object();

			Mock.Arrange(() => target.Clone()).Returns(clone);

			Assert.Equal(clone, target.Clone());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldHaveDifferntReturnForDifferentValues()
		{
			var iFoo = Mock.Create<IFoo>();

			Mock.Arrange(() => iFoo.Execute("x")).Returns("y");
			Mock.Arrange(() => iFoo.Execute("y")).Returns("z");

			Assert.Equal(iFoo.Execute("x"), "y");
			Assert.Equal(iFoo.Execute("y"), "z");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldReturnDifferntValuesForDifferntArgurments()
		{
			var iFoo = Mock.Create<IFoo>();

			Mock.Arrange(() => iFoo.Execute("x")).Returns("y");
			Mock.Arrange(() => iFoo.Execute("x", "y")).Returns("z");


			Assert.Equal(iFoo.Execute("x"), "y");
			Assert.Equal(iFoo.Execute("x", "y"), "z");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldReturnNullIfSepcified()
		{
			var iFoo = Mock.Create<IFoo>();

			Mock.Arrange(() => iFoo.Execute("x")).Returns((string)null);

			Assert.Equal(iFoo.Execute("x"), null);
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ReturnsValueFromVariable()
		{
			var value = "ack";
			var iFoo = Mock.Create<IFoo>();

			Mock.Arrange(() => iFoo.Execute(null)).Returns(value);

			Assert.Equal(iFoo.Execute(null), value);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldReturnNullValueIfNullFunc()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Execute("ping")).Returns((Func<string>)null).MustBeCalled();

			Assert.Null(foo.Execute("ping"));

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldPassSameArgumentToReturns()
		{
			var iFoo = Mock.Create<IFoo>();
			Mock.Arrange(() => iFoo.Execute("x")).Returns((string s) => s);
			Assert.Equal(iFoo.Execute("x"), "x");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldNotRequiredToDeclareArgumentWhenReturnTypeIsSame()
		{
			var iFoo = Mock.Create<IFoo>();
			Mock.Arrange(() => iFoo.Execute(Arg.AnyString)).Returns(s => s).MustBeCalled();
			iFoo.Execute(string.Empty);
			Mock.Assert(iFoo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldThrowWhenReturnTypeIsNotSameAsArgumentWhenPassFromReturns()
		{
			var iFoo = Mock.Create<IFoo>();
			Mock.Arrange(() => iFoo.Echo(Arg.AnyInt)).Returns(s => s);
			Assert.Throws<Exception>(() => iFoo.Echo(10));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldPassOneArgumentToReturns()
		{
			var iFoo = Mock.Create<IFoo>();
			Mock.Arrange(() => iFoo.Execute(Arg.IsAny<string>(), Arg.IsAny<string>()))
				.Returns((string s1, string s2) => s1 + s2);
			Assert.Equal(iFoo.Execute("blah", ".."), "blah..");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldPassTwoArgumentsToReturns()
		{
			var iFoo = Mock.Create<IFoo>();
			Mock.Arrange(() => iFoo.Execute("x", "y")).Returns((string s1, string s2) => s1 + s2);
			Assert.Equal(iFoo.Execute("x", "y"), "xy");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldPassThreeArgumentsToReturns()
		{
			var iFoo = Mock.Create<IFoo>();
			Mock.Arrange(() => iFoo.Execute("x", "y", "z"))
				.Returns((string s1, string s2, string s3) => s1 + s2 + s3);
			Assert.Equal(iFoo.Execute("x", "y", "z"), "xyz");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldPassFourArgumentsToReturns()
		{
			var iFoo = Mock.Create<IFoo>();
			Mock.Arrange(() => iFoo.Execute("x", "y", "z", "a"))
				.Returns((string s1, string s2, string s3, string s4) => s1 + s2 + s3 + s4);
			Assert.Equal(iFoo.Execute("x", "y", "z", "a"), "xyza");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldReturnNullForArrayWhenSpecified()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Children).Returns((IFoo[])null);
			Assert.Null(foo.Children);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldIgnoreReturnValueFromLambdaPassedToDoInstead()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Value).DoInstead(new Func<int>(() => 123));
			Assert.Equal(0, foo.Value);
		}


#if !SILVERLIGHT && !LITE_EDITION
		public interface ICollectionSource
		{
			IEnumerable<T> GetCollection<T>();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldReturnCollection()
		{
			var mock = Mock.Create<ICollectionSource>();
			Mock.Arrange(() => mock.GetCollection<int>()).ReturnsCollection(new[] { 1, 2, 3 }.AsQueryable());
			Assert.Equal(6, mock.GetCollection<int>().Sum());
		}
#endif

#if SILVERLIGHT || PORTABLE
		
		public interface ICloneable
		{
			object Clone();
		}

#endif


		public interface IFoo
		{
			string Execute(string arg1, string arg2);
			string Execute(string arg1, string arg2, string arg3);
			string Execute(string arg1, string arg2, string arg3, string arg4);
			string Execute(string arg1);

			string Echo(int arg1);

			void Submit(int arg1);
			void Submit(int arg1, int arg2);
			void Submit(int arg1, int arg2, int arg3);
			void Submit(int arg1, int arg2, int arg3, int arg4);

			int Value { get; set; }

			IFoo[] Children { get; set; }
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldPassInstanceAndArgumentsToReturnsDelegate()
		{
			var mock = Mock.Create<IFoo>();
			Mock.Arrange(() => mock.Echo(Arg.AnyInt))
				.Returns((IFoo @this, int arg) => @this.Value.ToString());
			Mock.Arrange(() => mock.Value).Returns(123);

			Assert.Equal("123", mock.Echo(14));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldReturnManyValuesAndThenThrow()
		{
			var mock = Mock.Create<IFoo>();
			Mock.Arrange(() => mock.Value).ReturnsMany(new[] { 1, 2, 3 }, AfterLastValue.ThrowAssertionFailed);

			Assert.Equal(1, mock.Value);
			Assert.Equal(2, mock.Value);
			Assert.Equal(3, mock.Value);
			Assert.Throws<AssertionException>(() => { var x = mock.Value; });
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldReturnManyValuesAndThenCycle()
		{
			var mock = Mock.Create<IFoo>();
			Mock.Arrange(() => mock.Value).ReturnsMany(new[] { 1, 2, 3 }, AfterLastValue.StartFromBeginning);

			Assert.Equal(1, mock.Value);
			Assert.Equal(2, mock.Value);
			Assert.Equal(3, mock.Value);
			Assert.Equal(1, mock.Value);
			Assert.Equal(2, mock.Value);
			Assert.Equal(3, mock.Value);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldReturnManyValuesAndThenKeepReturningLast()
		{
			var mock = Mock.Create<IFoo>();
			Mock.Arrange(() => mock.Value).ReturnsMany(1, 2, 3);

			Assert.Equal(1, mock.Value);
			Assert.Equal(2, mock.Value);
			Assert.Equal(3, mock.Value);
			Assert.Equal(3, mock.Value);
			Assert.Equal(3, mock.Value);
			Assert.Equal(3, mock.Value);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldReturnManyValuesWhichAreModifiedAfterArrangement()
		{
			var mock = Mock.Create<IFoo>();
			var list = new List<int> { 1, 2, 3 };

			Mock.Arrange(() => mock.Value).ReturnsMany(list, AfterLastValue.KeepReturningLastValue);

			Assert.Equal(1, mock.Value);
			Assert.Equal(2, mock.Value);
			Assert.Equal(3, mock.Value);

			list.RemoveAt(list.Count - 1);
			Assert.Equal(2, mock.Value);
			Assert.Equal(2, mock.Value);
		}

		public interface IRefReturns
		{
			object Do(ref int a);
		}

		public delegate object DoDelegate(ref int a);

		[TestMethod, TestCategory("Lite"), TestCategory("Returns"), TestCategory("OutRef")]
		public void ShouldReturnUsingCustomDelegate()
		{
			var mock = Mock.Create<IRefReturns>();
			Mock.Arrange(() => mock.Do(ref Arg.Ref(Arg.AnyInt).Value)).Returns(new DoDelegate((ref int a) => a++));

			int value = 5;
			object result = mock.Do(ref value);

			Assert.Equal(6, value);
			Assert.Equal(5, result);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldInterpretNullReturnsDelegateAsNullReturnsValue()
		{
			var test = Mock.Create<Entity>(Behavior.CallOriginal);

			Mock.Arrange(() => test.AsReference()).Returns<object>(null);
			Assert.Null(test.AsReference());

			Mock.Arrange(() => test.AsNullable()).Returns<int?>(null);
			Assert.Null(test.AsNullable());

			Assert.Throws<MockException>(() => Mock.Arrange(() => test.AsInt()).Returns<object>(null));

			Mock.Arrange(() => test.Throw()).DoInstead(null);
			test.Throw();
			//didn't throw
		}

		public class Entity
		{
			public virtual int AsInt()
			{
				return 1;
			}

			public virtual object AsReference()
			{
				return new object();
			}

			public virtual int? AsNullable()
			{
				return 1;
			}

			public virtual void Throw()
			{
				throw new Exception();
			}
		}
	}
}
