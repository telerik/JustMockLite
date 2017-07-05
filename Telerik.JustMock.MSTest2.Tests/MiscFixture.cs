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
using System.IO;
using System.Net;
using Telerik.JustMock.Core;

#if !COREFX
using Telerik.JustMock.DemoLib;
#endif

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

namespace Telerik.JustMock.MSTest2.Tests
{
	[TestClass]
	public class MiscFixture
	{
		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldReturnForDateTimeUsedAsArg()
		{
			var items = new List<string>() { "Foo", "Bar" };

			var myClass = Mock.Create<IMyClass>(Behavior.Strict);

			Mock.Arrange(() => myClass.GetValuesSince(Arg.IsAny<DateTime>())).Returns(items);

			var actual = myClass.GetValuesSince(DateTime.Now).ToList();

			Assert.Equal(items.Count, actual.Count);
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldAssertInPtrAsReturnValue()
		{
			var fooPtr = Mock.Create<IFooPtr>(Behavior.Strict);

			IntPtr ret = new IntPtr(3);

			Mock.Arrange(() => fooPtr.Get("a")).Returns(ret);

			IntPtr actual = fooPtr.Get("a");

			Assert.Equal(ret, actual);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldAssertArgumentPassedByImplictConversation()
		{
			const string s = "XYZ";

			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Get(s)).Returns(10);

			Assert.Throws<AssertionException>(() => Mock.Assert(() => foo.Get(s)));

			int ret = foo.Get(s);

			Mock.Assert(() => foo.Get(s));

			Assert.Equal(10, ret);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldAssertArgumentPassedByExplictConversation()
		{
			const string s = "XYZ";

			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Get(SomeClass<string>.From(s))).Returns(10);

			Assert.Throws<AssertionException>(() => Mock.Assert(() => foo.Get(SomeClass<string>.From(s))));

			int ret = foo.Get(SomeClass<string>.From(s));

			Mock.Assert(() => foo.Get(SomeClass<string>.From(s)));

			Assert.Equal(10, ret);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldCreateMockWithGenericConstraints()
		{
			var target = Mock.Create<ISomething<int>>();
			Assert.NotNull(target);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShoudlAssertCallWithGenericConstraint()
		{
			var target = Mock.Create<ISomething<int>>();
			// the following line should not throw any exception.
			target.DoSomething<int>();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldAssertInterfaceForGenericConstaint()
		{
			var target = Mock.Create<ISomething<IDummy>>();
			// the following line should not throw any exception.
			target.DoSomething<IDummy>();
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldAssertImplicitInterface()
		{
			var bar = Mock.Create<IBar>();
			var barSUT = new Bar(bar);

			barSUT.Do(new Foo());

			Mock.Assert(() => bar.Do(Arg.IsAny<Foo>()));
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldBeAddRemoveConcreteMockItemsFromCollection()
		{
			var foo = Mock.Create<Foo>();

			IList<Foo> list = new List<Foo>();

			list.Add(foo);

			if (list.Contains(foo))
			{
				list.Remove(foo);
			}
			Assert.True(list.Count == 0);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldBeAbleToCallBaseForGenericMethod()
		{
			var facade = Mock.Create<TestFacade>();

			Mock.Arrange(() => facade.Done<ContentItem>()).CallOriginal();

			Assert.Throws<ArgumentException>(() => facade.Done<ContentItem>());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldBeAbleToCallOrignalForOutArgMethod()
		{
			var foo = Mock.Create<FooOut>();

			int expected = 0;

			Mock.Arrange(() => foo.EchoOut(out expected)).CallOriginal();

			foo.EchoOut(out expected);

			Assert.Equal(10, expected);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldAssertCallOrignalForMethodWithGenericParameter()
		{
			var foo = Mock.Create<FooGeneric>();
			Mock.Arrange(() => foo.Echo<int>(10)).CallOriginal();
			foo.Echo<int>(10);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldPickCorrectGenericVarientInCaseOfCallOriginal()
		{
			var foo = Mock.Create<FooGeneric>();
			Mock.Arrange(() => foo.Echo<int, int>(10)).CallOriginal();
			Assert.Throws<ArgumentException>(() => foo.Echo<int, int>(10));
		}

		[TestMethod, TestCategory("Lite")]
		public void ShouldAssertInvocationFromInsideAMockedEvent()
		{
			var @interface = Mock.Create<IInterface>();

			Mock.Arrange(() => @interface.TheFunc()).Returns(true);

			var target = new EventContainer(@interface);

			Mock.Raise(() => @interface.TheEvent += null, EventArgs.Empty);

			Assert.True(target.Result);
		}

		[TestMethod, TestCategory("Lite")]
		public void ShouldAssertRaiseEventAfterAMethodCallFromDifferentMock()
		{
			var @interface = Mock.Create<IInterface>();
			var @extended = Mock.Create<IInterfaceExtended>();
			var target = new EventContainer(@interface);

			Mock.Arrange(() => @interface.TheFunc()).Returns(true);
			Mock.Raise(() => @interface.TheEvent += null, EventArgs.Empty);

			@extended.TheFunc();

			Mock.Raise(() => @interface.TheEvent += null, EventArgs.Empty);

			Assert.True(target.NumberOfTimesCalled == 2);
		}

		[TestMethod, TestCategory("Lite")]
		public void ShouldBeToSubscribeEventForStrictMock()
		{
			new EventContainer(Mock.Create<IInterface>(Behavior.Strict));
		}

		[TestMethod, TestCategory("Lite")]
		public void ShouldNotThrowExceptionForDecimalTypeThatHasMultipleImplicitMethods()
		{
			var foo = Mock.Create<TestBase>();
			decimal value = 1;
			Mock.Arrange(() => foo.SetValue(value)).MustBeCalled();

			foo.SetValue(value);

			Mock.Assert(foo);
		}

		public abstract class TestBase
		{
			public virtual decimal Value { get; set; }

			public virtual void SetValue(decimal newValue)
			{
				Value = newValue;
			}
		}

		public class EventContainer
		{
			public bool Result = false;

			private IInterface @interface = null;

			public int NumberOfTimesCalled { get; set; }

			public EventContainer(IInterface i)
			{
				this.@interface = i;
				this.@interface.TheEvent += new EventHandler(i_TheEvent);
			}

			void i_TheEvent(object sender, EventArgs e)
			{
				this.Result = this.@interface.TheFunc();
				this.NumberOfTimesCalled++;
			}
		}

		public interface IInterface
		{
			event EventHandler TheEvent;
			bool TheFunc();
		}

		public interface IInterfaceExtended
		{
			bool TheFunc();
		}

		public class FooOut
		{
			public virtual void EchoOut(out int argOut)
			{
				argOut = 10;
			}
		}

		public class FooGeneric
		{
			public virtual void Echo<TKey>(TKey key)
			{

			}
			public virtual void Echo<TKey, TKey2>(TKey key)
			{
				throw new ArgumentException();
			}
		}

		public class ContentItem : IFacade<ContentItem>
		{

		}

		public interface IFacade<TParaentFacade> { }

		public class TestFacade
		{
			public virtual TParentFacade Done<TParentFacade>() where TParentFacade : class, IFacade<TParentFacade>
			{
				throw new ArgumentException("My custom error");
			}
		}

#if !COREFX

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldNotImplementInternalVirtualMemberUsingProxyWhenNotVisible()
		{
			var context = Mock.Create<Telerik.JustMock.DemoLibSigned.DummyContext>();
			Assert.NotNull(context);
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldCreateMockForFrameWorkClassWithInternalCtor()
		{
			var downloadDateCompleted = Mock.Create<DownloadDataCompletedEventArgs>();
			Assert.NotNull(downloadDateCompleted != null);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldAssertStreamMocking()
		{
			var stream = Mock.Create<Stream>();

			Mock.Arrange(() => stream.Seek(0, SeekOrigin.Begin)).Returns(0L);

			var position = stream.Seek(0, SeekOrigin.Begin);

			Assert.Equal(0, position);

			Mock.Arrange(() => stream.Flush()).MustBeCalled();
			Mock.Arrange(() => stream.SetLength(100)).MustBeCalled();

			Assert.Throws<AssertionException>(() => Mock.Assert(stream));

			stream.Flush();
			stream.SetLength(100);

			Mock.Assert(stream);
		}
#endif

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldMockMultipleInterfaceOnASingleMock()
		{
			var foo = Mock.Create<IFooDispose>();
			var iDisposable = foo as IDisposable;

			bool called = false;

			Mock.Arrange(() => iDisposable.Dispose()).DoInstead(() => called = true);

			iDisposable.Dispose();

			Assert.True(called);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldMockClassWithInterfaceConstraints()
		{
			var container = Mock.Create<FakeContainer<Product>>();

			Mock.Arrange(() => container.Do<Product>()).MustBeCalled();

			container.Do<Product>();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldMockMethodCallWithObjectArgumentWithMatcher()
		{
			var container = Mock.Create<IContainer>();
			var called = false;

			Mock.Arrange(() => container.Resolve(Arg.IsAny<IDatabase>())).DoInstead(() => called = true);

			var database = Mock.Create<IDatabase>();

			container.Resolve(database);

			Assert.True(called);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldBeAbleToAssertNestedSetupDirectly()
		{
			var outer = Mock.Create<FooOuter>();
			var inner = Mock.Create<FooInter>();

			Mock.Arrange(() => outer.GetInnerClass()).Returns(inner);
			Mock.Arrange(() => inner.Value).Returns(10).MustBeCalled();

			Assert.Throws<AssertionException>(() => Mock.Assert(() => outer.GetInnerClass().Value));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldNotCreateNestedMockWhenReturningCallBackForGenericCall()
		{
			Product product1 = new Product();
			Product product2 = new Product();

			Queue<Product> products = new Queue<Product>();

			products.Enqueue(product1);
			products.Enqueue(product2);

			var context = Mock.Create<IDataContext>();

			Mock.Arrange(() => context.Get<Product>()).Returns(() => products.Dequeue());

			Assert.True(context.Get<Product>().Equals(product1));
			Assert.True(context.Get<Product>().Equals(product2));
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldReturnNullWhenSpecifiedByReturn()
		{
			var exmpleMock = Mock.Create<IExampleInterface>();

			Mock.Arrange(() => exmpleMock.GetMeAllFoos()).Returns((IList<IFoo>)null);

			Assert.Null(exmpleMock.GetMeAllFoos());
		}

#if !COREFX

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldMockInternalMemberFromBaseClass()
		{
			var id = Guid.NewGuid();

			var manager = Mock.Create<IContentManager>();
			var facade = Mock.Create<BlogFacade>(Behavior.CallOriginal);

			Mock.Arrange(() => facade.ContentManager).Returns(manager);

			Mock.Arrange(() => manager.GetItem(Arg.IsAny<Type>(), Arg.AnyGuid))
				.Returns(new Product()).MustBeCalled();

			facade.LoadItem(id);

			Mock.Assert(facade.ContentManager);
		}

#endif


		[TestMethod, TestCategory("Lite")]
		public void ShouldAssertSetupWithObjectArrayAsParams()
		{
			var foo = Mock.Create<Foo<Product>>();

			const int expected = 1;

			Mock.Arrange(() => foo.GetByKey(expected)).Returns(() => null).MustBeCalled();

			foo.GetByKey(expected);

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite")]
		public void ShouldNotInstantiatePropertyWhenSetExplicitly()
		{
			var foo = Mock.Create<NestedFoo>();
			var actual = new FooThatFails(string.Empty);

			Mock.Arrange(() => foo.FooThatFailsOnCtor).Returns(actual);

			Assert.Equal(foo.FooThatFailsOnCtor, actual);
		}


#if !COREFX

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldBeAbleToMockInternalProtectedVirtualMember()
		{
			var visitor = Mock.Create<ExpressionNodeVisitor>(Behavior.CallOriginal);
			var node = Mock.Create<ExpressionNode>(Behavior.CallOriginal);

			visitor.VisitExtension(node);

			Mock.Assert(() => node.VisitChildren(visitor), Occurs.Once());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldBeAbleToCreateMockWithInternalCtor()
		{
			var expected = "hello";

			var foo = Mock.Create<FooInternal>(x =>
			{
				x.CallConstructor(() => new FooInternal("hello"));
				x.SetBehavior(Behavior.CallOriginal);
			});

			Assert.Equal(foo.Name, expected);
		}

#endif

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldExecuteEqualsDuringAssertWithMockArgument()
		{
			var foo = Mock.Create<FooAbstract>();
			var fooWork = Mock.Create<FooWork>();

			fooWork.DoWork(foo);

			Mock.Assert(() => fooWork.DoWork(foo));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldAssertMultipleOccurrencesSeparatelyForAssertAll()
		{
			IFileReader fileReader = Mock.Create<IFileReader>(Behavior.Strict);

			Mock.Arrange(() => fileReader.FileExists(@"C:\Foo\Categories.txt")).Returns(false).OccursOnce();
			Mock.Arrange(() => fileReader.ReadFile(@"C:\Foo\Categories.txt")).IgnoreArguments().OccursNever();

			fileReader.FileExists(@"C:\Foo\Categories.txt");

			Mock.Assert(fileReader);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldAssertOccurenceWhenCombinedWithNoSetupCalls()
		{
			string userName = "Bob";
			string password = "Password";

			ILoginService service = Mock.Create<ILoginService>();
			Mock.Arrange(() => service.ValidateUser(userName, password)).Returns(5).OccursOnce();
			Mock.Arrange(() => service.ValidateUser("foo", "bar")).OccursNever();

			SecurityHandler handler = new SecurityHandler(service);

			bool loggedIn = handler.LoginUser(userName, password);

			Assert.True(loggedIn);
			Assert.Equal(handler.UserID, 5);

			Mock.Assert(service);
		}

		public class NestedFoo
		{
			public virtual FooThatFails FooThatFailsOnCtor { get; set; }
		}

		public class FooThatFails
		{
			public FooThatFails(string message)
			{

			}

			public FooThatFails()
			{
				throw new ArgumentException("Failed");
			}
		}

		public class SecurityHandler
		{
			private readonly ILoginService _service;
			public int UserID { get; internal set; }

			public SecurityHandler(ILoginService service)
			{
				_service = service;
				_service.DatabaseName = "NorthWind";
			}


			public bool LoginUser(string userName, string password)
			{
				UserID = _service.ValidateUser(userName, password);
				return (UserID != 0);
			}
		}

		public interface ILoginService
		{
			int ValidateUser(string userName, string password);


			string DatabaseName { get; set; }

			event EventHandler UserLoggedOnEvent;
			event EventHandler DatabaseChangedEvent;
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldAssertCallOriginalOnOverloadsViaProxy()
		{
			var dummyExpression = Mock.Create<DummyExpression>(Behavior.CallOriginal);
			dummyExpression.Evaluate(10);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void ShouldAssertSetPropertyOccurenceForAnyValue()
		{
			var foo = Mock.Create<IFoo>();

			Mock.ArrangeSet(() => foo.EffectiveFrom = DateTime.Now).IgnoreArguments();

			foo.EffectiveFrom = DateTime.Now;

			Assert.Throws<AssertionException>(() => Mock.AssertSet(() => foo.EffectiveFrom = Arg.IsAny<DateTime>(), Occurs.Never()));
		}

		[TestMethod, TestCategory("Lite")]
		public void ShouldAssertWithByteArrayArguments()
		{
			ITestInterface ti = Mock.Create<ITestInterface>();

			byte[] newimagebytes = new byte[1] { 4 };

			ti.DoStuff(newimagebytes);

			Mock.Assert(() => ti.DoStuff(newimagebytes), Occurs.AtLeastOnce());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Misc")]
		public void UsingShouldNotInterfereWithPreOccurrence()
		{
			var fakereader = Mock.Create<IXmlReader>();

			Mock.Arrange(() => fakereader.EOF).Returns(true).OccursOnce();
			Mock.Arrange(() => fakereader.ReadOuterXml()).Returns("aaa").OccursNever();

			using (fakereader)
			{
				if (!fakereader.EOF)
				{
					string s = fakereader.ReadOuterXml();
				}
			}

			Mock.Assert(fakereader);
		}

		[TestMethod, TestCategory("Lite")]
		public void ShouldAssertNewGuIdArgumentForSpecificValue()
		{
			var localPersister = Mock.Create<IProcessDataPersister>();

			Mock.Arrange(() => localPersister.GetTaskWarnings(new Guid("{00000000-0000-0000-0001-000000000003}")))
				.Returns(new List<TaskWarning>() { new TaskWarning(new Guid("{00000000-0000-0000-0001-000000000003}")) { EscalationLevel = 0 } })
				.MustBeCalled();

			var list = localPersister.GetTaskWarnings(new Guid("{00000000-0000-0000-0001-000000000003}"));

			Assert.NotNull(list);
			Mock.Assert(localPersister);
		}

		[TestMethod, TestCategory("Lite")]
		public void ShouldConfirmMockingClassWithMethodHidingItsVirtualBase()
		{
			var child = Mock.Create<ChildClass>();
			Assert.NotNull(child);
		}

		public class ChildClass : ParentClass, IElement
		{
			public new bool CanWriteProperty(string propertyName)
			{
				throw new NotImplementedException();
			}
		}

		public interface IElement
		{
			bool CanWriteProperty(string propertyName);
		}

		public class ParentClass
		{
			public virtual bool CanWriteProperty(string propertyName)
			{
				return false;
			}
		}

		public class TaskWarning
		{
			private Guid guid;

			public TaskWarning(Guid guid)
			{
				this.guid = guid;
			}
			public int EscalationLevel { get; set; }
		}

		public interface IProcessDataPersister
		{
			List<TaskWarning> GetTaskWarnings(Guid taskId);
		}

		public interface ITestInterface
		{
			void DoStuff(byte[] bytes);
		}

		public class Foo<TEntity>
		{
			public virtual TEntity GetByKey(params object[] keyValues)
			{
				return default(TEntity);
			}
		}

		public class ContentFacade<TItem>
		{
			public virtual IContentManager ContentManager { get; set; }

			internal virtual void LoadItem(Guid guid)
			{
				var product = this.ContentManager.GetItem(typeof(TItem), guid);

				if (product == null)
				{
					throw new ArgumentException("Invalid object");
				}
			}
		}

		public class BlogFacade : ContentFacade<Product>
		{

		}

		public interface IContentManager
		{
			object GetItem(Type itemType, Guid id);
		}

		public interface IXmlReader : IDisposable
		{
			bool EOF { get; }
			string ReadOuterXml();
		}

		public class DummyExpression
		{
			public virtual object Evaluate(int arg1, string myString)
			{
				return null;
			}

			public virtual object Evaluate(int arg1)
			{
				return null;
			}
		}

		public interface IFileReader
		{
			bool FileExists(string pathAndFile);
			IList<string> ReadFile(string pathAndFile);
		}

		public enum FooWorkType
		{
			Go = 0,
			Went
		}

		public class FooWork
		{
			public virtual void DoWork(FooAbstract foo)
			{

			}
		}

		public abstract class FooAbstract : IEquatable<FooAbstract>
		{
			public abstract FooWorkType Type { get; }

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				return this.Equals(obj as FooAbstract);
			}

			public bool Equals(FooAbstract other)
			{
				if (object.ReferenceEquals(this, other))
				{
					return true;
				}
				return this.Type == other.Type;
			}
		}

		public class FooInternal
		{
			internal FooInternal(string name)
			{
				this.name = name;
			}

			public string Name { get { return name; } }

			private string name;
		}

		public interface IExampleInterface
		{
			IList<IFoo> GetMeAllFoos();
		}

		public interface IDataContext
		{
			T Get<T>();
		}

		public class FooOuter
		{
			public virtual FooInter GetInnerClass()
			{
				return null;
			}
		}

		public class FooInter
		{
			public virtual int Value { get; set; }
		}


		public class Product : IContainer
		{

			#region IContainer Members

			public void Resolve(object obj)
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		public interface IDatabase
		{

		}

		public interface IContainer
		{
			void Resolve(object obj);
		}

		public class FakeContainer<T> where T : class
		{
			public virtual void Do<TSub>() where TSub : IContainer
			{
				throw new NotImplementedException();
			}
		}


		public interface IFooDispose : IDisposable
		{
			void Do();
		}

		public class Foo : IFoo
		{
			#region IFoo Members

			public int Get(SomeClass<string> id)
			{
				throw new NotImplementedException();
			}

			public DateTime EffectiveFrom { get; set; }

			public long Id
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public void SetIt(long it)
			{
				throw new NotImplementedException();
			}

			#endregion

		}

		public interface IBar
		{
			void Do(IFoo foo);
		}

		public class Bar
		{
			public Bar(IBar bar)
			{
				this.bar = bar;
			}

			public void Do(IFoo foo)
			{
				bar.Do(foo);
			}

			private IBar bar;
		}


		public interface IDummy
		{
			// dummy interface.
		}

		public interface ISomething<T>
		{
			void DoSomething<U>() where U : T;
		}


		public struct SomeClass<T> // Struct just to avoid having to implement Equals/GetHashCode
		{
			public static implicit operator SomeClass<T>(T t)
			{
				return new SomeClass<T>();
			}

			public static SomeClass<T> From(T t)
			{
				return t;
			}
		}

		public interface IFoo
		{
			int Get(SomeClass<string> id);
			long Id { get; set; }
			DateTime EffectiveFrom { get; set; }
			void SetIt(long it);
		}

		public interface IFooPtr
		{
			IntPtr Get(string input);
		}

		public interface IMyClass
		{
			IEnumerable<string> GetValuesSince(DateTime since);
		}
	}
}
