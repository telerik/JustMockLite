'  JustMock Lite
'  Copyright © 2010-2014 Telerik AD
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


Namespace JustMock.NonElevatedExamples.BasicUsage.Mock_DoInstead
    ''' <summary>
    ''' The DoInstead method is used to replace the actual implementation of a method with a mocked one.
    ''' See http://www.telerik.com/help/justmock/basic-usage-mock-do-instead.html for full documentation of the feature.
    ''' </summary>
    <TestClass> _
    Public Class Mock_DoInstead_Tests
        <TestMethod> _
        Public Sub Execute_OnExecuteWithAnyStringArg_ShouldNotifyAndReturnThatArg()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            Dim called As Boolean = False

            ' Arranging: When foo.Execute() is called with any string as an argument it should change "called" to true and also return that argument.
            Mock.Arrange(Function() foo.Execute(Arg.IsAny(Of String)())).DoInstead(Sub()
                                                                                       called = True
                                                                                   End Sub).Returns(Function(s As String) s)

            ' ACT 
            Dim actual = foo.Execute("bar")

            ' ASSERT
            Assert.AreEqual("bar", actual)
            Assert.IsTrue(called)
        End Sub

        <TestMethod> _
        Public Sub Submit_OnExecuteWitAnyIntArgs_ShouldAssignTheirSumToVariable()
            ' Arrange
            Dim expected As Integer = 0

            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: When foo.Submit() is called with any integers as an arguments it should assign their sum to the "expected" variable.
            Mock.Arrange(Function() foo.Submit(Arg.IsAny(Of Integer)(), Arg.IsAny(Of Integer)(), Arg.IsAny(Of Integer)(), Arg.IsAny(Of Integer)())).DoInstead(Sub(arg1 As Integer, arg2 As Integer, arg3 As Integer, arg4 As Integer)
                                                                                                                                                                           expected = arg1 + arg2 + arg3 + arg4

                                                                                                                                                                       End Sub)

            ' Act
            foo.Submit(10, 10, 10, 10)

            ' Assert
            Assert.AreEqual(40, expected)
        End Sub

        <TestMethod> _
        Public Sub Bar_OnSetTo1_ShouldNotify()
            ' Arrange
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            Dim isSetTo1 As Boolean = False

            ' Arranging: When foo.Bar is set to 1 it should change "isSetTo1" to true.
            Mock.ArrangeSet(Sub() foo.Bar = 1).DoInstead(Sub() isSetTo1 = True)

            ' Act
            foo.Bar = 1

            ' Assert
            Assert.IsTrue(isSetTo1)
        End Sub

        <TestMethod> _
        Public Sub AddTo_OnCertainCall_ShouldSumTheArgsAndAssignInsidedTheRefArg()
            ' Arrange
            Dim refArg As Integer = 1

            ' Creating a mocked instance of the "DoInsteadWithCustomDelegate" class.
            Dim myMock = Mock.Create(Of DoInsteadWithCustomDelegate)()

            ' Arranging: When myMock.AddTo is called with 10 and "refArg" it should assign their sum to the second argument (refArg).
            Mock.Arrange(Sub() myMock.AddTo(10, refArg)) _
                .DoInstead(New RefAction(Of Integer, Integer)(Sub(arg1 As Integer, ByRef arg2 As Integer)
                                                                              arg2 += arg1
                                                                          End Sub))

            ' Act
            myMock.AddTo(10, refArg)

            ' Assert
            Assert.AreEqual(11, refArg)
        End Sub
    End Class

#Region "SUT"
    Public Interface IFoo
        Property Bar() As Integer
        Function Execute(str As String) As String
        Function Submit(arg1 As Integer, arg2 As Integer, arg3 As Integer, arg4 As Integer) As Integer
    End Interface

    Public Delegate Sub RefAction(Of T1, T2)(arg1 As T1, ByRef arg2 As T2)

    Public Class DoInsteadWithCustomDelegate
        Public Overridable Sub AddTo(arg1 As Integer, ByRef arg2 As Integer)
        End Sub
    End Class
#End Region
End Namespace
