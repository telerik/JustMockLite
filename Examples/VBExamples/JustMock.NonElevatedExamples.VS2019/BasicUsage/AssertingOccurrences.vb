'    limitations under the License.
'    See the License for the specific language governing permissions and
'    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'    distributed under the License is distributed on an "AS IS" BASIS,
'    Unless required by applicable law or agreed to in writing, software
' 
'      http://www.apache.org/licenses/LICENSE-2.0
' 
'    You may obtain a copy of the License at
'    you may not use this file except in compliance with the License.
'    Licensed under the Apache License, Version 2.0 (the "License");
' 
'  Copyright © 2010-2014 Telerik EAD
'  JustMock Lite

Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting


Namespace JustMock.NonElevatedExamples.BasicUsage.AssertingOccurrence
    ''' <summary>
    ''' See http://www.telerik.com/help/justmock/basic-usage-asserting-occurrence.html for full documentation of the feature.
    ''' Occurrence is used in conjunction with Mock.Assert and Mock.AssertSet to determine how many times a call has occurred.
    ''' There are 6 types of occurrence that we can use:
    '''    Occurs.Never() - Specifies that a particular call is never made on a mock.
    '''    Occurs.Once() - Specifies that a call has occurred only once on a mock.
    '''    Occurs.AtLeastOnce() - Specifies that a call has occurred at least once on a mock.
    '''    Occurs.AtLeast(numberOfTimes) - Specifies the number of times at least a call should occur on a mock.
    '''    Occurs.AtMost(numberOfTimes) - Specifies the number of times at most a call should occur on a mock.
    '''    Occurs.Exactly(numberOfTimes) - Specifies exactly the number of times a call should occur on a mock. 
    ''' Furthermore, you can set occurrence directly in the arrangement of a method.
    ''' You can use one of 5 different constructs of Occur:
    '''    Occurs(numberOfTimes) - Specifies exactly the number of times a call should occur on a mock.
    '''    OccursOnce() - Specifies that a call should occur only once on a mock.
    '''    OccursNever() - Specifies that a particular call should never be made on a mock.
    '''    OccursAtLeast(numberOfTimes) - Specifies that a call should occur at least once on a mock.
    '''    OccursAtMost(numberOfTimes) - Specifies the number of times at most a call should occur on a mock. 
    ''' </summary>
    <TestClass> _
    Public Class AssertingOccurrence_Tests
        <TestMethod> _
        Public Sub ShouldOccursNever()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' ASSERT - Asserting that foo.Submit() has never occurred during the test method.
            Mock.Assert(Sub() foo.Submit(), Occurs.Never())
        End Sub

        <TestMethod> _
        Public Sub ShouldOccursOnce()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' ACT
            foo.Submit()

            ' ASSERT - Asserting that foo.Submit() occurs exactly once during the test method.
            Mock.Assert(Sub() foo.Submit(), Occurs.Once())
        End Sub

        <TestMethod> _
        Public Sub ShouldOccursAtLeastOnce()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' ACT
            foo.Submit()

            ' ASSERT - Asserting that foo.Submit() occurs at least once (could be more than once) during the test method.
            Mock.Assert(Sub() foo.Submit(), Occurs.AtLeastOnce())

            ' ACT - Calling foo.Submit() more times.
            foo.Submit()
            foo.Submit()

            ' ASSERT - This should pass again.
            Mock.Assert(Sub() foo.Submit(), Occurs.AtLeastOnce())
        End Sub

        <TestMethod> _
        Public Sub ShouldOccursAtLeastCertainNumberOfTimes()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' ACT
            foo.Submit()
            foo.Submit()
            foo.Submit()

            ' ASSERT - Asserting that foo.Submit() occurs at least three times during the test method.
            Mock.Assert(Sub() foo.Submit(), Occurs.AtLeast(3))

            ' ACT - Calling foo.Submit() more times.
            foo.Submit()

            ' ASSERT - This should pass again.
            Mock.Assert(Sub() foo.Submit(), Occurs.AtLeast(3))
        End Sub

        <TestMethod> _
        <ExpectedException(GetType(AssertFailedException))> _
        Public Sub ShouldOccursCertainNumberOfTimesAtMost()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' ACT
            foo.Submit()
            foo.Submit()

            ' ASSERT - Asserting that foo.Submit() occurs maximum twice during the test method.
            Mock.Assert(Sub() foo.Submit(), Occurs.AtMost(2))

            ' ACT - Calling foo.Submit() once again - 3 times in total.
            foo.Submit()

            ' Assert - This throws an exception.
            Mock.Assert(Sub() foo.Submit(), Occurs.AtMost(2))
        End Sub

        <TestMethod> _
        <ExpectedException(GetType(AssertFailedException))> _
        Public Sub ShouldOccursExactly()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' ACT
            foo.Submit()
            foo.Submit()
            foo.Submit()

            ' ASSERT - Asserting that foo.Submit() occurs exactly 3 times during the test method.
            Mock.Assert(Sub() foo.Submit(), Occurs.Exactly(3))

            ' ACT - Calling foo.Submit once again - 4 times in total.
            foo.Submit()

            ' Assert - This fails because foo.Submit was called more times than specified.
            Mock.Assert(Sub() foo.Submit(), Occurs.Exactly(3))
        End Sub

        <TestMethod> _
        <ExpectedException(GetType(AssertFailedException))> _
        Public Sub ShouldFailOnAssertAllWhenExpectionIsNotMet()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: That foo.Submit() should occur exactly twice during the test method.
            Mock.Arrange(Sub() foo.Submit()).Occurs(2)

            ' ACT - No actions.

            ' ASSERT - This will throw an exception as the expectations are not met.
            Mock.Assert(foo)
        End Sub

        <TestMethod> _
        Public Sub ShouldArrangeOccursOnce()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: That foo.Submit() should occur exactly once during the test method.
            Mock.Arrange(Sub() foo.Submit()).OccursOnce()

            ' ACT
            foo.Submit()

            ' ASSERT
            Mock.Assert(foo)
        End Sub

        <TestMethod> _
        Public Sub ShouldArrangeOccursNever()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: That foo.Submit() should never occur during the test method.
            Mock.Arrange(Sub() foo.Submit()).OccursNever()

            ' ACT - No actions.

            ' ASSERT
            Mock.Assert(foo)
        End Sub

        <TestMethod> _
        Public Sub ShouldArrangeOccursAtLeast()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: That foo.Submit() should occur at least twice during the test method.
            Mock.Arrange(Sub() foo.Submit()).OccursAtLeast(2)

            ' ACT - Calling foo.Submit() 3 times.
            foo.Submit()
            foo.Submit()
            foo.Submit()

            ' ASSERT - This passes as foo.Submit() is called at least twice.
            Mock.Assert(foo)
        End Sub

        <TestMethod> _
        <ExpectedException(GetType(AssertFailedException))> _
        Public Sub ShouldFailWhenInvokedMoreThanRequried()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: That foo.Submit() should occur maximum twice during the test method.
            Mock.Arrange(Sub() foo.Submit()).OccursAtMost(2)

            ' ACT
            foo.Submit()
            foo.Submit()
            foo.Submit() ' This throws an exception because foo.Submit is being called more times than specified.
        End Sub

        <TestMethod> _
        Public Sub ShouldBeAbleToAssertOccursUsingMatcherForSimilarCallAtOneShot()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: Different expectations depending on the argument of foo.Echo().
            Mock.Arrange(Function() foo.Echo(1)).Returns(Function(arg__1 As Integer) arg__1)
            Mock.Arrange(Function() foo.Echo(2)).Returns(Function(arg__1 As Integer) arg__1)
            Mock.Arrange(Function() foo.Echo(3)).Returns(Function(arg__1 As Integer) arg__1)

            ' ACT
            foo.Echo(1)
            foo.Echo(2)
            foo.Echo(3)

            ' ASSERT - This will pass as foo.Echo() has been called exactly 3 times no matter the argument.
            Mock.Assert(Function() foo.Echo(Arg.AnyInt), Occurs.Exactly(3))
        End Sub

        <TestMethod> _
        Public Sub ShouldVerifyCallsOrder()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: That foo.Submit() should be called before foo.Echo().
            Mock.Arrange(Sub() foo.Submit()).InOrder()
            Mock.Arrange(Function() foo.Echo(Arg.AnyInt)).InOrder()

            ' ACT
            foo.Submit()
            foo.Echo(5)

            ' ASSERT 
            Mock.Assert(foo)
        End Sub

        <TestMethod> _
        Public Sub ShouldAssertInOrderForDifferentInstancesInTestMethodScope()
            Dim userName As String = "Bob"
            Dim password As String = "Password"
            Dim userID As Integer = 5
            Dim cart = New List(Of String)() From { _
                "Foo", _
                "Bar" _
            }

            ' ARRANGE
            ' Creating mocked instances of the "IUserValidationService" and "IShoppingCartService" interfaces.
            Dim userServiceMock = Mock.Create(Of IUserValidationService)()
            Dim shoppingCartServiceMock = Mock.Create(Of IShoppingCartService)()

            ' Arranging: When userServiceMock.ValidateUser(userName, password) is called it should return userID. 
            '  Also this method should occur exactly once in a given order during the test execution. 
            Mock.Arrange(Function() userServiceMock.ValidateUser(userName, password)).Returns(userID).InOrder().OccursOnce()
            ' Arranging: When shoppingCartServiceMock.LoadCart(userID) is called it should return cart. 
            '  Also this method should occur exactly once in a given order during the test execution. 
            Mock.Arrange(Function() shoppingCartServiceMock.LoadCart(userID)).Returns(cart).InOrder().OccursOnce()

            ' ACT
            userServiceMock.ValidateUser(userName, password)
            shoppingCartServiceMock.LoadCart(userID)

            ' ASSERT - Asserting occurrence and calls order. 
            Mock.Assert(userServiceMock)
            Mock.Assert(shoppingCartServiceMock)
        End Sub
    End Class

#Region "SUT"
    Public Interface IFoo
        Sub Submit()
        Function Echo(intArg As Integer) As Integer
    End Interface

    Public Interface IUserValidationService
        Function ValidateUser(userName As String, password As String) As Integer
    End Interface

    Public Interface IShoppingCartService
        Function LoadCart(userID As Integer) As IList(Of String)
    End Interface
#End Region
End Namespace

