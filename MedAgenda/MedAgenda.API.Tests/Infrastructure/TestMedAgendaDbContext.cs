using MedAgenda.CORE.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedAgenda.CORE.Domain;
using System.Data.Entity;

namespace MedAgenda.API.Tests.Infrastructure
{
    public class TestMedAgendaDbContext : IMedAgendaDbContext
    {
        public TestMedAgendaDbContext()
        {
            Appointments = new TestDbSet<Appointment>();
            DoctorChecks = new TestDbSet<DoctorCheck>();
            Doctors = new TestDbSet<Doctor>();
            EmergencyContacts = new TestDbSet<EmergencyContact>();
            ExamRooms = new TestDbSet<ExamRoom>();
            PatientChecks = new TestDbSet<PatientCheck>();
            Patients = new TestDbSet<Patient>();
            Specialties = new TestDbSet<Specialty>();
        }

        public IDbSet<Appointment> Appointments { get; set; }

        public IDbSet<DoctorCheck> DoctorChecks { get; set; }

        public IDbSet<Doctor> Doctors { get; set; }

        public IDbSet<EmergencyContact> EmergencyContacts { get; set; }

        public IDbSet<ExamRoom> ExamRooms { get; set; }

        public IDbSet<PatientCheck> PatientChecks { get; set; }

        public IDbSet<Patient> Patients { get; set; }

        public IDbSet<Specialty> Specialties { get; set; }

        public void Dispose()
        {
                
        }

        public int SaveChangesCount { get; private set; }
        public int SaveChanges()
        {
            SaveChangesCount++;

            return 1;
        }
    }
}
