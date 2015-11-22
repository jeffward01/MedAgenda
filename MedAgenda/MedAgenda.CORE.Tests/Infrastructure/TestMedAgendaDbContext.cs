using MedAgenda.CORE.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedAgenda.CORE.Domain;
using System.Data.Entity;
using MedAgenda.CORE.Tests.MockDbSets;

namespace MedAgenda.CORE.Tests.Infrastructure
{
    public class TestMedAgendaDbContext : IMedAgendaDbContext
    {
        public TestMedAgendaDbContext()
        {
            Appointments = new TestAppointmentsDbSet();
            DoctorChecks = new TestDoctorChecksDbSet();
            Doctors = new TestDoctorsDbSet();
            EmergencyContacts = new TestEmergencyContactsDbSet();
            ExamRooms = new TestExamRoomsDbSet();
            PatientChecks = new TestPatientChecksDbSet();
            Patients = new TestPatientsDbSet();
            Specialties = new TestSpecialtiesDbSet();
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
