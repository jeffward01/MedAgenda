using MedAgenda.CORE.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedAgenda.CORE.Infrastructure
{
    public interface IMedAgendaDbContext : IDisposable
    {
        IDbSet<Appointment> Appointments { get; set; }
        IDbSet<Doctor> Doctors { get; set; }
        IDbSet<DoctorCheck> DoctorChecks { get; set; }
        IDbSet<EmergencyContact> EmergencyContacts { get; set; }
        IDbSet<ExamRoom> ExamRooms { get; set; }
        IDbSet<Patient> Patients { get; set; }
        IDbSet<PatientCheck> PatientChecks { get; set; }
        IDbSet<Specialty> Specialties { get; set; }

        int SaveChanges();
    }
}
