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

Namespace JustMock.NonElevatedExamples.BasicUsage.Mock_Throws
    ''' <summary>
    ''' The Throws method is used to throw an exception when a given call is made.
    ''' See http://www.telerik.com/help/justmock/basic-usage-mock-throws.html for full documentation of the feature.
    ''' </summary>
    <TestClass> _
    Public Class Mock_Throws_Tests
        <TestMethod> _
        <ExpectedException(GetType(ArgumentException))> _
        Public Sub ShouldThrowExceptionOnMethodCall()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: When foo.Execute() is called with any string as an argument, it should throw an ArgumentException.
            Mock.Arrange(Function() foo.Execute(Arg.AnyString)).Throws(Of ArgumentException)()

            ' ACT
            foo.Execute(String.Empty)
        End Sub

        <TestMethod> _
        <ExpectedException(GetType(ArgumentException))> _
        Public Sub ShouldThrowExceptionWithArgumentsOnMethodCall()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface.
            Dim foo = Mock.Create(Of IFoo)()

            ' Arranging: When foo.Execute() is called with an empty string as an argument, 
            '  it should throw an ArgumentException with args: "Argument shouldn't be empty.".
            Mock.Arrange(Function() foo.Execute(String.Empty)).Throws(Of ArgumentException)("Argument shouldn't be empty.")

            ' ACT
            foo.Execute(String.Empty)
        End Sub
    End Class

#Region "SUT"
    Public Interface IFoo
        Function Execute(myStr As String) As String
    End Interface
#End Region
End Namespace
