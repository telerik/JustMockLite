Imports System
Imports System.Linq
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Telerik.JustMock

Namespace JustMock.NonElevatedExamples.AdvancedUsage.MockingDbContextSeams
    ''' <summary>
    ''' JustMock Lite keeps healthcare data access testable through explicit overridable or
    ''' interface seams without elevated interception.
    ''' </summary>
    <TestClass>
    Public Class MockingDbContextSeams_Tests
        <TestMethod>
        Public Sub ShouldUseVirtualContextSeamForPatientQueries()
            Using backingContext = SeamHealthcareContext.CreateInMemory("MockingDbContextSeams.Virtual")
                backingContext.Patients.AddRange(
                    New SeamPatient With {.Id = 1, .Name = "Olivia Carter", .Department = "Cardiology", .IsActive = False},
                    New SeamPatient With {.Id = 2, .Name = "Liam Turner", .Department = "Cardiology", .IsActive = True})
                backingContext.SaveChanges()

                Dim context = Mock.Create(Of SeamHealthcareContext)()
                Mock.Arrange(Function() context.Patients).Returns(backingContext.Patients)

                Dim actual = New VirtualPatientReader(context).FindActivePatient()

                Assert.IsNotNull(actual)
                Assert.AreEqual("Liam Turner", actual.Name)
            End Using
        End Sub

        <TestMethod>
        Public Sub ShouldReturnFakePatientCollectionForFutureInstance()
            Using backingContext = SeamHealthcareContext.CreateInMemory("MockingDbContextSeams.Future")
                backingContext.Patients.Add(
                    New SeamPatient With {.Id = 4, .Name = "Mia Chen", .Department = "Neurology", .IsActive = True})
                backingContext.SaveChanges()

                Dim context = Mock.Create(Of FuturePatientContext)()
                Mock.Arrange(Function() context.GetPatients()).
                    IgnoreInstance().
                    Returns(backingContext.Patients)

                Dim actual = New FuturePatientReader(Function() Mock.Create(Of FuturePatientContext)()).GetById(4)

                Assert.AreEqual("Mia Chen", actual.Name)
            End Using
        End Sub

        <TestMethod>
        Public Sub ShouldUseInterfaceContextSeamForPatientWrites()
            Using backingContext = SeamHealthcareContext.CreateInMemory("MockingDbContextSeams.Interface")
                Dim context = Mock.Create(Of ISeamPatientContext)()
                Mock.Arrange(Function() context.Patients).Returns(backingContext.Patients)
                Mock.Arrange(Function() context.SaveChanges()).
                    Returns(1).
                    MustBeCalled()

                Dim result = New InterfacePatientWriter(context).Add(New SeamPatient With {
                    .Id = 3,
                    .Name = "Noah Williams",
                    .Department = "Pediatrics",
                    .IsActive = True
                })

                Assert.AreEqual(1, result)
                Assert.AreEqual(1, backingContext.Patients.Local.Count)
                Mock.Assert(context)
            End Using
        End Sub
    End Class

    Public Class SeamHealthcareContext
        Inherits DbContext

        Public Sub New()
        End Sub

        Public Sub New(options As DbContextOptions(Of SeamHealthcareContext))
            MyBase.New(options)
        End Sub

        Public Overridable Property Patients As DbSet(Of SeamPatient)

        Public Overridable Property Doctors As DbSet(Of SeamDoctor)

        Public Shared Function CreateInMemory(databaseName As String) As SeamHealthcareContext
            Dim options = New DbContextOptionsBuilder(Of SeamHealthcareContext)().
                UseInMemoryDatabase(databaseName).
                Options
            Dim context = New SeamHealthcareContext(options)

            context.Database.EnsureDeleted()
            Return context
        End Function
    End Class

    Public Interface ISeamPatientContext
        ReadOnly Property Patients As DbSet(Of SeamPatient)

        Function SaveChanges() As Integer
    End Interface

    Public Class VirtualPatientReader
        Private ReadOnly context As SeamHealthcareContext

        Public Sub New(context As SeamHealthcareContext)
            Me.context = context
        End Sub

        Public Function FindActivePatient() As SeamPatient
            Return context.Patients.
                Where(Function(patient) patient.IsActive).
                OrderBy(Function(patient) patient.Id).
                FirstOrDefault()
        End Function
    End Class

    Public Class FuturePatientReader
        Private ReadOnly createContext As Func(Of FuturePatientContext)

        Public Sub New(createContext As Func(Of FuturePatientContext))
            Me.createContext = createContext
        End Sub

        Public Function GetById(patientId As Integer) As SeamPatient
            Dim context = createContext()
            Return context.GetPatients().
                Where(Function(patient) patient.Id = patientId).
                Single()
        End Function
    End Class

    Public Class FuturePatientContext
        Public Overridable Function GetPatients() As IQueryable(Of SeamPatient)
            Return Nothing
        End Function
    End Class

    Public Class InterfacePatientWriter
        Private ReadOnly context As ISeamPatientContext

        Public Sub New(context As ISeamPatientContext)
            Me.context = context
        End Sub

        Public Function Add(patient As SeamPatient) As Integer
            context.Patients.Add(patient)
            Return context.SaveChanges()
        End Function
    End Class

    Public Class SeamPatient
        Public Property Id As Integer
        Public Property Name As String
        Public Property Department As String
        Public Property IsActive As Boolean
    End Class

    Public Class SeamDoctor
        Public Property Id As Integer
        Public Property Name As String
        Public Property Specialty As String
    End Class
End Namespace
