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
        private readonly IMedAgendaDbContext db;

        public AppointmentScheduler(IMedAgendaDbContext context)
        {
            db = context;
        }

        public Appointment CreateAppointment(int patientCheckId)
        {
            PatientCheck check = db.PatientChecks.Find(patientCheckId);

            Patient dbPatient = db.Patients.Find(check.PatientID);

            Specialty preferredSpecialty = db.Specialties.FirstOrDefault(d => d.SpecialtyID == check.SpecialtyID);

            Doctor doctor = null;
            Appointment appointment = null;

            // If patient is under 16, search for pediatrician,
            //  otherwise look for requested specialty
            if (dbPatient.Age < 16)
            {
                doctor = findDoctorBySpecialty("Pediatrics");
            }
            else
            {
                doctor = findDoctorBySpecialty(preferredSpecialty.SpecialtyName);
            }

            // If no doctor found, search for a GP;
            //  if none found, search for any available doctor
            if (doctor == null)
            {
                doctor = findDoctorBySpecialty("General Practice");

                if (doctor == null)
                {
                    doctor = findCheckedInDoctor(dbPatient);
                }
            }

            // If doctor was found, get the exam room for the appointment
            if (doctor != null)
            {
                ExamRoom examRoom = examRoomDoctorMatcher(doctor);

                // If exam room was found, create the appointment
                if (examRoom != null)
                {
                    appointment = new Appointment
                    {
                        DoctorID = doctor.DoctorID,
                        PatientID = dbPatient.PatientID,
                        ExamRoomID = examRoom.ExamRoomID,
                        CheckinDateTime = check.CheckinDateTime
                    };

                    db.Appointments.Add(appointment);

                    db.SaveChanges();
                }
            } 
                      
            return appointment;
        }

        /// <summary>
        /// Returns a doctor with the given specialty name
        /// </summary>
        /// <param name="specialtyName"></param>
        /// <returns></returns>
        private Doctor findDoctorBySpecialty(string specialtyName)
        {
            var specialty = db.Specialties.FirstOrDefault(d => d.SpecialtyName == specialtyName);
            Doctor doctor = null;

            // If specialty was found in specialty table, then search for checked-in doctor
            //   with that specialty with lowest number of upcoming appointments
            if (specialty != null)
            {
                //TODO: Replace this with stored procedure
                doctor = db.Doctors.ToList().Where(d => d.SpecialtyID == specialty.SpecialtyID &&
                                                  d.IsCheckedIn)
                                      .OrderBy(a => a.UpcomingAppointmentCount)
                                      .FirstOrDefault();

            }
            return doctor;
        }

        /// <summary>
        /// Finds a checked in doctor with the smallest amount of upcoming appointments
        /// </summary>
        /// <returns></returns>
        private Doctor findCheckedInDoctor(Patient patient)
        {
            Doctor doctor = null;

            //TODO: Replace this with stored procedure
            doctor = db.Doctors.ToList().Where(d => d.IsCheckedIn == true)
                             .OrderBy(ac => ac.UpcomingAppointmentCount)
                             .FirstOrDefault();
            if (doctor != null)
            {
                // If patient is 16 or over, do not assign a pediatrician unless 
                //   there are no other checked-in doctors with a different specialty
                if (patient.Age >= 16 && doctor.Specialty.SpecialtyName == "Pediatrics")
                {
                    //TODO: Replace this with stored procedure
                    var newDoctor = db.Doctors.ToList().Where(d => d.IsCheckedIn == true && d.Specialty.SpecialtyName != "Pediatrics")
                                                .OrderBy(ac => ac.UpcomingAppointmentCount)
                                                .FirstOrDefault();
                    if (newDoctor != null)
                    {
                        doctor = newDoctor;
                    }
                }
            }

            return doctor;
        }

        /// <summary>
        /// Finds an exam room based on a doctors preference - or the next available exam room.
        /// </summary>
        /// <param name="doctor"></param>
        /// <returns></returns>
        private ExamRoom examRoomDoctorMatcher(Doctor doctor)
        {
            var currentCheckin = db.DoctorChecks.ToList()
                                   .Where(dc => dc.DoctorID == doctor.DoctorID &&
                                                !dc.CheckoutDateTime.HasValue)
                                   .LastOrDefault();
            // If no checkin records found, look for any exam room
            if (currentCheckin == null)
            {
                return findAvailableExamRoom();
            }
            else
            // Return the preferred exam room if it has no upcoming appointments
            {
                if (upcomingAppointmentsForExamRoom(currentCheckin.ExamRoomID) == 0)
                {
                    return currentCheckin.ExamRoom;
                }
                else
                {
                    // Find the exam room with the fewest upcoming appointments,
                    //  excluding the preferred exam room, but if no exam room found,
                     //  then search for any available exam room
                    var examRoom = findAvailableExamRoomExclude(currentCheckin.ExamRoomID);
                    if (examRoom != null)
                    {
                        return examRoom;                        
                    }
                    else
                    {
                        return findAvailableExamRoom();
                    }
                }
            }
        }

        /// <summary>
        /// Find any exam room with least number of upcoming appointments
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
        /// Find exam room other than input exam room with least number of upcoming appointments
        /// </summary>
        /// <param name="examRoomID"></param>
        /// <returns></returns>
        private ExamRoom findAvailableExamRoomExclude(int examRoomID)
        {
            // exam rooms ordered by number of upcoming appointments
            var examRooms = db.ExamRooms.Where(e => e.ExamRoomID != examRoomID)
                    .OrderBy(e => e.Appointments.Count(a => !a.CheckoutDateTime.HasValue));

            var examRoom = examRooms.FirstOrDefault();

            return db.ExamRooms.Where(e => e.ExamRoomID != examRoomID)
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
