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
using System.Collections.Generic;
using System.Linq;

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


namespace Telerik.JustMock.Tests.Coverage
{
	[TestClass]
	public class DoInsteadFixture
	{
			
		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDoInsteadWithFiveArgsForExpected()
		{
			int expected = 0;

			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Submit(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.DoInstead((int arg1, int arg2, int arg3, int arg4, int arg5) => { expected = arg1 + arg2 + arg3 + arg4 + arg5; });

			foo.Submit(1, 1, 1, 1, 1);

			Assert.Equal(5, expected);
		}
		
			
		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDoInsteadWithSixArgsForExpected()
		{
			int expected = 0;

			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Submit(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.DoInstead((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6) => { expected = arg1 + arg2 + arg3 + arg4 + arg5 + arg6; });

			foo.Submit(1, 1, 1, 1, 1, 1);

			Assert.Equal(6, expected);
		}
		
			
		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDoInsteadWithSevenArgsForExpected()
		{
			int expected = 0;

			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Submit(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.DoInstead((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7) => { expected = arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7; });

			foo.Submit(1, 1, 1, 1, 1, 1, 1);

			Assert.Equal(7, expected);
		}
		
			
		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDoInsteadWithEightArgsForExpected()
		{
			int expected = 0;

			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Submit(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.DoInstead((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8) => { expected = arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8; });

			foo.Submit(1, 1, 1, 1, 1, 1, 1, 1);

			Assert.Equal(8, expected);
		}
		
			
		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDoInsteadWithNineArgsForExpected()
		{
			int expected = 0;

			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Submit(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.DoInstead((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9) => { expected = arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9; });

			foo.Submit(1, 1, 1, 1, 1, 1, 1, 1, 1);

			Assert.Equal(9, expected);
		}
		
			
		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDoInsteadWithTenArgsForExpected()
		{
			int expected = 0;

			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Submit(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.DoInstead((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10) => { expected = arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10; });

			foo.Submit(1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

			Assert.Equal(10, expected);
		}
		
			
		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDoInsteadWithElevenArgsForExpected()
		{
			int expected = 0;

			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Submit(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.DoInstead((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11) => { expected = arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11; });

			foo.Submit(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

			Assert.Equal(11, expected);
		}
		
			
		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDoInsteadWithTwelveArgsForExpected()
		{
			int expected = 0;

			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Submit(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.DoInstead((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12) => { expected = arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12; });

			foo.Submit(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

			Assert.Equal(12, expected);
		}
		
			
		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDoInsteadWithThirteenArgsForExpected()
		{
			int expected = 0;

			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Submit(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.DoInstead((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12, int arg13) => { expected = arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13; });

			foo.Submit(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

			Assert.Equal(13, expected);
		}
		
			
		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDoInsteadWithFourteenArgsForExpected()
		{
			int expected = 0;

			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Submit(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.DoInstead((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12, int arg13, int arg14) => { expected = arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13 + arg14; });

			foo.Submit(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

			Assert.Equal(14, expected);
		}
		
			
		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDoInsteadWithFifteenArgsForExpected()
		{
			int expected = 0;

			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Submit(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.DoInstead((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12, int arg13, int arg14, int arg15) => { expected = arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13 + arg14 + arg15; });

			foo.Submit(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

			Assert.Equal(15, expected);
		}
		
			
		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDoInsteadWithSixteenArgsForExpected()
		{
			int expected = 0;

			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Submit(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.DoInstead((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12, int arg13, int arg14, int arg15, int arg16) => { expected = arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13 + arg14 + arg15 + arg16; });

			foo.Submit(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

			Assert.Equal(16, expected);
		}
		
				
		public interface IFoo
		{
			void Submit(int arg1, int arg2, int arg3, int arg4, int arg5);
			void Submit(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6);
			void Submit(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7);
			void Submit(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8);
			void Submit(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9);
			void Submit(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10);
			void Submit(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11);
			void Submit(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12);
			void Submit(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12, int arg13);
			void Submit(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12, int arg13, int arg14);
			void Submit(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12, int arg13, int arg14, int arg15);
			void Submit(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12, int arg13, int arg14, int arg15, int arg16);
			
		}
	}
}
