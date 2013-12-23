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
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#else
using NUnit.Framework;
using TestCategory = NUnit.Framework.CategoryAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using AssertionException = NUnit.Framework.AssertionException;
#endif

namespace Telerik.JustMock.Tests
{
	[TestClass]
	public class InOrderFixture
	{
		[TestMethod, TestCategory("Lite"), TestCategory("InOrder")]
		public void ShouldAssertCallMadeInOrderTheySetup()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Save()).InOrder();
			Mock.Arrange(() => foo.Update()).InOrder();
			Mock.Arrange(() => foo.CommitChanges()).InOrder();

			foo.Save();
			foo.Update();
			foo.CommitChanges();

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("InOrder")]
		public void ShouldThrowCallMadeInDifferentOrderFromSetup()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Save()).InOrder();
			Mock.Arrange(() => foo.Update()).InOrder();
			Mock.Arrange(() => foo.CommitChanges()).InOrder();

			Assert.Throws<AssertionException>(() => foo.Update());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("InOrder")]
		public void ShouldThrowNotEveryMethodCalledInSetupOrder()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Save()).InOrder();
			Mock.Arrange(() => foo.Update()).InOrder();
			Mock.Arrange(() => foo.CommitChanges()).InOrder();

			foo.Save();
			Assert.Throws<AssertionException>(() => foo.CommitChanges());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("InOrder")]
		public void ShouldThrowNotEveryMethodCalledInSetupOrderOnAssert()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Save()).InOrder();
			Mock.Arrange(() => foo.Update()).InOrder();
			Mock.Arrange(() => foo.CommitChanges()).InOrder();

			foo.Save();
			foo.Update();
			Assert.Throws<AssertionException>(() => Mock.Assert(foo));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("InOrder")]
		public void ShouldAsserInOrderExecutionForPropertySet()
		{
			var foo = Mock.Create<IFoo>();

			Mock.ArrangeSet(() => foo.Value = 10).InOrder();
			Mock.ArrangeSet(() => foo.Value = 11).InOrder();

			foo.Value = 10;
			foo.Value = 11;
	  
			Mock.Assert(foo); 
		}

		[TestMethod, TestCategory("Lite"), TestCategory("InOrder")]
		public void ShouldAssertOrderFlowWhenCombinedWithOcurrences()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Save()).InOrder().OccursOnce();
			Mock.Arrange(() => foo.Update()).InOrder().OccursOnce();

			Assert.Throws<AssertionException>(() => foo.Update());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("InOrder")]
		public void ShouldAssertOrderFlowWhenCombinedWithOcurrencesOnAssert()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Save()).InOrder().OccursOnce();
			Mock.Arrange(() => foo.Update()).InOrder().OccursOnce();

			foo.Save();

			Assert.Throws<AssertionException>(() => Mock.Assert(foo));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("InOrder")]
		public void ShouldAssertInOrderForDifferentInstancesInTestMethodScope()
		{
			string userName = "Bob";
			string password = "Password";
			int userID = 5;
			var cart = new List<string> { "Foo", "Bar" };
		  
			var userServiceMock = Mock.Create<IUserValidationService>();
			var shoppingCartServiceMock = Mock.Create<IShoppingCartService>();

			Mock.Arrange(() => userServiceMock.ValidateUser(userName, password)).Returns(userID).InOrder().OccursOnce();
			Mock.Arrange(() => shoppingCartServiceMock.LoadCart(userID)).Returns(cart).InOrder().Occurs(1);

			Assert.Throws<AssertionException>(() => shoppingCartServiceMock.LoadCart(userID));
			Assert.Throws<AssertionException>(() => userServiceMock.ValidateUser(userName, password));
		}

		public interface IUserValidationService
		{
			int ValidateUser(string userName, string password);
		}

		public interface IShoppingCartService
		{
			IList<string> LoadCart(int userID);
		}

		public interface IFoo
		{
			int Value { get; set; }
			void Update();
			void Save();
			void CommitChanges();
		}
	}
}
