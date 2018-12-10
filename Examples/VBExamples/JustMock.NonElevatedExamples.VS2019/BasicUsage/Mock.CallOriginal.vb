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


Namespace JustMock.NonElevatedExamples.BasicUsage.Mock_CallOriginal
    ''' <summary>
    ''' The CallOriginal method marks a mocked method/property call that should execute the original method/property implementation.
    ''' See http://www.telerik.com/help/justmock/basic-usage-mock-call-original.html for full documentation of the feature.
    ''' </summary>
    <TestClass> _
    Public Class Mock_CallOriginal_Tests
        <TestMethod> _
        Public Sub ReturnSum_CallOriginal_ReturnsExpectedSum()
            ' ARRANGE
            ' Creating a mock instance of the "Log" class.
            Dim log = Mock.Create(Of Log)()

            ' Arranging when log.ReturnSum() is called with any integers as arguments it should execute its original implementation.
            Mock.Arrange(Function() log.ReturnSum(Arg.AnyInt, Arg.AnyInt)).CallOriginal()

            ' ACT - Calling log.ReturnSum with 1 and 2 as arguments.
            Dim actual = log.ReturnSum(1, 2)

            ' We should expect 3 as a return value
            Dim expected As Integer = 3

            ' ASSERT - We are asserting that the expected and the actual values are equal.
            Assert.AreEqual(expected, actual)
        End Sub

        <TestMethod> _
        <ExpectedException(GetType(Exception))> _
        Public Sub Info_CallOriginal_ThrowException()
            ' ARRANGE
            ' Creating a mock instance of the "Log" class.
            Dim log = Mock.Create(Of Log)()

            ' Arranging when log.Info() is called with any string as an argument it should execute its original implementation.
            Mock.Arrange(Sub() log.Info(Arg.IsAny(Of String)())).CallOriginal()

            ' ACT
            log.Info("test")

            ' ASSERT - We are asserting with the [ExpectedException(typeof(Exception))] test attribute.
        End Sub
    End Class

#Region "SUT"
    Public Class Log
        Public Overridable Function ReturnSum(firstInt As Integer, secondInt As Integer) As Integer
            Return firstInt + secondInt
        End Function

        Public Overridable Sub Info(message As String)
            Throw New Exception(message)
        End Sub
    End Class
#End Region
End Namespace
