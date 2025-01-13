'  JustMock Lite
'  Copyright © 2010-2014,2022 Progress Software Corporation
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



Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Telerik.JustMock

Namespace JustMock.NonElevatedExamples.AdvancedUsage.MockingDelegates

    ''' <summary>
    ''' With Telerik JustMock you can mock delegates and additionally apply all mock capabilities on them. For example, you can 
    '''  assert against their invocation, arrange certain expectations and then pass them in the system under test.
    ''' See http://www.telerik.com/help/justmock/advanced-usage-mocking-delegates.html for full documentation of the feature.
    ''' </summary>
    <TestClass>
    Public Class MockingDelegates_Tests
        <TestMethod>
        Public Sub ShouldArrangeReturnExpectation()
            ' ARRANGE
            ' Creating a mock instance of the Func<int, int> delegate.
            Dim delegateMock = Mock.Create(Of Func(Of Integer, Integer))()

            ' Arranging: When the mock is called with 10 as an integer argument, it should return 20.
            Mock.Arrange(Function() delegateMock(10)).Returns(20)

            ' ACT
            Dim mySUT = New Foo()
            ' Assigning the mock to the dependent property.
            mySUT.FuncDelegate = delegateMock
            Dim actual = mySUT.GetInteger(10)

            ' ASSERT
            Assert.AreEqual(20, actual)
        End Sub

        <TestMethod>
        Public Sub ShouldArrangeOccurrenceExpectation()
            ' ARRANGE
            ' Creating a mock instance of the Func<int, int> delegate.
            Dim delegateMock = Mock.Create(Of Func(Of Integer, Integer))()

            ' Arranging: That the mock should be called with any integer values during the test execution.
            Mock.Arrange(Function() delegateMock(Arg.AnyInt)).MustBeCalled()

            ' ACT
            Dim mySUT = New Foo()
            mySUT.FuncDelegate = delegateMock
            ' Assigning the mock to the dependent property.
            Dim actual = mySUT.GetInteger(123)

            ' ASSERT - asserting the mock.
            Mock.Assert(delegateMock)
        End Sub

        <TestMethod>
        Public Sub ShouldPassPrearrangedDelegateMockAsArgument()
            ' ARRANGE
            ' Creating a mock instance of the Func<string> delegate.
            Dim delegateMock = Mock.Create(Of Func(Of String))()

            ' Arranging: When the mock is called, it should return "Success".
            Mock.Arrange(Function() delegateMock()).Returns("Success")

            ' ACT
            Dim testInstance = New DataRepository()
            ' Passing the mock into our system under test.
            Dim actual = testInstance.GetCurrentUserId(delegateMock)

            ' ASSERT
            Assert.AreEqual("Success", actual)
        End Sub

        <TestMethod>
        Public Sub ShouldPassDelegateMockAsArgumentAndAssertItsOccurrence()
            Dim isCalled As Boolean = False

            ' ARRANGE
            ' Creating a mock instance of the Action<int> delegate.
            Dim delegateMock = Mock.Create(Of Action(Of Integer))()

            ' Arranging: When the mock is called with any integer value as an argument, it should assign true to isCalled instead.
            Mock.Arrange(Sub() delegateMock(Arg.AnyInt)).DoInstead(Function() InlineAssignHelper(isCalled, True))

            ' ACT
            Dim testInstance = New DataRepository()
            ' Passing the mock into our system under test.
            testInstance.ApproveCredentials(delegateMock)

            ' ASSERT
            Assert.IsTrue(isCalled)
        End Sub
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function
    End Class

#Region "SUT"
    Public Class DataRepository
        Public Function GetCurrentUserId(callback As Func(Of String)) As String
            Return callback()
        End Function

        Public Sub ApproveCredentials(callback As Action(Of Integer))
            ' Some logic here...

            callback(1)
        End Sub
    End Class

    Public Class Foo
        Public Property FuncDelegate() As Func(Of Integer, Integer)
            Get
                Return m_FuncDelegate
            End Get
            Set(value As Func(Of Integer, Integer))
                m_FuncDelegate = value
            End Set
        End Property
        Private m_FuncDelegate As Func(Of Integer, Integer)

        Public Function GetInteger(toThisInt As Integer) As Integer
            Return FuncDelegate(toThisInt)
        End Function
    End Class
#End Region
End Namespace
