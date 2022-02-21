'  JustMock Lite
'  Copyright © 2010-2014,2022 Telerik EAD
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
Imports Telerik.JustMock.Core
Imports Telerik.JustMock


Namespace JustMock.NonElevatedExamples.BasicUsage.MockingProperties
    ''' <summary>
    ''' Mocking properties is similar to mocking methods, but there are a few cases that need special attention 
    ''' like mocking indexers and particular set operations. 
    ''' See http://www.telerik.com/help/justmock/basic-usage-mock-properties.html for full documentation of the feature.
    ''' </summary>
    <TestClass> _
    Public Class MockingProperties_Tests
        <TestMethod> _
        Public Sub ShouldFakePropertyGet()
            Dim expectedValue = 25

            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: When foo.Value_GET is called, it should return expectedValue.
            Mock.Arrange(Function() foo.Value).Returns(expectedValue)

            ' ACT
            Dim actual = foo.Value

            ' ASSERT
            Assert.AreEqual(expectedValue, actual)
        End Sub

        <TestMethod> _
        Public Sub ShouldAssertPropertySet()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: That foo.Value must be set to 1 during the test method.
            Mock.ArrangeSet(Sub() foo.Value = 1).MustBeCalled()

            ' ACT
            foo.Value = 1

            ' ASSERT - Asserting the expected foo.Value_SET.
            Mock.AssertSet(Sub() foo.Value = 1)
        End Sub

        <TestMethod> _
        <ExpectedException(GetType(StrictMockException))> _
        Public Sub ShouldThrowExceptionOnTheThirdPropertySetCall()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface with Behavior.Strict.
            Dim foo = Mock.Create(Of IFoo)(Behavior.[Strict])

            ' Arranging: That foo.Value should be set to an integer bigger than 3.
            Mock.ArrangeSet(Sub() foo.Value = Arg.Matches(Of Integer)(Function(x) x > 3))

            ' ACT - These lines will not trigger the Strict behavior, because they satisfy the expectations.
            foo.Value = 4
            foo.Value = 5

            ' This throws MockException because matching criteria is not met.
            foo.Value = 3
        End Sub

        <TestMethod> _
        Public Sub MockIndexers()
            ' ARRANGE
            ' Creating a mocked instance of the "IIndexedFoo" interface.
            Dim indexedFoo = Mock.Create(Of IIndexedFoo)()

            ' Arranging: That the [0] element of indexedFoo should return "ping".
            Mock.Arrange(Function() indexedFoo(0)).Returns("ping")
            ' Arranging: That the [1] element of indexedFoo should return "pong".
            Mock.Arrange(Function() indexedFoo(1)).Returns("pong")

            ' ACT
            Dim actualFirst As String = indexedFoo(0)
            Dim actualSecond As String = indexedFoo(1)

            ' ASSERT
            Assert.AreEqual("ping", actualFirst)
            Assert.AreEqual("pong", actualSecond)
        End Sub

        <TestMethod> _
        <ExpectedException(GetType(StrictMockException))> _
        Public Sub ShouldThrowExceptionForNotArrangedPropertySet()
            ' ARRANGE
            ' Creating a mocked instance of the "IIndexedFoo" interface with Behavior.Strict.
            Dim foo = Mock.Create(Of IIndexedFoo)(Behavior.[Strict])

            ' Arranging: That the [0] element of foo should be set to "foo".
            Mock.ArrangeSet(Sub() foo(0) = "foo")

            ' ACT - This meets the expectations.
            foo(0) = "foo"

            ' This throws MockException because matching criteria is not met.
            foo(0) = "bar"
        End Sub

        <TestMethod> _
        <ExpectedException(GetType(StrictMockException))> _
        Public Sub ShouldAssertIndexedSetWithMatcher()
            ' ARRANGE
            ' Creating a mocked instance of the "IIndexedFoo" interface with Behavior.Strict.
            Dim foo = Mock.Create(Of IIndexedFoo)(Behavior.[Strict])

            ' Arranging: That the [0] element of foo should match a string "ping".
            Mock.ArrangeSet(Sub() foo(0) = Arg.Matches(Of String)(Function(x) x.Equals("ping")))
            ' Arranging: That the [1] element of foo should be any string.
            Mock.ArrangeSet(Sub() foo(1) = Arg.IsAny(Of String)())

            ' ACT - These lines will not trigger the Strict behavior, because they satisfy the expectations.
            foo(0) = "ping"
            foo(1) = "pong"

            ' This line does not satisfy the matching criteria and throws a MockException.
            foo(0) = "bar"
        End Sub
    End Class

#Region "SUT"
    Public Interface IIndexedFoo
        Default Property Item(key As Integer) As String
    End Interface

    Public Interface IFoo
        Property Value() As Integer
    End Interface
#End Region
End Namespace
