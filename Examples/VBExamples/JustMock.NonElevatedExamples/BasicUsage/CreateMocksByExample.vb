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
'  Copyright © 2010-2014 Telerik AD
'  JustMock Lite

Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace JustMock.NonElevatedExamples.BasicUsage.CreateMocksByExample
    ''' <summary>
    ''' The built-in feature for creating mocks by example saves time when it comes to tiresome set up of arrangements. 
    ''' This functionality allows you to create mocks of a certain class (the system under test) 
    '''  and in the same time to arrange their behavior.
    ''' For simple tests with few arrangements, this provides only marginal benefit. 
    '''  The real benefit comes with complex tests with multiple arrangements
    ''' See http://www.telerik.com/help/justmock/basic-usage-create-mocks-by-example.html for full documentation of the feature.
    ''' </summary>
    <TestClass> _
    Public Class CreateMocksByExample_Tests
        <TestMethod> _
        Public Sub ShouldUseMatchers()
            ' Create a mock and arrange the Equals method, when called with any arguments, 
            '  to forward the call to Object.Equals with the given arguments.
            Mock.CreateLike(Of IEqualityComparer)(Function(cmp) cmp.Equals(Arg.AnyObject, Arg.AnyObject) = [Object].Equals(Param._1, Param._2))
        End Sub

        <TestMethod> _
        Public Sub SampleTest()
            ' Create a mock and arrange: 
            '  - the Driver property, should return "MSSQL" when called, 
            '  - the Open() method, when called with any string argument should return true.
            Dim conn = Mock.CreateLike(Of IConnection)(Function([me]) [me].Driver Is "MSSQL" AndAlso [me].Open(Arg.AnyString) = True)

            ' ASSERT
            Assert.AreEqual("MSSQL", conn.Driver)
            Assert.IsTrue(conn.Open(".\SQLEXPRESS"))
        End Sub

        <TestMethod> _
        Public Sub ShouldUseCreateLikeAlongWithStandartArrangements()
            ' Create a mock and arrange: 
            '  - the Driver property, should return "MSSQL" when called.
            Dim conn = Mock.CreateLike(Of IConnection)(Function([me]) [me].Driver Is "MSSQL")

            ' Arranging: The Open() method must be called with any string argument and it should return true.
            Mock.Arrange(Function() conn.Open(Arg.AnyString)).Returns(True).MustBeCalled()

            ' ASSERT
            Assert.AreEqual("MSSQL", conn.Driver)
            Assert.IsTrue(conn.Open(".\SQLEXPRESS"))
            Mock.Assert(conn)
        End Sub
    End Class

#Region "SUT"
    Public Interface IEqualityComparer
        Function Equals(a As Object, b As Object) As Boolean
    End Interface

    Public Interface IConnection
        ReadOnly Property Driver() As String
        Function Open(parameters As String) As Boolean
    End Interface
#End Region
End Namespace
