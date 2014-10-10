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
using System.Linq.Expressions;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestCategory = NUnit.Framework.CategoryAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using AssertFailedException = NUnit.Framework.AssertionException;
#endif

namespace Telerik.JustMock.Tests
{
	[TestClass]
	public partial class MatchersFixture
	{
		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldMatchAnyParameterValue()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Echo(Arg.IsAny<string>())).Returns("pong");
			Mock.Arrange(() => foo.Echo(Arg.IsAny<int>())).Returns(1);

			Assert.Equal(foo.Echo("ping"), "pong");
			Assert.Equal(foo.Echo("Any"), "pong");
			Assert.Equal(foo.Echo(10), 1);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldMatchPredicates()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Echo(Arg.Matches<int>(x => x > 0 && x < 5))).Returns(1);
			Mock.Arrange(() => foo.Echo(Arg.Matches<int>(x => x >= 5 && x < 10))).Returns(2);
			Mock.Arrange(() => foo.Echo(Arg.Matches<int>(x => x > 10))).Returns(3);

			Assert.Equal(1, foo.Echo(3));
			Assert.Equal(2, foo.Echo(5));
			Assert.Equal(3, foo.Echo(12));
			Assert.Equal(2, foo.Echo(7));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldMatchValueInRange()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Echo(Arg.IsInRange(1, 5, RangeKind.Inclusive))).Returns(1);
			Mock.Arrange(() => foo.Echo(Arg.IsInRange(6, 10, RangeKind.Exclusive))).Returns(2);

			Assert.Equal(foo.Echo(1), 1);
			Assert.Equal(foo.Echo(2), 1);
			Assert.Equal(foo.Echo(5), 1);

			// default value.
			Assert.Equal(foo.Echo(6), 0);

			// second one.
			Assert.Equal(foo.Echo(7), 2);
			Assert.Equal(foo.Echo(9), 2);
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldAssertPreDefinedAnyMatcherWithInt()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Echo(Arg.AnyInt)).MustBeCalled();

			foo.Echo(2);

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldMatchArgumetnForNullOrEmpty()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(Arg.NullOrEmpty)).Occurs(2);

			foo.Echo(string.Empty);
			foo.Echo(null);

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldMatchNullableArgument()
		{
			var foo = Mock.Create<Foo>();

			Mock.Arrange(() => foo.Echo(Arg.IsAny<int?>())).Returns(10);

			int ret = foo.Echo(4);
			Assert.Equal(10, ret);

			Assert.Equal(10, foo.Echo(null));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldSelectCorrectSetupInCaseOfSpecialization()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Echo(Arg.AnyInt)).Returns(10);
			Mock.Arrange(() => foo.Echo(Arg.Matches<int>(x => x > 10)))
				.Throws(new ArgumentException());

			foo.Echo(1);

			Assert.Throws<ArgumentException>(() => foo.Echo(11));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldAssertArgIsAnyForDelegates()
		{
			var foo = Mock.Create<IFoo>();

			bool called = false;

			Mock.Arrange(() => foo.Submit<string>(string.Empty, Arg.IsAny<Func<string, string>>()))
				.DoInstead(() => called = true);

			foo.Submit<string>(string.Empty, p => string.Empty);

			Assert.True(called);
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldAssertNewArgumentWhenArgIsAnySpecified()
		{
			var foo = Mock.Create<Foo>();

			Mock.Arrange(() => foo.ExeuteObject(Arg.IsAny<Foo>(), Arg.IsAny<Dummy>()));

			foo.ExeuteObject(null, new Dummy());

			Mock.AssertAll(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldMatchParamsArrayWithArgIsAnyForExpressionType()
		{
			var foo = Mock.Create<Foo>();

			string expected = "KKGKGKGHGHJG";
			var entity = new Entity { Prop2 = expected };

			Mock.Arrange(() => foo.GetByID(42, Arg.IsAny<Expression<Func<Entity, object>>>(), Arg.IsAny<Expression<Func<Entity, object>>>())).Returns(entity);

			//Act
			string result = foo.GetByID(42, x => x.Prop1, x => x.Prop2).Prop2;

			//Assert
			Assert.Equal(expected, result);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldMatchExactInstanceBasedOnFilter()
		{
			string expected = "expected";
			int expectedNumberOfTimes = 0;

			var foo = Mock.Create<IFoo>();
			var argumentOne = Mock.Create<IArgument>();
			var argumentTwo = Mock.Create<IArgument>();

			Mock.Arrange(() => argumentOne.Name).Returns(expected);
			Mock.Arrange(() => foo.TakeArgument(Arg.IsAny<IArgument>())).DoInstead((IArgument argument) =>
			{
				if (argumentOne == argument) { expectedNumberOfTimes++; }
			});

			foo.TakeArgument(argumentOne);
			foo.TakeArgument(argumentTwo);

			Mock.Assert(() => foo.TakeArgument(Arg.Matches<IArgument>(x => x.Name == expected)), Occurs.Exactly(expectedNumberOfTimes));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldMatchNullInPredicate()
		{
			var mock = Mock.Create<IFoo>();
			Mock.Arrange(() => mock.Echo(Arg.Matches<string>(s => s == null))).Returns("null");
			Assert.Equal("null", mock.Echo(null));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldApplyIgnoreInstanceToAllMockInstances()
		{
			var mock = Mock.Create<IFoo>();
			Mock.Arrange(() => mock.Echo(5)).IgnoreInstance().Returns(5);

			var differentMock = Mock.Create<IFoo>();
			Assert.Equal(5, differentMock.Echo(5));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldInferIgnoreInstanceFromNewExpression()
		{
			Mock.Arrange(() => new Foo().Echo(5)).Returns(5);

			var differentMock = Mock.Create<Foo>();
			Assert.Equal(5, differentMock.Echo(5));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldInferIgnoreInstanceFromNullCastToType()
		{
			Mock.Arrange(() => ((Foo)null).Echo(5)).Returns(5);

			var differentMock = Mock.Create<Foo>();
			Assert.Equal(5, differentMock.Echo(5));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldInferIgnoreInstanceFromNullTryCastToType()
		{
			Mock.Arrange(() => (null as Foo).Echo(5)).Returns(5);

			var differentMock = Mock.Create<Foo>();
			Assert.Equal(5, differentMock.Echo(5));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldInferIgnoreInstanceFromTargetPatternContainingCasts()
		{
			Mock.Arrange(() => (new Echoer() as IEchoer).Echo(5)).Returns(5);
			var mock = Mock.Create<IEchoer>();
			Assert.Equal(5, mock.Echo(5));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldMatchBoxedStructWithAny()
		{
			var mock = Mock.Create<IEchoer>();
			Mock.Arrange(() => mock.Echo(Arg.IsAny<DateTime>())).OccursOnce();
			mock.Echo(DateTime.Now);
			Mock.Assert(mock);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldNotMatchBoxedStructWithNull()
		{
			var mock = Mock.Create<IEchoer>();
			Mock.Arrange(() => mock.Echo(Arg.IsAny<DateTime>())).Throws<AssertFailedException>("Expected");
			mock.Echo(null);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldMatchDerivedTypeWithAny()
		{
			var mock = Mock.Create<IEchoer>();
			Mock.Arrange(() => mock.Echo(Arg.IsAny<IEchoer>())).Occurs(2);
			mock.Echo(mock);
			mock.Echo(null);
			Mock.Assert(mock);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldMatchRangeIntersection()
		{
			var mock = Mock.Create<IEchoer>();

			Mock.Arrange(() => mock.Echo(Arg.IsInRange(10, 20, RangeKind.Inclusive))).DoNothing().OccursNever();
			Mock.Arrange(() => mock.Echo(Arg.IsInRange(100, 200, RangeKind.Inclusive))).DoNothing().OccursOnce();

			Mock.Assert(() => mock.Echo(Arg.IsInRange(10, 50, RangeKind.Inclusive)));
			Assert.Throws<AssertFailedException>(() => Mock.Assert(() => mock.Echo(Arg.IsInRange(10, 200, RangeKind.Inclusive))));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldCompareBuiltinCollectionArgumentsElementwise()
		{
			string expected = "bar";
			string argument = "foo";

			var target = Mock.Create<IParams>();

			Mock.Arrange(() => target.ExecuteArray(new string[] { argument, "baz" })).Returns(expected);
			string ret = target.ExecuteArray(new string[] { argument, "baz" });
			Assert.Equal(expected, ret);

			Mock.Arrange(() => target.ExecuteArray(new List<string> { argument, "baz" })).Returns(expected);
			ret = target.ExecuteArray(new List<string> { argument, "baz" });
			Assert.Equal(expected, ret);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldMatchUserDefinedColletionArgumentsByReference()
		{
			var target = Mock.Create<IParams>();
			var s1 = new StringVector();
			var s2 = new StringVector();
			Mock.Arrange(() => target.ExecuteArray(s1)).Returns("1");
			Mock.Arrange(() => target.ExecuteArray(s2)).Returns("2");
			Assert.Equal("1", target.ExecuteArray(s1));
			Assert.Equal("2", target.ExecuteArray(s2));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldNotMatchUserDefinedColletionArgumentsWithBuiltinCollectionElementwise()
		{
			var target = Mock.Create<IParams>();
			var s1 = new StringVector();
			Mock.Arrange(() => target.ExecuteArray(s1)).Returns("1");
			Assert.Equal("", target.ExecuteArray(new string[0]));
		}

		public class StringVector : ICollection<string>
		{
			#region ICollection<string>
			public void Add(string item)
			{
				throw new InvalidOperationException();
			}

			public void Clear()
			{
				throw new InvalidOperationException();
			}

			public bool Contains(string item)
			{
				throw new InvalidOperationException();
			}

			public void CopyTo(string[] array, int arrayIndex)
			{
				throw new InvalidOperationException();
			}

			public int Count
			{
				get { throw new InvalidOperationException(); }
			}

			public bool IsReadOnly
			{
				get { throw new InvalidOperationException(); }
			}

			public bool Remove(string item)
			{
				throw new InvalidOperationException();
			}

			public IEnumerator<string> GetEnumerator()
			{
				throw new InvalidOperationException();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				throw new InvalidOperationException();
			}
			#endregion
		}

		public interface IParams
		{
			string ExecuteArray(IEnumerable<string> arg);
		}

		public class Entity
		{
			public string Prop1 { get; set; }
			public string Prop2 { get; set; }
		}

		public interface IEchoer
		{
			object Echo(object a);
		}

		public class Echoer : IEchoer
		{
			public object Echo(object a)
			{
				throw new NotImplementedException();
			}
		}

		public class Foo
		{
			public virtual int Echo(int? intValue)
			{
				return intValue.Value;
			}

			public virtual void ExeuteObject(Foo foo, Dummy dummy)
			{

			}

			public virtual Entity GetByID(int id, params Expression<Func<Entity, object>>[] args)
			{
				return null;
			}

			public Foo GetSelf()
			{
				throw new NotImplementedException();
			}

			public Foo Self
			{
				get { throw new NotImplementedException(); }
			}
		}

		public class Dummy
		{

		}

		public interface IArgument
		{
			string Name { get; }
		}

		public interface IFoo
		{
			string Echo(string argument);
			int Echo(int intArg);
			int Add(int[] args);
			int CheckMe(IFoo foo);
			void Submit<T>(string param1, Func<T, string> func);
			void Submit<T, T1>(Func<T, T1, string> func);
			void TakeArgument(IArgument argument);
		}

		public class FakeMe
		{
			public virtual string Params(string firstArg, params string[] otherArgs)
			{
				return "I'm real!";
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers"), TestCategory("Params")]
		public void ShouldWrapStringNullOrEmptyMatcherInParamsMatcher()
		{
			var mock = Mock.Create<FakeMe>();

			const string iMFake = "I'm Fake";

			string only = "only";

			Mock.Arrange(() => mock.Params(only, Arg.NullOrEmpty)).Returns(iMFake);

			var actual = mock.Params(only, string.Empty);

			Assert.Equal(iMFake, actual);
		}

		public interface IRequest
		{
			string Method { get; set; }
			string GetResponse();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldConsiderWhenClause()
		{
			var mock = Mock.Create<IRequest>();
			Mock.Arrange(() => mock.GetResponse()).When(() => mock.Method == "GET").OccursOnce();
			Mock.Arrange(() => mock.GetResponse()).When(() => mock.Method == "POST").OccursOnce();

			Assert.Throws<AssertFailedException>(() => Mock.Assert(mock));

			mock.Method = "GET";
			mock.GetResponse();
			mock.Method = "POST";
			mock.GetResponse();

			Mock.Assert(mock);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldDisregardMethodArgumentsInWhenClause()
		{
			var mock = Mock.Create<IFoo>(Behavior.Loose);
			bool execute = false;
			Mock.Arrange(() => mock.Echo(Arg.AnyString)).When(() => execute).Returns("aaa");

			Assert.Null(mock.Echo("xxx"));
			execute = true;
			Assert.Equal("aaa", mock.Echo("xxx"));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers"), TestCategory("Assertion")]
		public void ShouldAssertUsingCustomMatcherOnConcreteInstance()
		{
			var mock = Mock.Create<IComparer<int>>();
			mock.Compare(1, 5);
			mock.Compare(2, 2);
			mock.Compare(1, 1);
			mock.Compare(3, 1);

			var mock2 = Mock.Create<IComparer<int>>();
			mock2.Compare(5, 5);

			Mock.Assert(() => mock.Compare(0, 0),
				Args.Matching((int a, int b) => a == b),
				Occurs.Exactly(2));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers"), TestCategory("Assertion")]
		public void ShouldAssertUsingCustomMatcherOnAnyInstance()
		{
			var mock = Mock.Create<IComparer<int>>();
			mock.Compare(1, 5);
			mock.Compare(2, 2);
			mock.Compare(1, 1);
			mock.Compare(3, 1);

			var mock2 = Mock.Create<IComparer<int>>();
			mock2.Compare(5, 5);

			Mock.Assert(() => mock.Compare(0, 0),
				Args.Matching((IComparer<int> _this, int a, int b) => a == b && _this != null),
				Occurs.Exactly(3));
		}
	}
}
