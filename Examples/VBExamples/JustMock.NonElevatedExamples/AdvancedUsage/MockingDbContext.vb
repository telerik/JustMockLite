Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Telerik.JustMock

Namespace JustMock.NonElevatedExamples.AdvancedUsage.MockingDbContext
    ''' <summary>
    ''' Entity Framework Core contexts expose overridable members that can be arranged with JustMock Lite.
    ''' An in-memory context supplies a real DbSet while the healthcare data access code remains isolated from the test.
    ''' </summary>
    <TestClass>
    Public Class MockingDbContext_Tests
        <TestMethod>
        Public Sub ShouldReturnFakePatientCollectionForQuery()
            Using backingContext = HealthcareContext.CreateInMemory()
                backingContext.Patients.AddRange(
                    New Patient With {.Id = 1, .Name = "Olivia Carter", .Department = "Cardiology", .DoctorId = 10, .IsActive = False},
                    New Patient With {.Id = 2, .Name = "Liam Turner", .Department = "Cardiology", .DoctorId = 10, .IsActive = True},
                    New Patient With {.Id = 3, .Name = "Mia Chen", .Department = "Neurology", .DoctorId = 20, .IsActive = True})
                backingContext.SaveChanges()

                Dim context = Mock.Create(Of HealthcareContext)()
                Mock.Arrange(Function() context.Patients).Returns(backingContext.Patients)

                Dim actual = New PatientDirectory(context).FindActivePatient("Cardiology")

                Assert.IsNotNull(actual)
                Assert.AreEqual("Liam Turner", actual.Name)
            End Using
        End Sub

        <TestMethod>
        Public Sub ShouldFakeAddingPatientWithoutWritingToDatabase()
            Dim context = Mock.Create(Of HealthcareContext)()
            Dim patients = New List(Of Patient)()
            Dim patientSet = Mock.Create(Of DbSet(Of Patient))()
            Dim patient = New Patient With {
                .Id = 4,
                .Name = "Noah Williams",
                .Department = "Pediatrics",
                .DoctorId = 30,
                .IsActive = True
            }

            Mock.Arrange(Function() context.Patients).Returns(patientSet)
            Mock.Arrange(Function() patientSet.Add(patient)).
                DoInstead(Sub() patients.Add(patient))
            Mock.Arrange(Function() context.SaveChanges()).DoNothing()

            Dim result = New PatientWriter(context).Add(patient)

            Assert.AreEqual(0, result)
            Assert.AreEqual(1, patients.Count)
            Assert.AreSame(patient, patients(0))
        End Sub
    End Class

    Public Class HealthcareContext
        Inherits DbContext

        Public Sub New()
        End Sub

        Public Sub New(options As DbContextOptions(Of HealthcareContext))
            MyBase.New(options)
        End Sub

        Public Overridable Property Patients As DbSet(Of Patient)

        Public Overridable Property Doctors As DbSet(Of Doctor)

        Public Shared Function CreateInMemory() As HealthcareContext
            Dim options = New DbContextOptionsBuilder(Of HealthcareContext)().
                UseInMemoryDatabase(Guid.NewGuid().ToString()).
                Options

            Return New HealthcareContext(options)
        End Function
    End Class

    Public Class PatientDirectory
        Private ReadOnly context As HealthcareContext

        Public Sub New(context As HealthcareContext)
            Me.context = context
        End Sub

        Public Function FindActivePatient(department As String) As Patient
            Return context.Patients.
                Where(Function(patient) patient.Department = department AndAlso patient.IsActive).
                OrderBy(Function(patient) patient.Id).
                FirstOrDefault()
        End Function
    End Class

    Public Class PatientWriter
        Private ReadOnly context As HealthcareContext

        Public Sub New(context As HealthcareContext)
            Me.context = context
        End Sub

        Public Function Add(patient As Patient) As Integer
            context.Patients.Add(patient)
            Return context.SaveChanges()
        End Function
    End Class

    Public Class Patient
        Public Property Id As Integer
        Public Property Name As String
        Public Property Department As String
        Public Property DoctorId As Integer
        Public Property IsActive As Boolean
    End Class

    Public Class Doctor
        Public Property Id As Integer
        Public Property Name As String
        Public Property Specialty As String
    End Class
End Namespace
