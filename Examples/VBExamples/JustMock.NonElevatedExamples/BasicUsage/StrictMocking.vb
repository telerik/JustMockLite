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
Imports Telerik.JustMock.Core
Imports Telerik.JustMock


Namespace JustMock.NonElevatedExamples.BasicUsage.StrictMocking
    ''' <summary>
    ''' You may have a case where you want to enable only arranged calls and to reject others. 
    ''' In such cases you need to set the mock Behavior to Strict.
    ''' See http://www.telerik.com/help/justmock/basic-usage-strict-mocking.html for full documentation of the feature.
    ''' </summary>
    <TestClass> _
    Public Class StrictMocking_Tests
        <ExpectedException(GetType(StrictMockException))> _
        <TestMethod> _
        Public Sub ArbitraryCallsShouldGenerateException()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface with Behavior.Strict. 
            ' This means, every non-arranged call from this instance will throw MockException.
            Dim foo = Mock.Create(Of IFoo)(Behavior.Strict)

            'ACT - As foo.VoidCall() is not arranged, it should throw an exception.
            foo.VoidCall()
        End Sub

        <TestMethod> _
        <ExpectedException(GetType(StrictMockException))> _
        Public Sub ArbitraryCallsShouldGenerateExpectedException()
            ' ARRANGE
            ' Creating a mocked instance of the "IFoo" interface with Behavior.Strict. 
            ' This means, every non-arranged call from this instance will throw MockException.
            Dim foo = Mock.Create(Of IFoo)(Behavior.Strict)

            'ACT - As foo.VoidCall() is not arranged, it should throw an exception.
            foo.GetGuid()
        End Sub
    End Class

#Region "SUT"
    Public Interface IFoo
        Sub VoidCall()
        Function GetGuid() As Guid
    End Interface
#End Region
End Namespace
