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


Namespace JustMock.NonElevatedExamples.AdvancedUsage.ConcreteMocking
    ''' <summary>
    ''' Concrete mocking is one of the advanced features supported in Telerik JustMock. Up to this point we have been talking 
    ''' mostly about mocking interfaces. This feature allows you to mock the creation of an object. To some extent this is available 
    ''' in the free edition and there are more things you can do in the commercial edition of the product.
    ''' See http://www.telerik.com/help/justmock/advanced-usage-concrete-mocking.html for full documentation of the feature.
    ''' </summary>
    <TestClass> _
    Public Class ConcreteMocking_Tests
        <TestMethod> _
        <ExpectedException(GetType(NotImplementedException))> _
        Public Sub ShouldCallOriginalForVirtualMemberWithMockedConstructor()
            ' ARRANGE
            ' Creating a mocked instance of the "FooVirtual" class.
            '  Telerik JustMock also gives you the ability to explicitly specify whether a constructor should be mocked or not.
            '  By default the constructor is not mocked.
            Dim foo = Mock.Create(Of FooVirtual)(Constructor.Mocked)

            ' Arranging: When foo.GetList() is called, it should call the original method implementation.
            Mock.Arrange(Function() foo.GetList()).CallOriginal()

            ' ACT
            foo.GetList()
        End Sub

        <TestMethod> _
        Public Sub VoidMethod_OnExcute_ShouldCallGetList()
            ' ARRANGE
            ' Creating a mocked instance of the "FooVirtual" class.
            '  Telerik JustMock also gives you the ability to explicitly specify whether a constructor should be mocked or not.
            '  By default the constructor is not mocked.
            Dim foo = Mock.Create(Of FooVirtual)(Constructor.Mocked)

            ' Arranging: When foo.SubMethod() is called, it should call foo.GetList() instead.
            Mock.Arrange(Sub() foo.SubMethod()).DoInstead(Function() foo.GetList())
            ' Arranging: That foo.GetList() must be called during the test method and it should do nothing.
            Mock.Arrange(Function() foo.GetList()).DoNothing().MustBeCalled()

            ' ACT
            foo.SubMethod()

            ' ASSERT
            Mock.Assert(foo)
        End Sub
    End Class

#Region "SUT"
    Public Class FooVirtual
        Public Sub New()
            Throw New NotImplementedException("Constructor")
        End Sub

        Public Overridable Property Name() As String
            Get
                Return m_Name
            End Get
            Set(value As String)
                m_Name = Value
            End Set
        End Property
        Private m_Name As String

        Public Overridable Sub SubMethod()
            Throw New NotImplementedException()
        End Sub

        Public Overridable Function GetList() As IList(Of Integer)
            Throw New NotImplementedException()
        End Function
    End Class
#End Region
End Namespace
