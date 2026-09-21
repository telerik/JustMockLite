Imports System
Imports System.Linq
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Telerik.JustMock

Namespace JustMock.NonElevatedExamples.AdvancedUsage.MockingDbSetQueries
    ''' <summary>
    ''' A DbSet can be arranged to an EF Core in-memory patient set so LINQ operators execute normally.
    ''' </summary>
    <TestClass>
    Public Class MockingDbSetQueries_Tests
        <TestMethod>
        Public Sub ShouldReturnFakePatientCollectionForQuery()
            Using backingContext = HealthcareContext.CreateInMemory()
                backingContext.Patients.AddRange(
                    New Patient With {.Id = 1, .Name = "Olivia Carter", .Department = "Cardiology", .DoctorId = 10, .IsActive = True},
                    New Patient With {.Id = 2, .Name = "Liam Turner", .Department = "Cardiology", .DoctorId = 10, .IsActive = False},
                    New Patient With {.Id = 3, .Name = "Noah Williams", .Department = "Neurology", .DoctorId = 20, .IsActive = True})
                backingContext.SaveChanges()

                Dim context = Mock.Create(Of HealthcareContext)()
                Mock.Arrange(Function() context.Patients).Returns(backingContext.Patients)

                Dim actual = New PatientDirectory(context).GetActiveNames("Cardiology")

                CollectionAssert.AreEqual(New String() {"Olivia Carter"}, actual)
            End Using
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

        Public Function GetActiveNames(department As String) As String()
            Return context.Patients.
                AsNoTracking().
                Where(Function(patient) patient.Department = department AndAlso patient.IsActive).
                OrderBy(Function(patient) patient.Name).
                Select(Function(patient) patient.Name).
                ToArray()
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
