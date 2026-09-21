Imports System
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Telerik.JustMock

Namespace JustMock.NonElevatedExamples.AdvancedUsage.MockingDbContextAsync
    ''' <summary>
    ''' Overridable EF Core members can be arranged with JustMock Lite while asynchronous
    ''' healthcare queries and persistence use the EF Core InMemory provider.
    ''' </summary>
    <TestClass>
    Public Class MockingDbContextAsync_Tests
        <TestMethod>
        Public Async Function ShouldQueryArrangedVirtualPatientSetAsynchronously() As Task
            Using backingContext = AsyncHealthcareContext.CreateInMemory("MockingDbContextAsync.Query")
                backingContext.Patients.AddRange(
                    New AsyncPatient With {.Id = 1, .Name = "Olivia Carter", .Department = "Cardiology", .DoctorId = 10, .IsActive = True},
                    New AsyncPatient With {.Id = 2, .Name = "Liam Turner", .Department = "Cardiology", .DoctorId = 10, .IsActive = False},
                    New AsyncPatient With {.Id = 3, .Name = "Maya Patel", .Department = "Cardiology", .DoctorId = 20, .IsActive = True},
                    New AsyncPatient With {.Id = 4, .Name = "Noah Williams", .Department = "Pediatrics", .DoctorId = 30, .IsActive = True})
                Await backingContext.SaveChangesAsync()

                Dim context = Mock.Create(Of AsyncHealthcareContext)()
                Mock.Arrange(Function() context.Patients).Returns(backingContext.Patients)

                Dim actual = Await New AsyncPatientDirectory(context).FindActiveNamesAsync("Cardiology")

                CollectionAssert.AreEqual(New String() {"Maya Patel", "Olivia Carter"}, actual)
            End Using
        End Function

        <TestMethod>
        Public Async Function ShouldSavePatientThroughArrangedVirtualContextAsynchronously() As Task
            Using backingContext = AsyncHealthcareContext.CreateInMemory("MockingDbContextAsync.Save")
                Dim context = Mock.Create(Of AsyncHealthcareContext)()
                Mock.Arrange(Function() context.Patients).Returns(backingContext.Patients)
                Mock.Arrange(Function() context.SaveChangesAsync(Arg.IsAny(Of CancellationToken)())).
                    ReturnsAsync(1).
                    MustBeCalled()

                Dim result = Await New AsyncPatientWriter(context).AddAsync(New AsyncPatient With {
                    .Id = 5,
                    .Name = "Ava Brooks",
                    .Department = "Pediatrics",
                    .DoctorId = 30,
                    .IsActive = True
                })

                Assert.AreEqual(1, result)
                Assert.AreEqual(1, backingContext.Patients.Local.Count)
                Mock.Assert(context)
            End Using
        End Function

        <TestMethod>
        Public Async Function ShouldVerifyAsyncPatientSaveOccurrence() As Task
            Using backingContext = AsyncHealthcareContext.CreateInMemory("MockingDbContextAsync.Occurrence")
                Dim context = Mock.Create(Of AsyncHealthcareContext)()
                Mock.Arrange(Function() context.Patients).Returns(backingContext.Patients)
                Mock.Arrange(Function() context.SaveChangesAsync(Arg.IsAny(Of CancellationToken)())).
                    ReturnsAsync(1)

                Dim writer = New AsyncPatientWriter(context)
                Await writer.AddAsync(New AsyncPatient With {
                    .Id = 6,
                    .Name = "Ethan Clark",
                    .Department = "Cardiology",
                    .DoctorId = 10,
                    .IsActive = True
                })
                Await writer.AddAsync(New AsyncPatient With {
                    .Id = 7,
                    .Name = "Sofia Green",
                    .Department = "Neurology",
                    .DoctorId = 20,
                    .IsActive = True
                })

                Mock.Assert(
                    Function() context.SaveChangesAsync(Arg.IsAny(Of CancellationToken)()),
                    Occurs.Exactly(2))
            End Using
        End Function
    End Class

    Public Class AsyncHealthcareContext
        Inherits DbContext

        Public Sub New()
        End Sub

        Public Sub New(options As DbContextOptions(Of AsyncHealthcareContext))
            MyBase.New(options)
        End Sub

        Public Overridable Property Patients As DbSet(Of AsyncPatient)

        Public Overridable Property Doctors As DbSet(Of AsyncDoctor)

        Public Shared Function CreateInMemory(databaseName As String) As AsyncHealthcareContext
            Dim options = New DbContextOptionsBuilder(Of AsyncHealthcareContext)().
                UseInMemoryDatabase(databaseName).
                Options
            Dim context = New AsyncHealthcareContext(options)

            context.Database.EnsureDeleted()
            Return context
        End Function
    End Class

    Public Class AsyncPatientDirectory
        Private ReadOnly context As AsyncHealthcareContext

        Public Sub New(context As AsyncHealthcareContext)
            Me.context = context
        End Sub

        Public Async Function FindActiveNamesAsync(
            department As String,
            Optional cancellationToken As CancellationToken = Nothing) As Task(Of String())

            Return Await context.Patients.
                AsNoTracking().
                Where(Function(patient) patient.Department = department AndAlso patient.IsActive).
                OrderBy(Function(patient) patient.Name).
                Select(Function(patient) patient.Name).
                ToArrayAsync(cancellationToken)
        End Function
    End Class

    Public Class AsyncPatientWriter
        Private ReadOnly context As AsyncHealthcareContext

        Public Sub New(context As AsyncHealthcareContext)
            Me.context = context
        End Sub

        Public Async Function AddAsync(
            patient As AsyncPatient,
            Optional cancellationToken As CancellationToken = Nothing) As Task(Of Integer)

            Await context.Patients.AddAsync(patient, cancellationToken)
            Return Await context.SaveChangesAsync(cancellationToken)
        End Function
    End Class

    Public Class AsyncPatient
        Public Property Id As Integer
        Public Property Name As String
        Public Property Department As String
        Public Property DoctorId As Integer
        Public Property IsActive As Boolean
    End Class

    Public Class AsyncDoctor
        Public Property Id As Integer
        Public Property Name As String
        Public Property Specialty As String
    End Class
End Namespace
