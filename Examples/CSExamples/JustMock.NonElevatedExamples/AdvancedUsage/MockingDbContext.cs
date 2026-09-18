using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.NonElevatedExamples.AdvancedUsage.MockingDbContext
{
    /// <summary>
    /// Entity Framework Core contexts expose virtual members that can be arranged with JustMock Lite.
    /// An in-memory context supplies a real DbSet while the healthcare data access code remains isolated from the test.
    /// </summary>
    [TestClass]
    public class MockingDbContext_Tests
    {
        [TestMethod]
        public void ShouldReturnFakePatientCollectionForQuery()
        {
            using (var backingContext = HealthcareContext.CreateInMemory())
            {
                backingContext.Patients.AddRange(
                    new Patient { Id = 1, Name = "Olivia Carter", Department = "Cardiology", DoctorId = 10, IsActive = false },
                    new Patient { Id = 2, Name = "Liam Turner", Department = "Cardiology", DoctorId = 10, IsActive = true },
                    new Patient { Id = 3, Name = "Mia Chen", Department = "Neurology", DoctorId = 20, IsActive = true });
                backingContext.SaveChanges();

                var context = Mock.Create<HealthcareContext>();
                Mock.Arrange(() => context.Patients).Returns(backingContext.Patients);

                var actual = new PatientDirectory(context).FindActivePatient("Cardiology");

                Assert.IsNotNull(actual);
                Assert.AreEqual("Liam Turner", actual.Name);
            }
        }

        [TestMethod]
        public void ShouldFakeAddingPatientWithoutWritingToDatabase()
        {
            var context = Mock.Create<HealthcareContext>();
            var patients = new List<Patient>();
            var patientSet = Mock.Create<DbSet<Patient>>();
            var patient = new Patient
            {
                Id = 4,
                Name = "Noah Williams",
                Department = "Pediatrics",
                DoctorId = 30,
                IsActive = true
            };

            Mock.Arrange(() => context.Patients).Returns(patientSet);
            Mock.Arrange(() => patientSet.Add(patient))
                .DoInstead(() => patients.Add(patient));
            Mock.Arrange(() => context.SaveChanges()).DoNothing();

            var result = new PatientWriter(context).Add(patient);

            Assert.AreEqual(0, result);
            Assert.AreEqual(1, patients.Count);
            Assert.AreSame(patient, patients[0]);
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

        public Patient FindActivePatient(string department)
        {
            return this.context.Patients
                .Where(patient => patient.Department == department && patient.IsActive)
                .OrderBy(patient => patient.Id)
                .FirstOrDefault();
        }
    }

    public class PatientWriter
    {
        private readonly HealthcareContext context;

        public PatientWriter(HealthcareContext context)
        {
            this.context = context;
        }

        public int Add(Patient patient)
        {
            this.context.Patients.Add(patient);
            return this.context.SaveChanges();
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
