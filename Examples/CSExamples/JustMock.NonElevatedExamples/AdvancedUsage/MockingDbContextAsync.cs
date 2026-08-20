using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.NonElevatedExamples.AdvancedUsage.MockingDbContextAsync
{
    /// <summary>
    /// Virtual EF Core members can be arranged with JustMock Lite while asynchronous
    /// healthcare queries and persistence use the EF Core InMemory provider.
    /// </summary>
    [TestClass]
    public class MockingDbContextAsync_Tests
    {
        [TestMethod]
        public async Task ShouldQueryArrangedVirtualPatientSetAsynchronously()
        {
            using (var backingContext = AsyncHealthcareContext.CreateInMemory("MockingDbContextAsync.Query"))
            {
                backingContext.Patients.AddRange(
                    new AsyncPatient { Id = 1, Name = "Olivia Carter", Department = "Cardiology", DoctorId = 10, IsActive = true },
                    new AsyncPatient { Id = 2, Name = "Liam Turner", Department = "Cardiology", DoctorId = 10, IsActive = false },
                    new AsyncPatient { Id = 3, Name = "Maya Patel", Department = "Cardiology", DoctorId = 20, IsActive = true },
                    new AsyncPatient { Id = 4, Name = "Noah Williams", Department = "Pediatrics", DoctorId = 30, IsActive = true });
                await backingContext.SaveChangesAsync();

                var context = Mock.Create<AsyncHealthcareContext>();
                Mock.Arrange(() => context.Patients).Returns(backingContext.Patients);

                var actual = await new AsyncPatientDirectory(context).FindActiveNamesAsync("Cardiology");

                CollectionAssert.AreEqual(new[] { "Maya Patel", "Olivia Carter" }, actual);
            }
        }

        [TestMethod]
        public async Task ShouldSavePatientThroughArrangedVirtualContextAsynchronously()
        {
            using (var backingContext = AsyncHealthcareContext.CreateInMemory("MockingDbContextAsync.Save"))
            {
                var context = Mock.Create<AsyncHealthcareContext>();
                Mock.Arrange(() => context.Patients).Returns(backingContext.Patients);
                Mock.Arrange(() => context.SaveChangesAsync(Arg.IsAny<CancellationToken>()))
                    .ReturnsAsync(1)
                    .MustBeCalled();

                var result = await new AsyncPatientWriter(context).AddAsync(new AsyncPatient
                {
                    Id = 5,
                    Name = "Ava Brooks",
                    Department = "Pediatrics",
                    DoctorId = 30,
                    IsActive = true
                });

                Assert.AreEqual(1, result);
                Assert.AreEqual(1, backingContext.Patients.Local.Count);
                Mock.Assert(context);
            }
        }

        [TestMethod]
        public async Task ShouldVerifyAsyncPatientSaveOccurrence()
        {
            using (var backingContext = AsyncHealthcareContext.CreateInMemory("MockingDbContextAsync.Occurrence"))
            {
                var context = Mock.Create<AsyncHealthcareContext>();
                Mock.Arrange(() => context.Patients).Returns(backingContext.Patients);
                Mock.Arrange(() => context.SaveChangesAsync(Arg.IsAny<CancellationToken>()))
                    .ReturnsAsync(1);

                var writer = new AsyncPatientWriter(context);
                await writer.AddAsync(new AsyncPatient
                {
                    Id = 6,
                    Name = "Ethan Clark",
                    Department = "Cardiology",
                    DoctorId = 10,
                    IsActive = true
                });
                await writer.AddAsync(new AsyncPatient
                {
                    Id = 7,
                    Name = "Sofia Green",
                    Department = "Neurology",
                    DoctorId = 20,
                    IsActive = true
                });

                Mock.Assert(
                    () => context.SaveChangesAsync(Arg.IsAny<CancellationToken>()),
                    Occurs.Exactly(2));
            }
        }
    }

    public class AsyncHealthcareContext : DbContext
    {
        public AsyncHealthcareContext()
        {
        }

        public AsyncHealthcareContext(DbContextOptions<AsyncHealthcareContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AsyncPatient> Patients { get; set; }

        public virtual DbSet<AsyncDoctor> Doctors { get; set; }

        public static AsyncHealthcareContext CreateInMemory(string databaseName)
        {
            var options = new DbContextOptionsBuilder<AsyncHealthcareContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;
            var context = new AsyncHealthcareContext(options);

            context.Database.EnsureDeleted();
            return context;
        }
    }

    public class AsyncPatientDirectory
    {
        private readonly AsyncHealthcareContext context;

        public AsyncPatientDirectory(AsyncHealthcareContext context)
        {
            this.context = context;
        }

        public async Task<string[]> FindActiveNamesAsync(
            string department,
            CancellationToken cancellationToken = default)
        {
            return await context.Patients
                .AsNoTracking()
                .Where(patient => patient.Department == department && patient.IsActive)
                .OrderBy(patient => patient.Name)
                .Select(patient => patient.Name)
                .ToArrayAsync(cancellationToken);
        }
    }

    public class AsyncPatientWriter
    {
        private readonly AsyncHealthcareContext context;

        public AsyncPatientWriter(AsyncHealthcareContext context)
        {
            this.context = context;
        }

        public async Task<int> AddAsync(
            AsyncPatient patient,
            CancellationToken cancellationToken = default)
        {
            await context.Patients.AddAsync(patient, cancellationToken);
            return await context.SaveChangesAsync(cancellationToken);
        }
    }

    public class AsyncPatient
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Department { get; set; }

        public int DoctorId { get; set; }

        public bool IsActive { get; set; }
    }

    public class AsyncDoctor
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Specialty { get; set; }
    }
}
