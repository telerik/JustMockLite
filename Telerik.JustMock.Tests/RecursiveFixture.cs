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
using Telerik.JustMock.AutoMock;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#else
using TestCategory = NUnit.Framework.CategoryAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using AssertionException = NUnit.Framework.AssertionException;
#endif

namespace Telerik.JustMock.Tests
{
	[TestClass]
	public class RecursiveFixture
	{
		[TestMethod, TestCategory("Lite"), TestCategory("Recursive")]
		public void ShouldAssertNestedPropertySetups()
		{
			var foo = Mock.Create<IFoo>();
			
			Mock.Arrange(() => foo.Bar.Value).Returns(10);

			Assert.Equal(10, foo.Bar.Value);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Recursive")]
		public void ShouldAssertNestedProperyCallsAsEqual()
		{
			var foo = Mock.Create<IFoo>();

			var b1 = foo.Bar;
			var b2 = foo.Bar;

			Assert.Same(b1, b2);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Recursive")]
		public void ShouldAssertNestedSetupWithSimilarMethods()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Bar.Do("x")).Returns("xit");
			Mock.Arrange(() => foo.Bar1.Baz.Do("y")).Returns("yit");

			Assert.Equal(foo.Bar.Do("x"), "xit");
			Assert.Equal(foo.Bar1.Baz.Do("y"), "yit");
	   }

		[TestMethod, TestCategory("Lite"), TestCategory("Recursive")]
		public void ShouldAssertNestedSetupForSimilarRootAndSimilarMethods()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Bar.Do("x")).Returns("xit");
			Mock.Arrange(() => foo.Bar.Baz.Do("y")).Returns("yit");

