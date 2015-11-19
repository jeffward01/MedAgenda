using AutoMapper;
using MedAgenda.CORE.Domain;
using MedAgenda.CORE.Infrastructure;
using MedAgenda.CORE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedAgenda.CORE.Services
{
    public class AppointmentScheduler : IDisposable
    {
        private MedAgendaDbContext db = new MedAgendaDbContext();

        public AppointmentModel CreateAppointment(PatientCheckModel check)
        {
            Appointment appointment = null;
            var dbPatient = db.Patients.Find(check.PatientID);

            if (dbPatient.Age < 16)
            {
                var specialty = db.Specialties.FirstOrDefault(d => d.SpecialtyName == "Pediatrics");

                var pediatrician = db.Doctors.FirstOrDefault(d => d.SpecialtyID == specialty.SpecialtyID &&
                                                                  d.IsCheckedIn);
                if (pediatrician != null)
                {
                    int examRoomID = ExamRoomDoctorMatcher(pediatrician);
                } // if inside an if ?? <------ 
                else if (FindGeneralPractitioner != null) // Will return record of General Practitioner
                {
                    // if is a general pract do this, if not then pick out any doctor
                    // Least amount of appointments method
                    //
                }

                // yak shaving.. logic.. etc

                appointment = new Appointment
                {
                    DoctorID = pediatrician.DoctorID,
                    PatientID = dbPatient.PatientID
                };

                db.Appointments.Add(appointment);
            }
            else
            {
                
            }
            return Mapper.Map<AppointmentModel>(appointment);
        }

        private int ExamRoomDoctorMatcher(Doctor pediatrician)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
