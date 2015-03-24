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
using System.Text;

#if NUNIT
using NUnit.Framework;
using TestCategory = NUnit.Framework.CategoryAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using AssertionException = NUnit.Framework.AssertionException;
#elif PORTABLE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AssertionException = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AssertFailedException;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#endif

namespace Telerik.JustMock.Tests.Regression
{
	[TestClass]
	public partial class MatchersFixture
	{
		
		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldAssertAnyMatcherWithFloat()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Execute(Arg.AnyFloat)).MustBeCalled();

			foo.Execute(default(float));

			Mock.Assert(foo);
		}
		
		
		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldAssertAnyMatcherWithDouble()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Execute(Arg.AnyDouble)).MustBeCalled();

			foo.Execute(default(double));

			Mock.Assert(foo);
		}
		
		
		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldAssertAnyMatcherWithDecimal()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Execute(Arg.AnyDecimal)).MustBeCalled();

			foo.Execute(default(decimal));

			Mock.Assert(foo);
		}
		
		
		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldAssertAnyMatcherWithLong()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Execute(Arg.AnyLong)).MustBeCalled();

			foo.Execute(default(long));

			Mock.Assert(foo);
		}
		
		
		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldAssertAnyMatcherWithChar()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Execute(Arg.AnyChar)).MustBeCalled();

			foo.Execute(default(char));

			Mock.Assert(foo);
		}
		
		
		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldAssertAnyMatcherWithString()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Execute(Arg.AnyString)).MustBeCalled();

			foo.Execute(default(string));

			Mock.Assert(foo);
		}
		
		
		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldAssertAnyMatcherWithObject()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Execute(Arg.AnyObject)).MustBeCalled();

			foo.Execute(default(object));

			Mock.Assert(foo);
		}
		
		
		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldAssertAnyMatcherWithShort()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Execute(Arg.AnyShort)).MustBeCalled();

			foo.Execute(default(short));

			Mock.Assert(foo);
		}
		
		
		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldAssertAnyMatcherWithBool()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Execute(Arg.AnyBool)).MustBeCalled();

			foo.Execute(default(bool));

			Mock.Assert(foo);
		}
		
		
		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldAssertAnyMatcherWithGuid()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Execute(Arg.AnyGuid)).MustBeCalled();

			foo.Execute(default(Guid));

			Mock.Assert(foo);
		}
		
		
		public interface IFoo
		{
			void Execute(float arg1);
			void Execute(double arg1);
			void Execute(decimal arg1);
			void Execute(long arg1);
			void Execute(char arg1);
			void Execute(string arg1);
			void Execute(object arg1);
			void Execute(short arg1);
			void Execute(bool arg1);
			void Execute(Guid arg1);
		}
	}
}
