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
Imports Telerik.JustMock.AutoMock

Namespace JustMock.NonElevatedExamples.BasicUsage.Automocking
    ''' <summary>
    ''' Automocking allows the developer to create an instance of a class (the system under test) without having 
    ''' to explicitly create each individual dependency as a unique mock. The mocked dependencies are still available 
    ''' to the developer if methods or properties need to be arranged as part of the test. 
    ''' See http://www.telerik.com/help/justmock/basic-usage-automocking.html for full documentation of the feature.
    ''' </summary>
    <TestClass> _
    Public Class Automocking_Tests
        <TestMethod> _
        Public Sub ShouldMockDependenciesWithContainer()
            ' ARRANGE
            ' Creating a MockingContainer of ClassUnderTest. 
            ' To instantiate the system uder test (the container) you should use the Instance property 
            ' For example: container.Instance. 
            Dim container = New MockingContainer(Of ClassUnderTest)()

            Dim expectedString As String = "Test"

            ' Arranging: When the GetString() method from the ISecondDependecy interface 
            '              is called from the container, it should return expectedString. 
            container.Arrange(Of ISecondDependency)(Function(secondDep) secondDep.GetString()).Returns(expectedString)

            ' ACT - Calling SringMethod() from the mocked instance of ClassUnderTest
            Dim actualString = container.Instance.StringMethod()

            ' ASSERT
            Assert.AreEqual(expectedString, actualString)
        End Sub

        <TestMethod> _
        Public Sub ShouldAssertAllContainerArrangments()
            ' ARRANGE
            ' Creating a MockingContainer of ClassUnderTest. 
            ' To instantiate the system uder test (the container) you should use the Instance property 
            ' For example: container.Instance. 
            Dim container = New MockingContainer(Of ClassUnderTest)()

            ' Arranging: That the GetString() method from the ISecondDependecy interface 
            '              must be called from the container instance durring the test method. 
            container.Arrange(Of ISecondDependency)(Function(secondDep) secondDep.GetString()).MustBeCalled()

            ' ACT - Calling SringMethod() from the mocked instance of ClassUnderTest
            Dim actualString = container.Instance.StringMethod()

            ' ASSERT - Asserting all expectations for the container.
            container.AssertAll()
        End Sub
    End Class

#Region "SUT"
    Public Class ClassUnderTest
        Private firstDep As IFirstDependency
        Private secondDep As ISecondDependency

        Public Sub New(first As IFirstDependency, second As ISecondDependency)
            Me.firstDep = first
            Me.secondDep = second
        End Sub

        Public Function CollectionMethod() As IList(Of Object)
            Dim firstCollection = firstDep.GetList()

            Return firstCollection
        End Function

        Public Function StringMethod() As String
            Dim secondString = secondDep.GetString()

            Return secondString
        End Function
    End Class

    Public Interface IFirstDependency
        Function GetList() As IList(Of Object)
    End Interface

    Public Interface ISecondDependency
        Function GetString() As String
    End Interface
#End Region
End Namespace
