using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.NonElevatedExamples.AdvancedUsage.MockingDbSetQueries
{
    /// <summary>
    /// A DbSet can be arranged to an EF Core in-memory patient set so LINQ operators execute normally.
    /// </summary>
    [TestClass]
    public class MockingDbSetQueries_Tests
    {
        [TestMethod]
        public void ShouldReturnFakePatientCollectionForQuery()
        {
            using (var backingContext = HealthcareContext.CreateInMemory())
            {
                backingContext.Patients.AddRange(
                    new Patient { Id = 1, Name = "Olivia Carter", Department = "Cardiology", DoctorId = 10, IsActive = true },
                    new Patient { Id = 2, Name = "Liam Turner", Department = "Cardiology", DoctorId = 10, IsActive = false },
                    new Patient { Id = 3, Name = "Noah Williams", Department = "Neurology", DoctorId = 20, IsActive = true });
                backingContext.SaveChanges();

                var context = Mock.Create<HealthcareContext>();
                Mock.Arrange(() => context.Patients).Returns(backingContext.Patients);

                var actual = new PatientDirectory(context).GetActiveNames("Cardiology");

                CollectionAssert.AreEqual(new[] { "Olivia Carter" }, actual);
            }
        }
    }

    public class HealthcareContext : DbContext
    {
        public HealthcareContext()
        {
        }

        public HealthcareContext(DbContextOptions<HealthcareContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Patient> Patients { get; set; }

        public virtual DbSet<Doctor> Doctors { get; set; }

        public static HealthcareContext CreateInMemory()
        {
            var options = new DbContextOptionsBuilder<HealthcareContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new HealthcareContext(options);
        }
    }

    public class PatientDirectory
    {
        private readonly HealthcareContext context;

        public PatientDirectory(HealthcareContext context)
        {
            this.context = context;
        }

        public string[] GetActiveNames(string department)
        {
            return this.context.Patients
                .AsNoTracking()
                .Where(patient => patient.Department == department && patient.IsActive)
                .OrderBy(patient => patient.Name)
                .Select(patient => patient.Name)
                .ToArray();
        }
    }

    public class Patient
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Department { get; set; }

        public int DoctorId { get; set; }

        public bool IsActive { get; set; }
    }

    public class Doctor
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Specialty { get; set; }
    }
}
