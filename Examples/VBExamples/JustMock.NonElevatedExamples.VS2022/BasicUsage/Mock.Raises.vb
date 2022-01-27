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
Imports Telerik.JustMock


Namespace JustMock.NonElevatedExamples.BasicUsage.Mock_Raises
    ''' <summary>
    ''' The Raises method is used to fire an event once a method is called.
    ''' See http://www.telerik.com/help/justmock/basic-usage-mock-raises.html for full documentation of the feature.
    ''' </summary>
    <TestClass> _
    Public Class Mock_Raises_Tests
        <TestMethod> _
        Public Sub ShouldRaiseCustomEventOnMethodCall()
            Dim expected = "ping"
            Dim actual As String = String.Empty

            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: When foo.RaiseMethod() is called, it should raise foo.CustomEvent with expected args.
            Mock.Arrange(Sub() foo.RaiseMethod()).Raises(Sub() AddHandler foo.CustomEvent, Nothing, expected)
            AddHandler foo.CustomEvent, Sub(s)
                                            actual = s
                                        End Sub

            ' ACT
            foo.RaiseMethod()

            ' ASSERT
            Assert.AreEqual(expected, actual)
        End Sub

        <TestMethod> _
        Public Sub ShouldRaiseCustomEventForFuncCalls()
            Dim isRaised As Boolean = False

            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: When foo.Echo() is called with expected arguments, it should raise foo.EchoEvent with arg: true.
            Mock.Arrange(Function() foo.Echo("string")).Raises(Sub() AddHandler foo.EchoEvent, Nothing, True)
            AddHandler foo.EchoEvent, Sub(c)
                                          isRaised = c
                                      End Sub

            ' ACT
            Dim actual = foo.Echo("string")

            ' ASSERT
            Assert.IsTrue(isRaised)
        End Sub

        <TestMethod> _
        Public Sub ShouldAssertMultipleEventSubscription()
            Dim isRaisedForFirstSubscr As Boolean = False
            Dim isRaisedForSecondSubscr As Boolean = False

            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: When foo.Execute() is called, it should raise foo.EchoEvent with arg: true.
            Mock.Arrange(Sub() foo.Execute()).Raises(Sub() AddHandler foo.EchoEvent, Nothing, True)

            ' Subscribing for the event
            AddHandler foo.EchoEvent, Sub(c)
                                          isRaisedForFirstSubscr = c
                                      End Sub
            AddHandler foo.EchoEvent, Sub(c)
                                          isRaisedForSecondSubscr = c
                                      End Sub

            ' ACT
            foo.Execute()

            ' ASSERT
            Assert.IsTrue(isRaisedForFirstSubscr)
            Assert.IsTrue(isRaisedForSecondSubscr)
        End Sub

    End Class

#Region "SUT"
    Public Delegate Sub CustomEvent(value As String)

    Public Delegate Sub EchoEvent(echoed As Boolean)

    Public Interface IFoo
        Event CustomEvent As CustomEvent
        Event EchoEvent As EchoEvent
        Sub RaiseMethod()
        Function Echo(arg As String) As String
        Sub Execute()
    End Interface
#End Region
End Namespace
