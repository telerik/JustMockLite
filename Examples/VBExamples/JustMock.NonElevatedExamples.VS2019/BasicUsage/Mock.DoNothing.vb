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


Namespace JustMock.NonElevatedExamples.BasicUsage.Mock_DoNothing
    ''' <summary>
    ''' The DoNothing method is used to arrange that a call to a method or property should be ignored.
    ''' See http://www.telerik.com/help/justmock/basic-usage-mock-do-nothing.html for full documentation of the feature.
    ''' </summary>
    <TestClass> _
    Public Class Mock_DoNothing_Tests
        <TestMethod> _
        Public Sub VoidCall_OnExecute_ShouldDoNothingAndMustBeCalled()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: foo.VoidCall() should do nothing and it must be called during the test method.
            Mock.Arrange(Sub() foo.VoidCall()).DoNothing().MustBeCalled()

            ' ACT
            foo.VoidCall()

            ' ASSERT - Asserting all arrangements on "foo".
            Mock.Assert(foo)
        End Sub

        <TestMethod> _
        Public Sub Bar_OnSetWithArgument1_ShouldDoNothingAndMustBeCalled()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: foo.Bar should do nothing when set to 1, but must be called.
            Mock.ArrangeSet(Sub() foo.Bar = 1).DoNothing().MustBeCalled()

            ' ACT
            foo.Bar = 1

            ' ASSERT - Asserting all arrangements on "foo".
            Mock.Assert(foo)
        End Sub
    End Class

#Region "SUT"
    Public Interface IFoo
        Property Bar() As Integer

        Sub VoidCall()
    End Interface
#End Region
End Namespace
