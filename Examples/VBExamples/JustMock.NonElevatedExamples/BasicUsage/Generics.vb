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


Namespace JustMock.NonElevatedExamples.BasicUsage.Generics
    ''' <summary>
    ''' Telerik JustMock allows you to mock generic classes/interfaces/methods in the same way 
    ''' as you do it for non-generic ones.
    ''' See http://www.telerik.com/help/justmock/basic-usage-generics.html for full documentation of the feature.
    ''' </summary>
    <TestClass> _
    Public Class Generics_Tests
        <TestMethod> _
        Public Sub ShouldDistinguishVirtualCallsDependingOnArgumentTypes()
            Dim expextedCallWithInt As Integer = 0
            Dim expextedCallWithString As Integer = 1

            ' ARRANGE
            ' Creating a mock instance of the "FooGeneric" class.
            Dim foo = Mock.Create(Of FooGeneric)()

            ' Arranging: When foo.Get<int>() generic is called, it should return expextedCallWithInt.
            Mock.Arrange(Function() foo.[Get](Of Integer)()).Returns(expextedCallWithInt)
            ' Arranging: When foo.Get<string>() generic is called, it should return expextedCallWithString.
            Mock.Arrange(Function() foo.[Get](Of String)()).Returns(expextedCallWithString)

            ' ACT
            Dim actualCallWithInt = foo.[Get](Of Integer)()
            Dim actualCallWithString = foo.[Get](Of String)()

            ' ASSERT
            Assert.AreEqual(expextedCallWithInt, actualCallWithInt)
            Assert.AreEqual(expextedCallWithString, actualCallWithString)
        End Sub

        <TestMethod> _
        Public Sub ShouldMockGenericClass()
            Dim expectedValue As Integer = 1

            ' ARRANGE
            ' Creating a mock instance of the "FooGeneric<T>" class.
            Dim foo = Mock.Create(Of FooGeneric(Of Integer))()

            ' Arranging: When foo.Get() is called with any integer as an argument, it should return expectedValue.
            Mock.Arrange(Function() foo.[Get](Arg.IsAny(Of Integer)())).Returns(expectedValue)

            ' ACT
            Dim actualValue As Integer = foo.[Get](0)

            ' ASSERT
            Assert.AreEqual(expectedValue, actualValue)
        End Sub

        <TestMethod> _
        Public Sub ShouldMockGenericMethod()
            Dim expectedValue = 10

            ' ARRANGE
            ' Creating a mock instance of the "FooGeneric<T>" class.
            Dim genericClass = Mock.Create(Of FooGeneric(Of Integer))()

            ' Arranging: When genericClass.Get() is called with 1, 1 as arguments, it should return expectedValue.
            Mock.Arrange(Function() genericClass.[Get](1, 1)).Returns(expectedValue)

            ' ACT
            Dim actual = genericClass.[Get](1, 1)

            ' ASSERT
            Assert.AreEqual(expectedValue, actual)
        End Sub

        <TestMethod> _
        Public Sub ShouldMockMethodInGenericClass()
            Dim isCalled As Boolean = False

            ' ARRANGE
            ' Creating a mock instance of the "FooGeneric<T>" class.
            Dim genericClass = Mock.Create(Of FooGeneric(Of Integer))()

            ' Arranging: When genericClass.Execute() is called with 1 as an argument, it should notify for call.
            Mock.Arrange(Sub() genericClass.Execute(1)).DoInstead(Function() InlineAssignHelper(isCalled, True))

            ' ACT
            genericClass.Execute(1)

            ' ASSERT
            Assert.IsTrue(isCalled)
        End Sub

        <TestMethod> _
        Public Sub ShouldMockVirtualGenericMethodInNonGenericClass()
            Dim expectedValue = 10

            ' ARRANGE
            ' Creating a mock instance of the "FooGeneric" class.
            Dim genericClass = Mock.Create(Of FooGeneric)()

            ' Arranging: When genericClass.Get<int, int>() is called with 1 as an argument, it should return expectedValue.
            Mock.Arrange(Function() genericClass.[Get](Of Integer, Integer)(1)).Returns(expectedValue)

            ' ACT
            Dim actual = genericClass.[Get](Of Integer, Integer)(1)

            ' ASSERT
            Assert.AreEqual(expectedValue, actual)
        End Sub
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function
    End Class

#Region "SUT"
    Public Class FooGeneric
        Public Overridable Function [Get](Of T, TRet)(arg1 As T) As TRet
            Return Nothing
        End Function

        Public Overridable Function [Get](Of T)() As Integer
            Throw New NotImplementedException()
        End Function
    End Class

    Public Class FooGeneric(Of T)
        Public Overridable Function [Get](arg As T) As T
            Throw New NotImplementedException()
        End Function
        Public Overridable Function [Get](arg As T, arg2 As T) As T
            Throw New NotImplementedException()
        End Function

        Public Overridable Sub Execute(Of T1)(arg As T1)
            Throw New Exception()
        End Sub
    End Class
#End Region
End Namespace
