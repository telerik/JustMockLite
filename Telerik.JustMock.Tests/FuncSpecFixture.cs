/*
 JustMock Lite
 Copyright © 2010-2015 Progress Software Corporation

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
using System.Collections.Generic;
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
using AssertionException = Telerik.JustMock.XUnit.AssertFailedException;
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
	public class FuncSpecFixture
	{
		public enum FuncResult
		{
			A, B
		}

		public interface IFuncSpecced
		{
			bool Bool { get; set; }
			int Prop { get; }
			string GetString();
			int Complex(int a, string b);

			IFuncSpecced InnerElement { get; }
			IEnumerable<IFuncSpecced> Inner { get; }

			FuncResult Go(FuncResult a);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("FuncSpec")]
		public void ShouldMakeSpecForProperty()
		{
			var mock = Mock.CreateLike<IFuncSpecced>(me => me.Prop == 5);
			Assert.Equal(5, mock.Prop);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("FuncSpec")]
		public void ShouldMakeSpecForMethod()
		{
			var mock = Mock.CreateLike<IFuncSpecced>(me => me.GetString() == "hooray");
			Assert.Equal("hooray", mock.GetString());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("FuncSpec")]
		public void ShouldMakeSpecForMethodWithSpecificParameters()
		{
			var mock = Mock.CreateLike<IFuncSpecced>(me => me.Complex(5, "a") == 100);
			Assert.Equal(100, mock.Complex(5, "a"));
			Assert.Equal(0, mock.Complex(5, "b"));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("FuncSpec")]
		public void ShouldMakeSpecForMethodWithAnyParameterMatchers()
		{
			var mock = Mock.CreateLike<IFuncSpecced>(me => me.Complex(Arg.AnyInt, Arg.AnyString) == 100);
			Assert.Equal(100, mock.Complex(5, "a"));
			Assert.Equal(100, mock.Complex(500, "b"));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("FuncSpec")]
		public void ShouldMakeSpecForMethodWithComplexParameterMatchers()
		{
			var mock = Mock.CreateLike<IFuncSpecced>(me => me.Complex(Arg.IsInRange(100, 200, RangeKind.Inclusive), Arg.Matches<string>(s => s.Length >= 2)) == 100);
			Assert.Equal(100, mock.Complex(100, "xx"));
			Assert.Equal(100, mock.Complex(200, "xxxxx"));
			Assert.Equal(0, mock.Complex(150, "x"));
			Assert.Equal(0, mock.Complex(13, "xxxxx"));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("FuncSpec")]
		public void ShouldMakeComplexSpec()
		{
			var mock = Mock.CreateLike<IFuncSpecced>(me =>
				me.Prop == 5 &&
				me.GetString() == "leString"
				&& me.Complex(Arg.AnyInt, Arg.AnyString) == -1);

			Assert.Equal(5, mock.Prop);
			Assert.Equal("leString", mock.GetString());
			Assert.Equal(-1, mock.Complex(5, "none"));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("FuncSpec")]
		public void ShouldThrowForInvalidSpecFormat()
		{
			Assert.Throws<MockException>(() => Mock.CreateLike<IFuncSpecced>(me => me.Prop > 5));
			Assert.Throws<MockException>(() => Mock.CreateLike<IFuncSpecced>(me => me.Prop == 5 || me.GetString() == "abc"));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("FuncSpec")]
		public void ShouldArrangeBooleanImplicitly()
		{
			var mock = Mock.CreateLike<IFuncSpecced>(me => me.Bool);
			Assert.True(mock.Bool);
			var mock2 = Mock.CreateLike<IFuncSpecced>(me => me.Prop == 5 && me.Bool);
			Assert.True(mock2.Bool);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("FuncSpec")]
		public void ShouldArrangeNegatedBooleanImplicitly()
		{
			var mock = Mock.CreateLike<IFuncSpecced>(me => !me.Bool);
			mock.Bool = true;
			Assert.False(mock.Bool);
			var mock2 = Mock.CreateLike<IFuncSpecced>(me => me.Prop == 5 && !me.Bool);
			mock2.Bool = true;
			Assert.False(mock2.Bool);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("FuncSpec")]
		public void ShouldMakeSpecWithInnerMocks()
		{
			var mock = Mock.CreateLike<IFuncSpecced>(me =>
				me.InnerElement == Mock.CreateLike<IFuncSpecced>(i => i.Prop == -1) &&
				me.Inner == new List<IFuncSpecced>
					{
						Mock.CreateLike<IFuncSpecced>(i => i.Prop == 1 && i.GetString() == "inner 1"),
						Mock.CreateLike<IFuncSpecced>(i => i.Prop == 2 && i.GetString() == "inner 2")
					});

			Assert.Equal(-1, mock.InnerElement.Prop);

			var list = mock.Inner as List<IFuncSpecced>;
			Assert.Equal(2, list.Count);
			Assert.Equal(1, list[0].Prop);
			Assert.Equal(2, list[1].Prop);
			Assert.Equal("inner 1", list[0].GetString());
			Assert.Equal("inner 2", list[1].GetString());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("FuncSpec")]
		public void ShouldMakeSpecUsingAllParametersAndClosureVariable()
		{
			int a = 100;
			var mock = Mock.CreateLike<IFuncSpecced>(me => me.Complex(Arg.AnyInt, Arg.AnyString) == (a + Param<int>._1 + int.Parse(Param<string>._2)));

			var actual = mock.Complex(5, "15");
			Assert.Equal(120, actual);

			a = -100;
			actual = mock.Complex(5, "15");
			Assert.Equal(-80, actual);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("FuncSpec")]
		public void ShouldMakeSpecNotAllParametersUsed()
		{
			var mock = Mock.CreateLike<IFuncSpecced>(me => me.Complex(Arg.AnyInt, Arg.AnyString) == int.Parse(Param<string>._2));
			var actual = mock.Complex(5, "15");
			Assert.Equal(15, actual);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("FuncSpec")]
		public void ShouldMakeSpecUsingAllParametersAndClosureVariableImplicitlyTyped()
		{
			int a = 100;
			var mock = Mock.CreateLike<IFuncSpecced>(me => me.Complex(Arg.AnyInt, Arg.AnyString) == (a + Param._1 + int.Parse(Param._2)));

			var actual = mock.Complex(5, "15");
			Assert.Equal(120, actual);

			a = -100;
			actual = mock.Complex(5, "15");
			Assert.Equal(-80, actual);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("FuncSpec")]
		public void ShouldMakeSpecUsingImplicitlyTypedParametersConvertedToObject()
		{
			var mock = Mock.CreateLike<IEqualityComparer>(me => me.Equals(Arg.AnyObject, Arg.AnyObject) == Equals(Param._1, Param._2));
			Assert.True(mock.Equals(10, 10));
			Assert.False(mock.Equals(15, 20));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("FuncSpec")]
		public void ShouldMakeSpecNotAllParametersUsedImplicitlyTyped()
		{
			var mock = Mock.CreateLike<IFuncSpecced>(me => me.Complex(Arg.AnyInt, Arg.AnyString) == int.Parse(Param._2));
			var actual = mock.Complex(5, "15");
			Assert.Equal(15, actual);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("FuncSpec"), TestCategory("Recursive")]
		public void ShouldMakeSpecForRecursiveArrangement()
		{
			var mock = Mock.CreateLike<IFuncSpecced>(me => me.InnerElement.InnerElement.Prop == 5);
			Assert.Equal(0, mock.Prop);
			Assert.Equal(0, mock.InnerElement.Prop);
			Assert.Equal(5, mock.InnerElement.InnerElement.Prop);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("FuncSpec"), TestCategory("Recursive")]
		public void ShouldEvaluateConstantExpressionsEarly()
		{
			int a = 5;
			var mock = Mock.CreateLike<IFuncSpecced>(me => me.Prop == a);
			Assert.Equal(5, mock.Prop);
			a = 15;
			Assert.Equal(5, mock.Prop);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("FuncSpec"), TestCategory("Recursive")]
		public void ShouldEvaluateConstantMockExpressionsEarly()
		{
			int a = 5;
			var mock = Mock.CreateLike<IFuncSpecced>(me => me.InnerElement == Mock.Create<IFuncSpecced>());
			Assert.Same(mock.InnerElement, mock.InnerElement);
		}

		public interface IByteProperty
		{
			byte Prop { get; }
		}

		[TestMethod, TestCategory("Lite"), TestCategory("FuncSpec")]
		public void ShouldArrangeByteReturnValue()
		{
			var instance = Mock.CreateLike<IByteProperty>(o => o.Prop == 1);
			Assert.Equal(1, instance.Prop);
		}

		public abstract class ToStringTester
		{
			public abstract int Execute(IDisposable disp);

			public virtual string DisplayName { get; set; }

			public override string ToString()
			{
				throw new Exception();
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("FuncSpec")]
		public void ShouldRespectToStringArrangementWhenConvertingExpressionToString()
		{
			var param = Mock.Create<IDisposable>();
			var mock = Mock.CreateLike<ToStringTester>(x =>
				x.ToString() == "safe"
				&& x.DisplayName == "foo"
				&& x.Execute(param) == 5);

			Assert.Equal("safe", mock.ToString());
			Assert.Equal("foo", mock.DisplayName);
			Assert.Equal(5, mock.Execute(param));

			GC.KeepAlive(DebugView.FullTrace);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("FuncSpec")]
		public void ShouldNotThrowWhenConvertingExpressionToString()
		{
			var param = Mock.Create<IDisposable>();
			var mock = Mock.CreateLike<ToStringTester>(x =>
				x.DisplayName == "foo"
				&& x.Execute(param) == 5);

			Assert.Equal("foo", mock.DisplayName);
			Assert.Equal(5, mock.Execute(param));

			GC.KeepAlive(DebugView.FullTrace);
		}
	}
}
