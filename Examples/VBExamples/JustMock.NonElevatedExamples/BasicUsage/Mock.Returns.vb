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



Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Telerik.JustMock
Imports Telerik.JustMock.Helpers


Namespace JustMock.NonElevatedExamples.BasicUsage.Mock_Returns
    ''' <summary>
    ''' The Returns method is used with non void calls to ignore the actual call and return a custom value.
    ''' See http://www.telerik.com/help/justmock/basic-usage-mock-returns.html for full documentation of the feature.
    ''' </summary>
    <TestClass>
    Public Class Mock_Returns_Tests
        <TestMethod>
        Public Sub ShouldAssertPropertyGetCall()
            Dim expected = 10

            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: When foo.Bar is called, it should return the expected value.
            Mock.Arrange(Function() foo.Bar).Returns(expected)

            ' ACT
            Dim actual = foo.Bar

            ' ASSERT
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, actual)
        End Sub

        <TestMethod>
        Public Sub ShouldAssertMethodCallWithMatcher1()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: When foo.Echo() is called with any integer as an argument, it should return 1 + that argument.
            Mock.Arrange(Function() foo.Echo(Arg.IsAny(Of Integer)())).Returns(Function(i As Integer) System.Threading.Interlocked.Increment(i))

            ' ACT
            Dim actual = foo.Echo(10)

            ' ASSERT
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(11, actual)
        End Sub

        <TestMethod>
        Public Sub ShouldAssertMethodCallWithMatcher2()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: When foo.Echo() is called with an integer argument exactly matching 10, 
            '  it should return that argument.
            Mock.Arrange(Function() foo.Echo(Arg.Matches(Of Integer)(Function(x) x = 10))).Returns(Function(i As Integer) i)

            ' ACT
            Dim actual = foo.Echo(10)

            ' ASSERT
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(10, actual)
        End Sub

        <TestMethod>
        Public Sub ShouldReturnWhateverSecondArgIs()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: When foo.Execute() is called with any integer arguments, it should return the second argument.
            Mock.Arrange(Function() foo.Execute(Arg.IsAny(Of Integer)(), Arg.IsAny(Of Integer)())).Returns(Function(id As Integer, i As Integer) i)

            ' ACT
            Dim actual = foo.Execute(100, 10)

            ' ASSERT
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(actual, 10)
        End Sub


        <TestMethod>
        Public Sub ShouldReturnInSequence()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            Dim values As Integer() = New Integer(2) {1, 2, 3}

            ' Arranging: When foo.Bar_GET is called number of times, it should return the array values in sequence.
            Mock.Arrange(Function() foo.Bar).ReturnsMany(values)

            ' ACT
            Dim first = foo.Bar
            Dim second = foo.Bar
            Dim third = foo.Bar

            ' ASSERT
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(first, 1)
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(second, 2)
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(third, 3)
        End Sub
    End Class

#Region "SUT"
    Public Interface IFoo
        Property Bar() As Integer
        Function Echo(myInt As Integer) As Integer
        Function Execute(myInt1 As Integer, myInt2 As Integer) As Integer
    End Interface
#End Region
End Namespace
