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


Namespace JustMock.NonElevatedExamples.BasicUsage.RecursiveMocking
    ''' <summary>
    ''' Recursive mocks enable you to mock members that are obtained as a result of "chained" calls on a mock. 
    ''' For example, recursive mocking is useful in the cases when you test code like this: foo.Bar.Baz.Do("x"). 
    ''' See http://www.telerik.com/help/justmock/basic-usage-recursive-mocking.html for full documentation of the feature.
    ''' </summary>
    <TestClass> _
    Public Class RecursiveMocking_Tests
        <TestMethod> _
        Public Sub ShouldAssertNestedVeriables()
            Dim pingArg As String = "ping"
            Dim expected = "test"

            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: When foo.Bar.Do() is called, it should return the expected string. 
            '              This will automatically create mock of foo.Bar and a NullReferenceException will be avoided.
            Mock.Arrange(Function() foo.Bar.[Do](pingArg)).Returns(expected)

            ' ACT
            Dim actualFooBarDo = foo.Bar.[Do](pingArg)

            ' ASSERT
            Assert.AreEqual(expected, actualFooBarDo)
        End Sub

        <TestMethod> _
        Public Sub ShouldNotInstantiateFooBar()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' ASSERT - Not arranged members in a RecursiveLoose mocks should not be null.
            Assert.IsNotNull(foo.Bar)
        End Sub

        <TestMethod> _
        Public Sub ShouldAssertNestedPropertyGet()
            Dim expected = 10

            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: When foo.Bar.Value is called, it should return expected value. 
            '              This will automatically create mock of foo.Bar and a NullReferenceException will be avoided.
            Mock.Arrange(Function() foo.Bar.Value).Returns(expected)

            ' ACT
            Dim actual = foo.Bar.Value

            ' ASSERT
            Assert.AreEqual(expected, actual)
        End Sub

        <TestMethod> _
        Public Sub ShouldAssertNestedPropertySet()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: Setting foo.Bar.Value to 5, should do nothing. 
            '              This will automatically create mock of foo.Bar and a NullReferenceException will be avoided.
            Mock.ArrangeSet(Sub() foo.Bar.Value = 5).DoNothing().MustBeCalled()

            ' ACT
            foo.Bar.Value = 5

            ' ASSERT
            Mock.Assert(foo)
        End Sub

        <TestMethod> _
        Public Sub NestedPropertyAndMethodCalls()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: When foo.Bar.Do() is called with "x" as an argument, it return "xit". 
            '              This will automatically create mock of foo.Bar and a NullReferenceException will be avoided.
            Mock.Arrange(Function() foo.Bar.[Do]("x")).Returns("xit")

            ' Arranging: When foo.Bar.Baz.Do() is called with "y" as an argument, it return "yit". 
            '              This will automatically create mock of foo.Bar and foo.Bar.Baz and a 
            '              NullReferenceException will be avoided.
            Mock.Arrange(Function() foo.Bar.Baz.[Do]("y")).Returns("yit")

            ' ACT
            Dim actualFooBarDo = foo.Bar.[Do]("x")
            Dim actualFooBarBazDo = foo.Bar.Baz.[Do]("y")

            ' ASSERT
            Assert.AreEqual("xit", actualFooBarDo)
            Assert.AreEqual("yit", actualFooBarBazDo)
        End Sub
    End Class

#Region "SUT"
    Public Interface IFoo
        Property Bar() As IBar
        Function [Do](command As String) As String
    End Interface

    Public Interface IBar
        Property Value() As Integer
        Function [Do](command As String) As String
        Property Baz() As IBaz
    End Interface

    Public Interface IBaz
        Function [Do](command As String) As String
    End Interface
#End Region
End Namespace
