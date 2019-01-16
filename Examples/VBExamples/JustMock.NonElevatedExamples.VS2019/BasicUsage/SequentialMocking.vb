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
Imports Telerik.JustMock.Helpers


Namespace JustMock.NonElevatedExamples.BasicUsage.SequentialMocking
    ''' <summary>
    ''' Sequential mocking allows you to return different values on the same or different consecutive calls to 
    ''' one and the same type. In other words, you can set up expectations for successive calls of the same type. 
    ''' See http://www.telerik.com/help/justmock/basic-usage-sequential-mocking.html for full documentation of the feature.
    ''' </summary>
    <TestClass> _
    Public Class SequentialMocking_Tests
        <TestMethod> _
        Public Sub ShouldArrangeAndAssertInASequence()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: Sequence calls to foo.GetIntValue() should return different values.
            Mock.Arrange(Function() foo.GetIntValue()).Returns(0).InSequence()
            Mock.Arrange(Function() foo.GetIntValue()).Returns(1).InSequence()
            Mock.Arrange(Function() foo.GetIntValue()).Returns(2).InSequence()

            ' ACT
            Dim actualFirstCall As Integer = foo.GetIntValue()
            Dim actualSecondCall As Integer = foo.GetIntValue()
            Dim actualThirdCall As Integer = foo.GetIntValue()

            ' ASSERT
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, actualFirstCall)
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, actualSecondCall)
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, actualThirdCall)
        End Sub

        <TestMethod> _
        Public Sub ShouldAssertSequentlyWithAMatchers()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim iFoo = Mock.Create(Of IFoo)()

            ' Arranging: 
            '      When iFoo.Execute() is called with "foo" as an argument, it should return "hello". 
            '      Next iFoo.Execute() calls with any string as an argument should return "bye".
            Mock.Arrange(Function() iFoo.Execute("foo")).Returns("hello").InSequence()
            Mock.Arrange(Function() iFoo.Execute(Arg.IsAny(Of String)())).Returns("bye").InSequence()

            ' ACT
            Dim actualFirstCall As String = iFoo.Execute("foo")
            Dim actualSecondCall As String = iFoo.Execute("bar")

            ' This will also return "bye" as this is the last arrange in the sequence.
            Dim actualThirdCall As String = iFoo.Execute("foobar")

            ' ASSERT
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("hello", actualFirstCall)
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("bye", actualSecondCall)
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("bye", actualThirdCall)
        End Sub

        <TestMethod> _
        Public Sub ShouldAssertMultipleCallsWithDifferentMatchers()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: First call to foo.Echo() with any integer bigger than 10 as an argument should return 10, 
            '              every next call to foo.Echo() with any integer bigger than 20 as an argument should return 20. 
            Mock.Arrange(Function() foo.Echo(Arg.Matches(Of Integer)(Function(x) x > 10))).Returns(10).InSequence()
            Mock.Arrange(Function() foo.Echo(Arg.Matches(Of Integer)(Function(x) x > 20))).Returns(20).InSequence()

            ' ACT
            Dim actualFirstCall As Integer = foo.Echo(11)
            Dim actualSecondCall As Integer = foo.Echo(21)

            ' ASSERT
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(10, actualFirstCall)
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(20, actualSecondCall)
        End Sub

        <TestMethod> _
        Public Sub ShouldArrangeInSequencedReturns()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: First call to foo.Echo() with any integer as an argument should return 10, 
            '              every next call to foo.Echo() (using the same matcher) should return 11. 
            Mock.Arrange(Function() foo.Echo(Arg.AnyInt)).Returns(10).Returns(11)

            ' ACT
            Dim ctualFirstCall = foo.Echo(1)
            Dim actualSecondCall = foo.Echo(2)

            ' ASSERT
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(10, ctualFirstCall)
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(11, actualSecondCall)
        End Sub
    End Class

#Region "SUT"
    Public Interface IFoo
        Function Execute(arg As String) As String
        Function Echo(arg1 As Integer) As Integer
        Function GetIntValue() As Integer
    End Interface
#End Region
End Namespace
