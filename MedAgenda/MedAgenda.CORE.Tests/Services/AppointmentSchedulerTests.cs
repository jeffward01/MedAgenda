using MedAgenda.CORE.Domain;
using MedAgenda.CORE.Infrastructure;
using MedAgenda.CORE.Services;
using MedAgenda.CORE.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedAgenda.CORE.Tests.Services
{
    [TestClass]
    public class AppointmentSchedulerTests
    {
        [TestMethod]
        public void PatientUnder16GetsPediatrician()
        {
            using (IMedAgendaDbContext db = new TestMedAgendaDbContext())
            {
                //Arrange
                #region Add some specialties
                Specialty pediatricSpecialty = db.Specialties.Add(new Specialty {
                    SpecialtyID = 1, SpecialtyName = "Pediatrics"
                });

                Specialty armChopOff = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = 2,
                    SpecialtyName = "Arm Chop-off"
                });
                #endregion

                #region Add a patient under 16
                Patient patient = db.Patients.Add(new Patient {
                    PatientID = 1, Birthdate = DateTime.Now.AddYears(-8), FirstName = "Young", LastName = "Ster"
                });
                #endregion

                #region Add some doctors
                Doctor johnSmithPediatrics = db.Doctors.Add(new Doctor
                {
                    DoctorID = 1,
                    FirstName = "John",
                    LastName = "Smith",
                    SpecialtyID = pediatricSpecialty.SpecialtyID,
                    Specialty = pediatricSpecialty
                });

                Doctor juliaSmithArmChopOff = db.Doctors.Add(new Doctor
                {
                    DoctorID = 2,
                    FirstName = "Julia",
                    LastName = "Smith",
                    SpecialtyID = armChopOff.SpecialtyID,
                    Specialty = armChopOff
                });
                #endregion

                #region Add an exam room
                ExamRoom examroom = db.ExamRooms.Add(new ExamRoom { ExamRoomID = 1, ExamRoomName = "Room One" });
                #endregion

                #region Create a scenario where a pediatrician and an arm chopper are checked in

                // Check in the doctors
                var johnCheckIn = new DoctorCheck {
                    DoctorCheckID = 1,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = 1,
                    ExamRoomID = 1,
                    ExamRoom = examroom,
                    Doctor = johnSmithPediatrics
                };
                johnSmithPediatrics.DoctorChecks.Add(johnCheckIn);
                db.DoctorChecks.Add(johnCheckIn);

                var juliaCheckIn = new DoctorCheck {
                    DoctorCheckID = 2,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = 2,
                    Doctor = juliaSmithArmChopOff,
                    ExamRoomID = 1,
                    ExamRoom = examroom
                };
                juliaSmithArmChopOff.DoctorChecks.Add(juliaCheckIn);
                db.DoctorChecks.Add(juliaCheckIn);
                #endregion

                #region Check in a patient

                // Check in a patient
                PatientCheck patientcheck = db.PatientChecks.Add(new PatientCheck
                {
                    PatientCheckID = 1,
                    PatientID = 1,
                    Patient = patient,
                    CheckinDateTime = DateTime.Now,
                    SpecialtyID = 2,
                    Specialty = armChopOff
                });
                #endregion

                using (var scheduler = new AppointmentScheduler(db))
                {
                    //ACT
                    scheduler.CreateAppointment(patientcheck.PatientCheckID);

                    //ASSERT
                    Assert.IsTrue(db.Appointments.Count() > 0);
                    var appointment = db.Appointments.First();

                    Assert.IsTrue(appointment.PatientID == 1 && appointment.DoctorID == 1);
                }
            }
        }

        [TestMethod]
        public void Patient16OverDoesNotGetPediatrician()
        {
            using (IMedAgendaDbContext db = new TestMedAgendaDbContext())
            {
                #region Add a patient over 16
                Patient patient = db.Patients.Add(new Patient
                {
                    PatientID = 1,
                    Birthdate = DateTime.Now.AddYears(-78),
                    FirstName = "Old",
                    LastName = "Man"
                });
                #endregion

                #region Add some specialties
                Specialty pediatricSpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = 1,
                    SpecialtyName = "Pediatrics"
                });

                Specialty neurologistSpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = 2,
                    SpecialtyName = "Neurologist"
                });

                Specialty surgeonSpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = 3,
                    SpecialtyName = "Surgeon"
                });
                #endregion

                #region Add some doctors
                Doctor johnSmithPediatrics = db.Doctors.Add(new Doctor
                {
                    DoctorID = 1,
                    FirstName = "John",
                    LastName = "Smith",
                    SpecialtyID = pediatricSpecialty.SpecialtyID,
                    Specialty = pediatricSpecialty
                });

                Doctor juliaSmithSurgeon = db.Doctors.Add(new Doctor
                {
                    DoctorID = 2,
                    FirstName = "Julia",
                    LastName = "Smith",
                    SpecialtyID = surgeonSpecialty.SpecialtyID,
                    Specialty = surgeonSpecialty
                });
                #endregion

                #region Add an Exam Room

                ExamRoom examRoom1 = db.ExamRooms.Add(new ExamRoom
                {
                    ExamRoomID = 1,
                    ExamRoomName = "ExamRoom 1"
                });
                #endregion

                #region Check in Doctors
                // Check in the Pediatrician
                var johnCheckIn = new DoctorCheck
                {
                    DoctorCheckID = 1,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = 1,
                    ExamRoomID = 1,
                    ExamRoom = examRoom1,
                    Doctor = johnSmithPediatrics
                };
                // Check in the Surgeon
                var juliaCheckIn = new DoctorCheck
                {
                    DoctorCheckID = 2,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = 2,
                    ExamRoomID = 1,
                    ExamRoom = examRoom1,
                    Doctor = juliaSmithSurgeon
                };
                #endregion
            }
        }

    }


}
