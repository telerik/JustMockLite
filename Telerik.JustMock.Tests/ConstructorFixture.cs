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
	public class ConstructorFixture
	{
		[TestMethod, TestCategory("Lite"), TestCategory("Constructor")]
		public void ShouldCallBaseCtorWhenNotMocked()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				Mock.Create<Foo>(Constructor.NotMocked);
			});
		}

#if !(COREFX && LITE_EDITION)
		[TestMethod, TestCategory("Lite"), TestCategory("Constructor")]
#if SILVERLIGHT
		[Ignore, Description("SL instance constructor mocking not implemented")]
#endif
		public void ShouldSkipBaseConstructorWhenMocked()
		{
			Assert.NotNull(Mock.Create<Foo>());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Constructor")]
#if SILVERLIGHT
		[Ignore, Description("SL instance constructor mocking not implemented")]
#endif
		public void ShouldSkipBaseConstructorOfAbstractClassWhenMocked()
		{
			Assert.NotNull(Mock.Create<AbstractFoo>());
		}
#endif

#if !LITE_EDITION
		[TestMethod, TestCategory("Elevated"), TestCategory("Constructor")]
		public void ShouldCreateMockForFrameWorkClassWithInternalCtor()
		{
			var downloadDateCompleted = Mock.Create<System.IO.IsolatedStorage.IsolatedStorageFile>();
			Assert.NotNull(downloadDateCompleted != null);
		}

		[TestMethod, TestCategory("Elevated"), TestCategory("Constructor")]
		public void ShouldFutureMockConstructorWithArg()
		{
			long? arg = null;
			Mock.Arrange(() => new CtorLongArg(Arg.AnyLong)).DoInstead<long>(x => arg = x);

			new CtorLongArg(100);
			Assert.True(arg.Value == 100);
		}

#endif

		public class Foo
		{
			public Foo()
			{
				throw new ArgumentException("Failed");
			}
		}

		public abstract class AbstractFoo
		{
			public AbstractFoo()
			{
				throw new ArgumentException("Failed");
			}

			public abstract int Id { get; set; }
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Constructor")]
		public void ShouldCallConstructorRequiringPrimitiveArgumentConversions()
		{
			Mock.Create<CtorLongArg>(Behavior.CallOriginal, 0);
		}

		public class CtorLongArg
		{
			public CtorLongArg(long l) { }
		}

		public class Base
		{
			public int i;

			public Base(int i)
			{
				this.i = i;
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Constructor")]
		public void ShouldUseAutoselectedConstructorMockingBehaviorWithFluentConfig()
		{
			var proxy = Mock.Create<Base>(fluentConfig =>
			{
				fluentConfig.Implements<IDisposable>();
			});

			Assert.Equal(0, proxy.i);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Constructor")]
		public void ShouldSpecifyConstructorArgumentsWithFluentConfig()
		{
			var proxy = Mock.Create<Base>(fluentConfig =>
				fluentConfig.Implements<IDisposable>()
					.CallConstructor(new object[] { 5 })
			);

			Assert.Equal(5, proxy.i);
		}

		public class CallsCtor
		{
			public bool ok;

			public CallsCtor()
			{
				ok = true;
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Constructor")]
		public void ShouldCallDefaultConstructorWhenExplicitlyGivenNoArguments()
		{
			var mock = Mock.Create<CallsCtor>(new object[0]);
			Assert.True(mock.ok);
		}
	}
}
