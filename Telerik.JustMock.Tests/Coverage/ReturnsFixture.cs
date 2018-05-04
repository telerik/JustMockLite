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
using AssertionException = Xunit.Sdk.AssertException;
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
	public class ReturnsFixture
	{
	
				
		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldPassFiveArgsToReturns()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.Returns((int arg1, int arg2, int arg3, int arg4, int arg5) => arg1 + arg2 + arg3 + arg4 + arg5);

			int result = foo.Echo(1, 1, 1, 1, 1);

			Assert.Equal(5, result);
		}
	
		
				
		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldPassSixArgsToReturns()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6);

			int result = foo.Echo(1, 1, 1, 1, 1, 1);

			Assert.Equal(6, result);
		}
	
		
				
		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldPassSevenArgsToReturns()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7);

			int result = foo.Echo(1, 1, 1, 1, 1, 1, 1);

			Assert.Equal(7, result);
		}
	
		
				
		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldPassEightArgsToReturns()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8);

			int result = foo.Echo(1, 1, 1, 1, 1, 1, 1, 1);

			Assert.Equal(8, result);
		}
	
		
				
		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldPassNineArgsToReturns()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9);

			int result = foo.Echo(1, 1, 1, 1, 1, 1, 1, 1, 1);

			Assert.Equal(9, result);
		}
	
		
				
		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldPassTenArgsToReturns()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10);

			int result = foo.Echo(1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

			Assert.Equal(10, result);
		}
	
		
				
		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldPassElevenArgsToReturns()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11);

			int result = foo.Echo(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

			Assert.Equal(11, result);
		}
	
		
				
		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldPassTwelveArgsToReturns()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12);

			int result = foo.Echo(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

			Assert.Equal(12, result);
		}
	
		
				
		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldPassThirteenArgsToReturns()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12, int arg13) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13);

			int result = foo.Echo(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

			Assert.Equal(13, result);
		}
	
		
				
		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldPassFourteenArgsToReturns()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12, int arg13, int arg14) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13 + arg14);

			int result = foo.Echo(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

			Assert.Equal(14, result);
		}
	
		
				
		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldPassFifteenArgsToReturns()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12, int arg13, int arg14, int arg15) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13 + arg14 + arg15);

			int result = foo.Echo(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

			Assert.Equal(15, result);
		}
	
		
				
		[TestMethod, TestCategory("Lite"), TestCategory("Returns")]
		public void ShouldPassSixteenArgsToReturns()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt, Arg.AnyInt))
				.Returns((int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12, int arg13, int arg14, int arg15, int arg16) => arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7 + arg8 + arg9 + arg10 + arg11 + arg12 + arg13 + arg14 + arg15 + arg16);

			int result = foo.Echo(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

			Assert.Equal(16, result);
		}
	
		
				
		public interface IFoo
		{
			int Echo(int arg1, int arg2, int arg3, int arg4, int arg5);
			int Echo(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6);
			int Echo(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7);
			int Echo(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8);
			int Echo(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9);
			int Echo(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10);
			int Echo(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11);
			int Echo(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12);
			int Echo(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12, int arg13);
			int Echo(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12, int arg13, int arg14);
			int Echo(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12, int arg13, int arg14, int arg15);
			int Echo(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8, int arg9, int arg10, int arg11, int arg12, int arg13, int arg14, int arg15, int arg16);
			
		}
	}
}