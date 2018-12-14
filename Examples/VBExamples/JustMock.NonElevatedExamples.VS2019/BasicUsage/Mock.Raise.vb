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


Namespace JustMock.NonElevatedExamples.BasicUsage.Mock_Raise
    ''' <summary>
    ''' The Raise method is used for raising mocked events. You can use custom or standard events.
    ''' See http://www.telerik.com/help/justmock/basic-usage-mock-raise.html for full documentation of the feature.
    ''' </summary>
    <TestClass> _
    Public Class Mock_Raise_Tests
        <TestMethod> _
        Public Sub ShouldInvokeMethodForACustomEventWhenRaised()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            Dim expected As String = "ping"
            Dim actual As String = String.Empty

            AddHandler foo.CustomEvent, Sub(exp As String) actual = exp

            ' ACT
            Mock.Raise(Sub() AddHandler foo.CustomEvent, Nothing, expected)

            ' ASSERT
            Assert.AreEqual(expected, actual)
        End Sub

        <TestMethod> _
        Public Sub ShouldRaiseEventWithStandardEventArgs()
            ' ARRANGE
            ' Creating a mocked instance of the "IExecutor" interface.
            Dim executor = Mock.Create(Of IExecutor(Of Integer))()

            Dim acutal As String = Nothing
            Dim expected As String = "ping"

            AddHandler executor.Done, Sub(sender As Object, args As FooArgs) acutal = args.Value

            ' ACT - Raising the event with the expected args.
            Mock.Raise(Sub() AddHandler executor.Done, Nothing, New FooArgs(expected))

            ' ASSERT
            Assert.AreEqual(expected, acutal)
        End Sub

        <TestMethod> _
        Public Sub Submit_OnIsChangedRaised_ShouldBeCalled()
            ' ARRANGE
            ' Creating the necessary mocked instances.
            Dim activeDocument = Mock.Create(Of IDocument)()
            Dim activeView = Mock.Create(Of IDocumentView)()

            ' Attaching the Submit method to the IsChanged event. 
            AddHandler activeDocument.IsChanged, New EventHandler(AddressOf activeDocument.Submit)

            ' Arranging: activeDocument.Submit should be called during the test method with any arguments.
            Mock.Arrange(Sub() activeDocument.Submit(Arg.IsAny(Of Object)(), Arg.IsAny(Of EventArgs)())).MustBeCalled()
            ' Arranging: activeView.Document should return activeDocument when called.
            Mock.Arrange(Function() activeView.Document).Returns(activeDocument)

            ' ACT
            Mock.Raise(Sub() AddHandler activeView.Document.IsChanged, Nothing, EventArgs.Empty)

            ' ASSERT - Asserting all arrangements on "foo".
            Mock.Assert(activeDocument)
        End Sub
    End Class

#Region "SUT"
    Public Interface IFoo
        Event CustomEvent As CustomEvent
    End Interface

    Public Delegate Sub CustomEvent(value As String)

    Public Interface IExecutor(Of T)
        Event Done As EventHandler(Of FooArgs)
    End Interface

    Public Class FooArgs
        Inherits EventArgs
        Public Sub New()
        End Sub

        Public Sub New(value As String)
            Me.Value = value
        End Sub

        Public Property Value() As String
            Get
                Return m_Value
            End Get
            Set(value As String)
                m_Value = Value
            End Set
        End Property
        Private m_Value As String
    End Class

    Public Interface IDocumentView
        ReadOnly Property Document() As IDocument
    End Interface

    Public Interface IDocument
        Event IsChanged As EventHandler
        Sub Submit(sender As Object, e As EventArgs)
    End Interface
#End Region
End Namespace
