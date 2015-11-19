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
            Patient dbPatient = db.Patients.Find(check.PatientID);

            Specialty preferredSpecialty = db.Specialties.FirstOrDefault(d => d.SpecialtyID == check.SpecialtyID);

            Doctor doctor = null;

            if (dbPatient.Age < 16)
            {
                doctor = findDoctorBySpecialty("Pediatrics");
            }
            else
            {
                doctor = findDoctorBySpecialty(preferredSpecialty.SpecialtyName);
            }

            if (doctor == null)
            {
                doctor = findDoctorBySpecialty("General Practice");

                if (doctor == null)
                {
                    doctor = findCheckedInDoctor();
                }
            }

            ExamRoom examRoom = examRoomDoctorMatcher(doctor);

            var appointment = new Appointment
            {
                DoctorID = doctor.DoctorID,
                PatientID = dbPatient.PatientID,
                ExamRoomID = examRoom.ExamRoomID,
                CheckinDateTime = check.CheckinDateTime
            };

            db.Appointments.Add(appointment);
           
            return Mapper.Map<AppointmentModel>(appointment);
        }

        /// <summary>
        /// Returns a doctor with the given specialty name
        /// </summary>
        /// <param name="specialtyName"></param>
        /// <returns></returns>
        private Doctor findDoctorBySpecialty(string specialtyName)
        {
            var specialty = db.Specialties.FirstOrDefault(d => d.SpecialtyName == specialtyName);

            var doctor = db.Doctors.Where(d => d.SpecialtyID == specialty.SpecialtyID &&
                                                              d.IsCheckedIn).OrderBy(a => a.UpcomingAppointmentCount).FirstOrDefault();
            return doctor;
        }

        /// <summary>
        /// Finds a checked in doctor with the smallest amount of upcoming appointments
        /// </summary>
        /// <returns></returns>
        private Doctor findCheckedInDoctor()
        {
            return db.Doctors.Where(d => d.IsCheckedIn == true)
                             .OrderBy(ac => ac.UpcomingAppointmentCount)
                             .FirstOrDefault();
        }
        
        /// <summary>
        /// Finds an exam room based on a doctors preference - or the next available exam room.
        /// </summary>
        /// <param name="doctor"></param>
        /// <returns></returns>
        private ExamRoom examRoomDoctorMatcher(Doctor doctor)
        {
            var currentCheckin = db.DoctorChecks
                                   .Where(dc => dc.DoctorID == doctor.DoctorID && 
                                                !dc.CheckoutDateTime.HasValue)
                                   .LastOrDefault();

            if (upcomingAppointmentsForExamRoom(currentCheckin.ExamRoomID) > 0)
            {
                return findAvailableExamRoom();
            }
            else
            {
                return currentCheckin.ExamRoom;
            }
        }

        /// <summary>
        /// Find exam room with least number of upcoming appointments
        /// </summary>
        /// <returns></returns>
        private ExamRoom findAvailableExamRoom()
        {
            // exam rooms ordered by number of upcoming appointments

            return db.ExamRooms
                    .OrderBy(e => e.Appointments.Count(a => !a.CheckoutDateTime.HasValue))
                    .FirstOrDefault();
        }

        /// <summary>
        /// Returns the number of upcoming appointments for an exam room
        /// </summary>
        /// <param name="examRoomID"></param>
        /// <returns></returns>
        private int upcomingAppointmentsForExamRoom(int examRoomID)
        {
           return db.Appointments
                    .Count(ex => ex.ExamRoomID == examRoomID && 
                                 !ex.CheckoutDateTime.HasValue);

        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