			Assert.Equal(foo.Bar.Do("x"), "xit");
			Assert.Equal(foo.Bar.Baz.Do("y"), "yit");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Recursive")]
		public void ShouldNotAutoInstantiateIfNotArranged()
		{
			var foo = Mock.Create<IFoo>(Behavior.Loose);
			Assert.Equal(foo.Bar, null);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Recursive")]
		public void ShouldAssertNestedPropertySet()
		{
			var foo = Mock.Create<IFoo>(Behavior.Strict);

			Mock.ArrangeSet(() => { foo.Bar.Value = 5; }).DoNothing();

			Assert.Throws<MockException>(() => foo.Bar.Value = 10);

			foo.Bar.Value = 5;

			Assert.NotNull(foo.Bar);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Recursive")]
		public void ShouldAssertNestedVerifables()
		{
			var foo = Mock.Create<IFoo>();

			string ping = "ping";

			Mock.Arrange(() => foo.Do(ping)).Returns("ack");
			Mock.Arrange(() => foo.Bar.Do(ping)).Returns("ack2");

			Assert.Equal(foo.Do(ping), "ack");

			var bar = foo.Bar;

			Assert.Throws<AssertionException>(() => Mock.Assert(() => foo.Bar.Do(ping)));

			Assert.Equal(foo.Bar.Do(ping), "ack2");

			Mock.Assert(() => foo.Bar.Do(ping));
		}

#if !SILVERLIGHT

		[TestMethod, TestCategory("Lite"), TestCategory("Recursive")]
		public void ShouldNotAutoCreateNestedInstanceWhenSetExplictly()
		{
			var foo = Mock.Create<Foo>();

			foo.Bar = Mock.Create(() => new Bar(10));

			Mock.Arrange(() => foo.Bar.Echo()).CallOriginal();

			Assert.Equal(10, foo.Bar.Echo());    
		}

#endif

		[TestMethod, TestCategory("Lite"), TestCategory("Recursive")]
		public void ShouldMockIEnumerableImplementer()
		{
			var regionManager = Mock.Create<IRegionManager>();

			Mock.Arrange(() => regionManager.Regions["SomeRegion"]).Returns(5);
			Assert.Equal(5, regionManager.Regions["SomeRegion"]);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Recursive")]
		public void ShouldMockIDictionaryImplementer()
		{
			var regionManager = Mock.Create<IRegionManager>();

			Mock.Arrange(() => regionManager.RegionsByName["SomeRegion"]).Returns(5);
			Assert.Equal(5, regionManager.RegionsByName["SomeRegion"]);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Recursive")]
		public void ShouldRaiseEventsFromMockIEnumerable()
		{
			var regionManager = Mock.Create<IRegionManager>();
			Mock.Arrange(() => regionManager.Regions[""]).Returns(new object()); // auto-arrange Regions with mock collection

			bool ienumerableEventRaised = false;
			regionManager.Regions.CollectionChanged += (o, e) => ienumerableEventRaised = true;
			Mock.Raise(() => regionManager.Regions.CollectionChanged += null, EventArgs.Empty);
			Assert.True(ienumerableEventRaised);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Recursive")]
		public void ShouldRaiseEventsFromMockIDictionary()
		{
			var regionManager = Mock.Create<IRegionManager>();
			Mock.Arrange(() => regionManager.RegionsByName[""]).Returns(new object()); // auto-arrange RegionsByName with mock collection

			bool idictionaryEventRaised = false;
			regionManager.Regions.CollectionChanged += (o, e) => idictionaryEventRaised = true;
			Mock.Raise(() => regionManager.Regions.CollectionChanged += null, EventArgs.Empty);
			Assert.True(idictionaryEventRaised);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Recursive")]
		public void ShouldBeAbleToEnumerateMockEnumerable()
		{
			var mock = Mock.Create<IDataLocator>();
			Assert.Equal(0, mock.RecentEvents.Count());
		}

		private IMatrix Matrix { get; set; }

		[TestMethod, TestCategory("Lite"), TestCategory("Recursive")]
		public void ShouldNotAutoArrangeIfPropertyInThis()
		{
			var mockedMatrix = Mock.Create<IMatrix>();
			this.Matrix = mockedMatrix;

			var mockedArray = new object[0];
			Mock.Arrange(() => Matrix.Raw).Returns(mockedArray);

			Assert.Equal(mockedMatrix, this.Matrix);
			Assert.Equal(mockedArray, this.Matrix.Raw);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Recursive")]
		public void ShouldReturnNullOnLoose()
		{
			var foo = Mock.Create<IFoo>(Behavior.Loose);

			Assert.Null(foo.Bar);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Recursive")]
		public void ShouldAutoMockInArrangeOnLoose()
		{
			var foo = Mock.Create<IFoo>(Behavior.Loose);

			Mock.Arrange(() => foo.Bar.Baz);

			Assert.NotNull(foo.Bar);
		}

		public interface IRegionManager
		{
			IRegionCollection Regions { get; }
			IRegionLookup RegionsByName { get; }
		}

		public interface IRegionCollection : IEnumerable<object>
		{
			object this[string regionName] { get; }
			event EventHandler CollectionChanged;
		}

		public interface IRegionLookup : IDictionary<string, object>
		{
			object this[string regionName] { get; }
			event EventHandler CollectionChanged;
		}

		public interface IMatrix
		{
			Array Raw { get; }
		}

		public interface IDataLocator
		{
			IDataFeed RecentEvents { get; }
		}

		public interface IDataFeed : IEnumerable<object>
		{ }

		public class Bar
		{
			int arg1;

			public Bar(int arg1)
			{
				this.arg1 = arg1;
			}

			public virtual int Echo()
			{
				return arg1;
			}
		}

		public class Foo
		{
			public virtual Bar Bar { get; set; }
		}

		public interface IFoo
		{
			IBar Bar { get; set; }
			IBar Bar1 { get; set; }
			IBar this[int index] { get; set; }
			string Do(string command);
		}

		public interface IBar
		{
			int Value { get; set; }
			string Do(string command);
			IBaz Baz { get; set; }
			IBaz GetBaz(string value);
		}

		public interface IBaz
		{
			int Value { get; set; }
			string Do(string command);
			void Do();
		}

		public abstract class ValidateMember
		{
			public readonly object session;

			public ValidateMember(object session)
			{
				this.session = session;
			}

			public abstract bool With(string model);
		}

		public interface IServiceProvider
		{
			T Query<T>();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Recursive")]
		public void ShouldRecursivelyArrangeGenericMethod()
		{
			var service = Mock.Create<IServiceProvider>();
			Mock.Arrange(() => service.Query<ValidateMember>().With("me")).Returns(true);

			var actual = service.Query<ValidateMember>().With("me");
			Assert.Equal(true, actual);
		}

		public interface IBenefits
		{
		}

		public interface IOuter
		{
			IEnumerableWithBenefits Bar { get; }
			IDictionaryWithBenefits Dict { get; }
		}

		public interface IEnumerableWithBenefits : IEnumerable<Object>
		{
			IBenefits GetBaz();
		}

		public interface IDictionaryWithBenefits : IDictionary<string, int>
		{
			IBenefits GetBaz();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Recursive")]
		public void ShouldMockRecursivelyCustomMembersOnIEnumerable()
		{
			var foo = Mock.Create<IOuter>(Behavior.RecursiveLoose);
			Assert.NotNull(foo.Bar.GetBaz());
			Assert.NotNull(foo.Dict.GetBaz());
		}
	}

	[TestClass]
	public class RecursiveMockRepositoryInheritance
	{
		public interface IDataItem
		{
			int Id { get; }
		}

		public interface IDataProcessor
		{
			IDataItem Item { get; }
		}

		private IDataProcessor mock;

		[TestInitialize]
		public void BeforeEach()
		{
			mock = Mock.Create<IDataProcessor>();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("MockingContext"), TestCategory("Recursive")]
		public void ShouldSetUseContextualRepositoryForRecursiveMock()
		{
			Mock.Arrange(() => mock.Item.Id).Returns(5);
			Assert.Equal(5, mock.Item.Id);
		}
	}
}
