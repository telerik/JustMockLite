
This month, take advantage of our **30% discount on Telerik JustMock**—the fastest, most flexible and complete mocking solution for crafting unit tests. 
If you want to:
*	Remove the Hassle of Writing Unit Tests 
*	Utilize Arrange, Act, Assert (AAA) Pattern 
*	Mock Types and Members With Any Access Modifiers 
*	Fully Profiled Automocking 
*	Build System Integration 
*	Integrate with Visual Studio and Other Tools
We have got you covered! 

### With **Telerik JustMock you could save up to 50% in development time**. The product can be integrated seamlessly into your organization and doesn’t disrupt your existing business processes. 
[ Get Telerik JustMock at a 30% discount  ➤](https://store.progress.com/your-order)

### Are you building for **ASP.NET Web Forms/MVC/Core or WPF/Winforms or need a sturdy reporting solution?*** Get JustMock as part of our complete developer tools collection with priority support. 
[ Get DevCraft at a 30% discount  ➤](https://store.progress.com/your-order)

To receive your discount, simply apply this promo code at checkout:
**PROM-VNRFAN**

Happy coding, 

The Telerik Team at Progress 





JustMock Lite
===

[![release](https://img.shields.io/badge/release-R2%20SP1%202020-blue.svg)](https://www.nuget.org/packages/JustMock/)
[![nuget](https://img.shields.io/nuget/v/JustMock.svg?label=nuget)](https://www.nuget.org/packages/JustMock/)
[![license](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](https://github.com/telerik/JustMockLite/blob/master/LICENSE/)

**JustMock Lite by Telerik** is for developers who practice unit testing and want to deliver exceptional software. JustMock Lite is the superior free mocking framework that makes unit testing simpler for SOLID testable projects. It is also an open source product that is easy to use, feature rich, with great power and flexibility, making it the superior choice. JustMock Lite cuts your development time and helps you create better unit tests. It enables you to perform fast and controlled tests that are independent of external dependencies like databases, web services or proprietary code. For more information, refer to our <a href="http://www.telerik.com/justmock/free-mocking" target="_blank">JustMock Lite website</a>.

### Key Features
- AAA pattern – JustMockLite uses the Arrange Act Assert (AAA) pattern.
- Error-Free Mocking - Thanks to its strongly typed framework API, JustMock detects errors in your mock definitions and highlights them in Visual Studio. The JustMock Lite API fully leverages Visual Studio IntelliSense to make it easy to start mocking.
- Mock interfaces - Allows you to test public interfaces.
- Mock classes - Allows you to test public classes.
- Mock properties - Allows you to test the property getter and setter.
- Mock and rise events - Allows you to test events.
- Control mock behavior - Allows you to control the default behavior of a mock whether be to do nothing, call the original code or throw an exception.
- Assert call occurrences - Allows you to determine how many times a call has occurred.
- Recursive mocking - Еnable you to mock members that are obtained as a result of "chained" calls on a mock.
- Sequential mocking - Аllows you to return different values on the same or different consecutive calls to one and the same type.
- Support for out/ref - Mock methods accepting our and ref parameters.
- Support for Generics - Mock generic classes and methods.
- And many more.

### Examples

```csharp
[TestMethod]
public void TestBookService()
{
    // Arrange - initialize objects and prepare data.
    var repository = Mock.Create<IBookRepository>();
    var expectedBook = new Book { Title = "Adventures" };
    var service = new BookService(repository);
    
    // prepare an expectation of what the GetWhere method should do when called with the specified parameters
    // and how many times the call is supposed to occur.
    Mock.Arrange(() => repository.GetWhere(book => book.Id == 1)).Returns(expectedBook).OccursOnce();

    // Act - execute the tested logic.
    Book actualBook = service.GetSingleBook(1);

    // Assert - verify that the actual result is equal to the expected.
    Assert.AreEqual(expectedBook.Title, actualBook.Title);
}
```

### Supported Frameworks
- .Net Framework 4.5+ and later.
- .Net Core 2.0 and later.

### Documentation
JustMock Lite comes with extensive <a href="http://docs.telerik.com/devtools/justmock/introduction.html" target="_blank">Documentation</a> and examples that will help you quickly get started with the framework.

### Feedback
JustMock Lite is constantly improved through customer interaction and feedback. If you want to suggest a new feature, bug report or vote for a popular one, please visit our <a href="https://feedback.telerik.com/Project/105" target="_blank">Feedback Portal</a>.


### Happy mocking!
