using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.NonElevatedExamples.AdvancedUsage.MockingDbContextSeams
{
    /// <summary>
    /// JustMock Lite keeps healthcare data access testable through explicit virtual or
    /// interface seams without elevated interception.
    /// </summary>
    [TestClass]
    public class MockingDbContextSeams_Tests
    {
        [TestMethod]
        public void ShouldUseVirtualContextSeamForPatientQueries()
        {
            using (var backingContext = SeamHealthcareContext.CreateInMemory("MockingDbContextSeams.Virtual"))
            {
                backingContext.Patients.AddRange(
                    new SeamPatient { Id = 1, Name = "Olivia Carter", Department = "Cardiology", IsActive = false },
                    new SeamPatient { Id = 2, Name = "Liam Turner", Department = "Cardiology", IsActive = true });
                backingContext.SaveChanges();

                var context = Mock.Create<SeamHealthcareContext>();
                Mock.Arrange(() => context.Patients).Returns(backingContext.Patients);

                var actual = new VirtualPatientReader(context).FindActivePatient();

                Assert.IsNotNull(actual);
                Assert.AreEqual("Liam Turner", actual.Name);
            }
        }

        [TestMethod]
        public void ShouldReturnFakePatientCollectionForFutureInstance()
        {
            using (var backingContext = SeamHealthcareContext.CreateInMemory("MockingDbContextSeams.Future"))
            {
                backingContext.Patients.Add(
                    new SeamPatient { Id = 4, Name = "Mia Chen", Department = "Neurology", IsActive = true });
                backingContext.SaveChanges();

                var context = Mock.Create<FuturePatientContext>();
                Mock.Arrange(() => context.GetPatients())
                    .IgnoreInstance()
                    .Returns(backingContext.Patients);

                var actual = new FuturePatientReader(Mock.Create<FuturePatientContext>).GetById(4);

                Assert.AreEqual("Mia Chen", actual.Name);
            }
        }

        [TestMethod]
        public void ShouldUseInterfaceContextSeamForPatientWrites()
        {
            using (var backingContext = SeamHealthcareContext.CreateInMemory("MockingDbContextSeams.Interface"))
            {
                var context = Mock.Create<ISeamPatientContext>();
                Mock.Arrange(() => context.Patients).Returns(backingContext.Patients);
                Mock.Arrange(() => context.SaveChanges())
                    .Returns(1)
                    .MustBeCalled();

                var result = new InterfacePatientWriter(context).Add(new SeamPatient
                {
                    Id = 3,
                    Name = "Noah Williams",
                    Department = "Pediatrics",
                    IsActive = true
                });

                Assert.AreEqual(1, result);
                Assert.AreEqual(1, backingContext.Patients.Local.Count);
                Mock.Assert(context);
            }
        }
    }

    public class SeamHealthcareContext : DbContext
    {
        public SeamHealthcareContext()
        {
        }

        public SeamHealthcareContext(DbContextOptions<SeamHealthcareContext> options)
            : base(options)
        {
        }

        public virtual DbSet<SeamPatient> Patients { get; set; }

        public virtual DbSet<SeamDoctor> Doctors { get; set; }

        public static SeamHealthcareContext CreateInMemory(string databaseName)
        {
            var options = new DbContextOptionsBuilder<SeamHealthcareContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;
            var context = new SeamHealthcareContext(options);

            context.Database.EnsureDeleted();
            return context;
        }
    }

    public interface ISeamPatientContext
    {
        DbSet<SeamPatient> Patients { get; }

        int SaveChanges();
    }

    public class VirtualPatientReader
    {
        private readonly SeamHealthcareContext context;

        public VirtualPatientReader(SeamHealthcareContext context)
        {
            this.context = context;
        }

        public SeamPatient FindActivePatient()
        {
            return context.Patients
                .Where(patient => patient.IsActive)
                .OrderBy(patient => patient.Id)
                .FirstOrDefault();
        }
    }

    public class FuturePatientReader
    {
        private readonly Func<FuturePatientContext> createContext;

        public FuturePatientReader(Func<FuturePatientContext> createContext)
        {
            this.createContext = createContext;
        }

        public SeamPatient GetById(int patientId)
        {
            var context = this.createContext();
            return context.GetPatients()
                .Where(patient => patient.Id == patientId)
                .Single();
        }
    }

    public class FuturePatientContext
    {
        public virtual IQueryable<SeamPatient> GetPatients()
        {
            return null;
        }
    }

    public class InterfacePatientWriter
    {
        private readonly ISeamPatientContext context;

        public InterfacePatientWriter(ISeamPatientContext context)
        {
            this.context = context;
        }

        public int Add(SeamPatient patient)
        {
            context.Patients.Add(patient);
            return context.SaveChanges();
        }
    }

    public class SeamPatient
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Department { get; set; }

        public bool IsActive { get; set; }
    }

    public class SeamDoctor
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Specialty { get; set; }
    }
}
