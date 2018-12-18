'  JustMock Lite
'  Copyright © 2010-2014 Telerik EAD
' 
'    Licensed under the Apache License, Version 2.0 (the "License");
'    you may not use this file except in compliance with the License.
'    You may obtain a copy of the License at
' 
'      http://www.apache.org/licenses/LICENSE-2.0
' 
'    Unless required by applicable law or agreed to in writing, software
'    distributed under the License is distributed on an "AS IS" BASIS,
'    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'    See the License for the specific language governing permissions and
'    limitations under the License.

Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting


Namespace JustMock.NonElevatedExamples.BasicUsage.Matchers
    ''' <summary>
    ''' See http://www.telerik.com/help/justmock/basic-usage-matchers.html for full documentation of the feature.
    ''' Matchers let you ignore passing actual values as arguments used in mocks. 
    ''' Instead, they give you the possibility to pass just an expression that satisfies the 
    ''' argument type or the expected value range. There are several types of matchers supported in Telerik JustMock:
    '''     - Defined Matchers:
    '''         Arg.AnyBool
    '''         Arg.AnyDouble
    '''         Arg.AnyFloat
    '''         Arg.AnyGuid
    '''         Arg.AnyInt
    '''         Arg.AnyLong
    '''         Arg.AnyObject
    '''         Arg.AnyShort
    '''         Arg.AnyString
    '''         Arg.NullOrEmpty
    '''     - Arg.IsAny(Of Type)();
    '''     - Arg.IsInRange([FromValue : int], [ToValue : int], [RangeKind])
    '''     - Arg.Matches(Of T)(Expression(Of Predicate(Of T)) expression) 
    ''' </summary>
    <TestClass> _
    Public Class Matchers_Tests
        <TestMethod> _
        <ExpectedException(GetType(ArgumentException))> _
        Public Sub UsingMatchersAndSpecializations()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging when foo.Echo() is called with any integer as an argument it should return 10.
            Mock.Arrange(Function() foo.Echo(Arg.AnyInt)).Returns(10)
            ' Arranging when foo.Echo() is called with integer, bigger than 10 as an argument it should throw ArgumentException.
            Mock.Arrange(Function() foo.Echo(Arg.Matches(Of Integer)(Function(x) x > 10))).Throws(New ArgumentException())

            ' ACT
            Dim actual As Integer = foo.Echo(1)

            ' ASSERT
            Assert.AreEqual(10, actual)

            ' ACT - This will throw ArgumentException.
            foo.Echo(11)
        End Sub

        <TestMethod> _
        Public Sub ShouldUseMatchersInArrange()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging when foo.Echo() is called with arguments: integer equals to 10 and integer equals to 20, 
            '  it should return 30.
            Mock.Arrange(Function() foo.Echo(Arg.Matches(Of Integer)(Function(x) x = 10), Arg.Matches(Of Integer)(Function(x) x = 20))).Returns(30)

            ' ACT
            Dim actual = foo.Echo(10, 20)

            ' ASSERT
            Assert.AreEqual(30, actual)
        End Sub

        <TestMethod>
        Public Sub IgnoringAllArgumentsForASpecificExpectation()
            ' Arrange
            Dim foo = Mock.Create(Of IFoo)()

            Mock.Arrange(Function() foo.Echo(0, 0)).IgnoreArguments().Returns(10)

            ' Act
            Dim actual As Integer = foo.Echo(10, 200)

            ' Assert
            Assert.AreEqual(10, actual)
        End Sub

        <TestMethod> _
        Public Sub ShouldUseMatchersInAssert()
            ' ARRANGE
            ' Creating a mocked instance of the "IPaymentService" interface.
            Dim paymentService = Mock.Create(Of IPaymentService)()

            ' ACT
            paymentService.ProcessPayment(DateTime.Now, 54.44D)

            ' ASSERT - Asserting that paymentService.ProcessPayment() is called with arguments: 
            '              - any DateTime
            '              - decimal equals 54.44M.
            Mock.Assert(Sub() paymentService.ProcessPayment(Arg.IsAny(Of DateTime)(), Arg.Matches(Of Decimal)(Function(paymentAmount) paymentAmount = 54.44D)))
        End Sub

        <TestMethod> _
        Public Sub ShouldIgnoreArgumentsWuthMatcherInAssert()
            ' ARRANGE
            ' Creating a mocked instance of the "IPaymentService" interface.
            Dim paymentService = Mock.Create(Of IPaymentService)()

            ' ACT
            paymentService.ProcessPayment(DateTime.Now, 54.44D)

            ' ASSERT - Asserting that paymentService.ProcessPayment() is called no matter the arguments.
            Mock.Assert(Sub() paymentService.ProcessPayment(New DateTime(), 0), Args.Ignore())
        End Sub

        <TestMethod> _
        Public Sub MatchingCertainRefParameters()
            Dim myRefArg As Integer = 5

            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging when foo.Bar() is called with ref argument that equals 5, it should return 10.
            Mock.Arrange(Function() foo.Bar(5)).Returns(10)

            ' ACT
            Dim actual As Integer = foo.Bar(myRefArg)

            ' ASSERT
            Assert.AreEqual(10, actual)
            Assert.AreEqual(5, myRefArg)
            ' Asserting that the ref arguments has not been changed.
        End Sub

        <TestMethod> _
        Public Sub MatchingRefParametersOfAnyType()
            Dim myRefArg As Integer = 5

            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging when foo.Bar() is called with any integer ref argument, it should return 10.
            Mock.Arrange(Function() foo.Bar(Arg.AnyInt)).Returns(10)

            ' ACT
            Dim actual As Integer = foo.Bar(myRefArg)

            ' ASSERT
            Assert.AreEqual(10, actual)
            Assert.AreEqual(5, myRefArg)
            ' Asserting that the ref arguments has not been changed.
        End Sub

        <TestMethod> _
        Public Sub MatchingRefParametersWithSpecialization()
            Dim myRefArg As Integer = 11

            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging when foo.Bar() is called with integer ref argument that is bigger than 10, it should return 10.
            Mock.Arrange(Function() foo.Bar(Arg.Matches(Of Integer)(Function(x) x > 10))).Returns(10)

            ' ACT
            Dim actual As Integer = foo.Bar(myRefArg)

            ' ASSERT
            Assert.AreEqual(10, actual)
            Assert.AreEqual(11, myRefArg)
            ' Asserting that the ref arguments has not been changed.
        End Sub
    End Class

#Region "SUT"
    Public Interface IPaymentService
        Sub ProcessPayment(dateTi As DateTime, deci As Decimal)
    End Interface

    Public Interface IFoo
        Function Echo(intArg1 As Integer) As Integer
        Function Echo(intArg1 As Integer, intArg2 As Integer) As Integer
        Function Bar(ByRef intArg1 As Integer) As Integer
    End Interface
#End Region
End Namespace
