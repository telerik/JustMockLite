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
Imports Telerik.JustMock.Helpers
Imports Telerik.JustMock


Namespace JustMock.NonElevatedExamples.BasicUsage.FluentMocking
    ''' <summary>
    ''' Fluent Assertions allow you to easily follow the Arrange Act Assert pattern in a straigtforward way.
    ''' Note that JustMock dynamically checks for any assertion mechanism provided by the underlying test framework 
    ''' if such one is available (MSTest, XUnit, NUnit, MbUnit, Silverlight) and uses it, rather than using its own 
    ''' MockAssertionException when a mock assertion fails. This functionality extends the JustMock tooling support 
    ''' for different test runners. 
    ''' See http://www.telerik.com/help/justmock/basic-usage-fluent-mocking.html for full documentation of the feature.
    ''' 
    ''' Note: To write in a fluent way, you will need to have the Telerik.JustMock.Helpers namespace included. 
    ''' </summary>
    <TestClass> _
    Public Class FluentMocking_Tests
        <TestMethod> _
        Public Sub ShouldBeAbleToAssertSpecificFuntionForASetup()
            Dim expected = "c:\JustMock"

            ' ARRANGE
            ' Creating a mocked instance of the "IFileReader" interface.
            Dim fileReader = Mock.Create(Of IFileReader)()

            ' Arranging: When fileReader.Path_GET is called, it should return expected.
            fileReader.Arrange(Function(x) x.Path).Returns(expected).OccursOnce()

            ' ACT
            Dim actual = fileReader.Path

            ' ASSERT
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, actual)
            fileReader.Assert(Function(x) x.Path)
        End Sub
    End Class

#Region "SUT"
    Public Interface IFileReader
        Property Path() As String

        Sub Initialize()
    End Interface
#End Region
End Namespace
