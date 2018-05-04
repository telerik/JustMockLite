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


Namespace JustMock.NonElevatedExamples.AdvancedUsage.PrivateAccessorNamespace
    ''' <summary>
    ''' The Telerik JustMock PrivateAccessor allows you to call non-public members of the tested code right in your unit tests. 
    ''' See http://www.telerik.com/help/justmock/advanced-usage-private-accessor.html for full documentation of the feature.
    ''' </summary>
    <TestClass> _
    Public Class PrivateAccessor_Tests
        <TestMethod> _
        Public Sub PrivateAccessor_ShouldCallPrivateMethod()
            ' ACT
            ' Wrapping the instance holding the private method.
            Dim inst = New PrivateAccessor(New ClassWithNonPublicMembers())
            ' Calling the non-public method by giving its exact name.
            Dim actual = inst.CallMethod("MePrivate")

            ' ASSERT
            Assert.AreEqual(1000, actual)
        End Sub

        <TestMethod> _
        Public Sub PrivateAccessor_ShouldCallPrivateStaticMethod()
            ' ACT
            ' Wrapping the instance holding the private method by type.
            Dim inst = PrivateAccessor.ForType(GetType(ClassWithNonPublicMembers))
            ' Calling the non-public static method by giving its exact name.
            Dim actual = inst.CallMethod("MeStaticPrivate")

            ' ASSERT
            Assert.AreEqual(2000, actual)
        End Sub

        <TestMethod> _
        Public Sub PrivateAccessor_ShouldGetSetProperty()
            ' ACT
            ' Wrapping the instance holding the private property.
            Dim inst = New PrivateAccessor(New ClassWithNonPublicMembers())
            ' Setting the value of the private property.
            inst.SetProperty("Prop", 555)

            ' ASSERT - Asserting with getting the value of the private property.
            Assert.AreEqual(555, inst.GetProperty("Prop"))
        End Sub
    End Class

#Region "SUT"
    Public Class ClassWithNonPublicMembers
        Private Property Prop() As Integer
            Get
                Return m_Prop
            End Get
            Set(value As Integer)
                m_Prop = Value
            End Set
        End Property
        Private m_Prop As Integer

        Private Function MePrivate() As Integer
            Return 1000
        End Function

        Private Shared Function MeStaticPrivate() As Integer
            Return 2000
        End Function
    End Class
#End Region
End Namespace
