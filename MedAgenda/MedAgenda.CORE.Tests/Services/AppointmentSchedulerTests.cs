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
            int doctorIDArmChopOff = 1;
            int doctorIDPediatrics = 2;

            int doctorCheckIDArmChopOff = 1;
            int doctorCheckIDPediatrics = 2;

            int examRoomIDForTest = 1;

            int patientIDUnder16 = 1;

            int patientCheckIDUnder16 = 1;

            int specialtyIDPediatrics = 1;
            int specialtyIDArmChopOff = 2;

            using (IMedAgendaDbContext db = new TestMedAgendaDbContext())
            {
                //Arrange
                #region Add some specialties
                Specialty pediatricSpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDPediatrics,
                    SpecialtyName = "Pediatrics"
                });

                Specialty armChopOff = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDArmChopOff,
                    SpecialtyName = "Arm Chop-off"
                });
                #endregion

                #region Add a patient under 16
                Patient patient = db.Patients.Add(new Patient
                {
                    PatientID = patientIDUnder16,
                    Birthdate = DateTime.Now.AddYears(-8),
                    FirstName = "Young",
                    LastName = "Ster"
                });
                #endregion

                #region Add some doctors

                Doctor juliaSmithArmChopOff = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDArmChopOff,
                    FirstName = "Julia",
                    LastName = "Smith",
                    SpecialtyID = armChopOff.SpecialtyID,
                    Specialty = armChopOff
                });

                Doctor johnSmithPediatrics = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDPediatrics,
                    FirstName = "John",
                    LastName = "Smith",
                    SpecialtyID = pediatricSpecialty.SpecialtyID,
                    Specialty = pediatricSpecialty
                });

                #endregion

                #region Add an exam room
                ExamRoom examroom =
                    db.ExamRooms.Add(new ExamRoom
                    { ExamRoomID = examRoomIDForTest, ExamRoomName = "Room One" });
                #endregion

                #region Create a scenario where a pediatrician and an arm chopper are checked in

                // Check in the doctors
                var juliaArmChopOffCheckIn = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDArmChopOff,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = doctorIDArmChopOff,
                    Doctor = juliaSmithArmChopOff,
                    ExamRoomID = examRoomIDForTest,
                    ExamRoom = examroom
                };
                juliaSmithArmChopOff.DoctorChecks.Add(juliaArmChopOffCheckIn);
                db.DoctorChecks.Add(juliaArmChopOffCheckIn);

                var johnSmithPediatricsCheckIn = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDPediatrics,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = doctorIDPediatrics,
                    ExamRoomID = examRoomIDForTest,
                    ExamRoom = examroom,
                    Doctor = johnSmithPediatrics
                };
                johnSmithPediatrics.DoctorChecks.Add(johnSmithPediatricsCheckIn);
                db.DoctorChecks.Add(johnSmithPediatricsCheckIn);

                #endregion

                #region Check in a patient

                // Check in a patient
                PatientCheck patientcheck = db.PatientChecks.Add(new PatientCheck
                {
                    PatientCheckID = patientCheckIDUnder16,
                    PatientID = patientIDUnder16,
                    Patient = patient,
                    CheckinDateTime = DateTime.Now,
                    SpecialtyID = specialtyIDArmChopOff,
                    Specialty = armChopOff
                });

                #endregion

                using (var scheduler = new AppointmentScheduler(db))
                {
                    //ACT
                    scheduler.CreateAppointment(patientcheck.PatientCheckID);

                    //ASSERT: verify that appointment is created and
                    // patient is assigned to pediatrician
                    Assert.IsTrue(db.Appointments.Count() > 0);
                    var appointment = db.Appointments.First();

                    Assert.IsTrue(appointment.PatientID == patientIDUnder16
                                    && appointment.DoctorID == doctorIDPediatrics);
                }
            }
        }

        [TestMethod]
        public void Patient16OverDoesNotGetPediatrician()
        {
            using (IMedAgendaDbContext db = new TestMedAgendaDbContext())
            {
                int doctorIDPediatrics = 1;
                int doctorIDNeurology = 2;
                int doctorIDSurgeon = 3;
                
                int doctorCheckIDPediatrics = 1;
                int doctorCheckIDNeurology = 2;
                int doctorCheckIDSurgeon = 3;

                int examRoomIDForTest = 1;

                int patientID16Over = 1;

                int patientCheckID16Over = 1;

                int specialtyIDPediatrics = 1;
                int specialtyIDNeurology = 2;
                int specialtyIDSurgeon = 3;

                #region Add a patient over 16
                Patient patient = db.Patients.Add(new Patient
                {
                    PatientID = patientID16Over,
                    Birthdate = DateTime.Now.AddYears(-78),
                    FirstName = "Old",
                    LastName = "Man"
                });

                #endregion

                #region Add some specialties
                Specialty pediatricSpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDPediatrics,
                    SpecialtyName = "Pediatrics"
                });

                Specialty neurologistSpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDNeurology,
                    SpecialtyName = "Neurology"
                });

                Specialty surgeonSpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDSurgeon,
                    SpecialtyName = "Surgeon"
                });
                #endregion

                #region Check in a patient

                // Check in a patient
                PatientCheck patientcheck = db.PatientChecks.Add(new PatientCheck
                {
                    PatientCheckID = patientCheckID16Over,
                    PatientID = patientID16Over,
                    Patient = patient,
                    CheckinDateTime = DateTime.Now,
                    SpecialtyID = specialtyIDNeurology,
                    Specialty = neurologistSpecialty
                });


                #endregion

                #region Add some doctors
                Doctor johnSmithPediatrics = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDPediatrics,
                    FirstName = "John",
                    LastName = "Smith",
                    SpecialtyID = pediatricSpecialty.SpecialtyID,
                    Specialty = pediatricSpecialty
                });

                Doctor neurologyDoctor = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDNeurology,
                    FirstName = "Bob",
                    LastName = "Smith",
                    SpecialtyID = neurologistSpecialty.SpecialtyID,
                    Specialty = neurologistSpecialty
                });

                Doctor juliaSmithSurgeon = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDSurgeon,
                    FirstName = "Julia",
                    LastName = "Smith",
                    SpecialtyID = surgeonSpecialty.SpecialtyID,
                    Specialty = surgeonSpecialty
                });
                #endregion

                #region Add an Exam Room

                ExamRoom examRoom1 = db.ExamRooms.Add(new ExamRoom
                {
                    ExamRoomID = examRoomIDForTest,
                    ExamRoomName = "ExamRoom 1"
                });
                #endregion

                #region Check in Doctors
                // Check in the Pediatrician
                var johnSmithPediatricsCheckIn = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDPediatrics,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = doctorIDPediatrics,
                    ExamRoomID = examRoomIDForTest,
                    ExamRoom = examRoom1,
                    Doctor = johnSmithPediatrics
                };

                johnSmithPediatrics.DoctorChecks.Add(johnSmithPediatricsCheckIn);
                db.DoctorChecks.Add(johnSmithPediatricsCheckIn);

                // Check in and out the neurologist
                var neurologistCheckIn = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDNeurology,
                    CheckinDateTime = DateTime.Now.AddHours(-1),
                    CheckoutDateTime = DateTime.Now,
                    DoctorID = doctorIDNeurology,
                    ExamRoomID = examRoomIDForTest,
                    ExamRoom = examRoom1,
                    Doctor = neurologyDoctor
                };

                neurologyDoctor.DoctorChecks.Add(neurologistCheckIn);
                db.DoctorChecks.Add(neurologistCheckIn);

                // Check in the Surgeon
                var juliaSmithSurgeonCheckIn = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDSurgeon,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = doctorIDSurgeon,
                    ExamRoomID = examRoomIDForTest,
                    ExamRoom = examRoom1,
                    Doctor = juliaSmithSurgeon
                };

                juliaSmithSurgeon.DoctorChecks.Add(juliaSmithSurgeonCheckIn);
                db.DoctorChecks.Add(juliaSmithSurgeonCheckIn);
                
                #endregion

                using (var scheduler = new AppointmentScheduler(db))
                {
                    //ACT
                    scheduler.CreateAppointment(patientcheck.PatientCheckID);

                    //ASSERT
                    Assert.IsTrue(db.Appointments.Count() > 0);
                    var appointment = db.Appointments.First();

                    // Verify that the patient is assigned to surgeon, not pediatrician
                    Assert.IsTrue(appointment.PatientID == patientID16Over
                                    && appointment.DoctorID == doctorIDSurgeon);
                }
            }
        }

        [TestMethod]
        public void Patient16OverGetsRequestedSpecialty()
        {
            using (IMedAgendaDbContext db = new TestMedAgendaDbContext())
            {
                int doctorIDCardiology = 1;
                int doctorIDSurgeonReq = 2;

                int doctorCheckIDCardiology = 1;
                int doctorCheckIDSurgeonReq = 2;

                int examRoomIDForTest = 1;

                int patientID16Over = 1;

                int patientCheckID16Over = 1;

                int specialtyIDCardiology = 1;
                int specialtyIDSurgeonReq = 2;

                #region Add a patient over 16
                Patient patient = db.Patients.Add(new Patient
                {
                    PatientID = patientID16Over,
                    Birthdate = DateTime.Now.AddYears(-78),
                    FirstName = "Old",
                    LastName = "Man"
                });


                #endregion

                #region Add some specialties
                Specialty pediatricSpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDCardiology,
                    SpecialtyName = "Cardiology"
                });

                Specialty surgeonSpecialtyReq = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDSurgeonReq,
                    SpecialtyName = "Surgeon"
                });
                #endregion

                #region Check in a patient

                // Check in a patient
                PatientCheck patientcheck = db.PatientChecks.Add(new PatientCheck
                {
                    PatientCheckID = patientCheckID16Over,
                    PatientID = patientID16Over,
                    Patient = patient,
                    CheckinDateTime = DateTime.Now,
                    SpecialtyID = specialtyIDSurgeonReq,
                    Specialty = surgeonSpecialtyReq
                });
                #endregion

                #region Add some doctors
                Doctor johnSmithCardiology = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDCardiology,
                    FirstName = "John",
                    LastName = "Smith",
                    SpecialtyID = pediatricSpecialty.SpecialtyID,
                    Specialty = pediatricSpecialty
                });

                Doctor juliaSmithSurgeon = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDSurgeonReq,
                    FirstName = "Julia",
                    LastName = "Smith",
                    SpecialtyID = surgeonSpecialtyReq.SpecialtyID,
                    Specialty = surgeonSpecialtyReq
                });
               
                #endregion

                #region Add an Exam Room

                ExamRoom examRoom1 = db.ExamRooms.Add(new ExamRoom
                {
                    ExamRoomID = examRoomIDForTest,
                    ExamRoomName = "ExamRoom 1"
                });
                #endregion

                #region Check in Doctors
                // Check in the cardiologist
                var johnSmithCardiologyCheckIn = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDCardiology,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = doctorIDCardiology,
                    ExamRoomID = examRoomIDForTest,
                    ExamRoom = examRoom1,
                    Doctor = johnSmithCardiology
                };

                johnSmithCardiology.DoctorChecks.Add(johnSmithCardiologyCheckIn);
                db.DoctorChecks.Add(johnSmithCardiologyCheckIn);

                // Check in the Surgeon
                var juliaSmithSurgeonCheckIn = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDSurgeonReq,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = doctorIDSurgeonReq,
                    ExamRoomID = examRoomIDForTest,
                    ExamRoom = examRoom1,
                    Doctor = juliaSmithSurgeon
                };

                juliaSmithSurgeon.DoctorChecks.Add(juliaSmithSurgeonCheckIn);
                db.DoctorChecks.Add(juliaSmithSurgeonCheckIn);
                #endregion

                using (var scheduler = new AppointmentScheduler(db))
                {
                    //ACT
                    scheduler.CreateAppointment(patientcheck.PatientCheckID);

                    //ASSERT
                    Assert.IsTrue(db.Appointments.Count() > 0);
                    var appointment = db.Appointments.First();

                    // Verify that the patient is assigned to surgeon, not cardiologist
                    Assert.IsTrue(appointment.PatientID == patientID16Over
                                    && appointment.DoctorID == doctorIDSurgeonReq);
                }
            }
        }

        [TestMethod]
        public void Patient16OverGetsRequestedSpecialtyWithFewestUpcomingAppointments()
        {
            using (IMedAgendaDbContext db = new TestMedAgendaDbContext())
            {
                int appointmentIDSurgeonFewerAppts = 1;
                int appointmentIDSurgeonMoreAppts = 2;
                int appointmentIDSurgeonMoreApptsAnother = 3;

                int doctorIDSurgeonMoreAppts = 1;
                int doctorIDSurgeonFewerAppts = 2;

                int doctorCheckIDSurgeonMoreAppts = 1;
                int doctorCheckIDSurgeonFewerAppts = 2;

                int examRoomIDForTest = 1;

                int patientID16Over = 1;
                int patientIDSurgeonFewerAppts = 2;
                int patientIDSurgeonMoreAppts = 3;
                int patientIDSurgeonMoreApptsAnother = 4;

                int patientCheckID16Over = 1;

                int specialtyIDSurgeon = 1;


                #region Add some patients
                // Patient over 16
                Patient patient = db.Patients.Add(new Patient
                {
                    PatientID = patientID16Over,
                    Birthdate = DateTime.Now.AddYears(-78),
                    FirstName = "Old",
                    LastName = "Man"
                });

                // Patients to be used in appointments
                Patient patientSurgeonFewerAppts = db.Patients.Add(new Patient
                {
                    PatientID = patientIDSurgeonFewerAppts,
                    Birthdate = DateTime.Now.AddYears(-68),
                    FirstName = "Ima",
                    LastName = "Patient"
                });

                Patient patientSurgeonMoreAppts = db.Patients.Add(new Patient
                {
                    PatientID = patientIDSurgeonMoreAppts,
                    Birthdate = DateTime.Now.AddYears(-48),
                    FirstName = "Ineeda",
                    LastName = "Doctor"
                });

                Patient patientSurgeonMoreApptsAnother = db.Patients.Add(new Patient
                {
                    PatientID = patientIDSurgeonMoreApptsAnother,
                    Birthdate = DateTime.Now.AddYears(-58),
                    FirstName = "Iwanta",
                    LastName = "Doctor"
                });

                #endregion

                #region Add some specialties

                Specialty surgeonSpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDSurgeon,
                    SpecialtyName = "Surgeon"
                });
                #endregion

                #region Check in a patient

                // Check in the patient over 16 with requested specialty surgeon
                PatientCheck patientcheck = db.PatientChecks.Add(new PatientCheck
                {
                    PatientCheckID = patientCheckID16Over,
                    PatientID = patientID16Over,
                    Patient = patient,
                    CheckinDateTime = DateTime.Now,
                    SpecialtyID = specialtyIDSurgeon,
                    Specialty = surgeonSpecialty
                });
                #endregion

                #region Add some doctors

                // Add two surgeons
                Doctor johnSmithSurgeonMoreAppts = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDSurgeonMoreAppts,
                    FirstName = "John",
                    LastName = "Smith",
                    SpecialtyID = surgeonSpecialty.SpecialtyID,
                    Specialty = surgeonSpecialty
                });

                Doctor juliaSmithSurgeonFewerAppts = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDSurgeonFewerAppts,
                    FirstName = "Julia",
                    LastName = "Smith",
                    SpecialtyID = surgeonSpecialty.SpecialtyID,
                    Specialty = surgeonSpecialty
                });
                #endregion

                #region Add an Exam Room

                ExamRoom examRoom = db.ExamRooms.Add(new ExamRoom
                {
                    ExamRoomID = examRoomIDForTest,
                    ExamRoomName = "ExamRoom 1"
                });
                #endregion

                #region Check in Doctors
                // Check in both surgeons
                var johnSmithSurgeonMoreApptsCheckIn = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDSurgeonMoreAppts,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = doctorIDSurgeonMoreAppts,
                    ExamRoomID = examRoomIDForTest,
                    ExamRoom = examRoom,
                    Doctor = johnSmithSurgeonMoreAppts
                };

                johnSmithSurgeonMoreAppts.DoctorChecks.Add(johnSmithSurgeonMoreApptsCheckIn);
                db.DoctorChecks.Add(johnSmithSurgeonMoreApptsCheckIn);

                var juliaSmithSurgeonFewerApptsCheckIn = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDSurgeonFewerAppts,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = doctorIDSurgeonFewerAppts,
                    ExamRoomID = examRoomIDForTest,
                    ExamRoom = examRoom,
                    Doctor = juliaSmithSurgeonFewerAppts
                };

                juliaSmithSurgeonFewerAppts.DoctorChecks.Add(juliaSmithSurgeonFewerApptsCheckIn);
                db.DoctorChecks.Add(juliaSmithSurgeonFewerApptsCheckIn);
                #endregion

                #region Create appointments


                // Create 1 appointment for one surgeon, and 2 appointments for the other
                var juliaSurgeonFewerApptsAppointment = new Appointment
                {
                    AppointmentID = appointmentIDSurgeonFewerAppts,
                    PatientID = patientIDSurgeonFewerAppts,
                    DoctorID = doctorIDSurgeonFewerAppts,
                    ExamRoomID = examRoomIDForTest,
                    CheckinDateTime = DateTime.Now.AddHours(-1),
                    Doctor = juliaSmithSurgeonFewerAppts,
                    ExamRoom = examRoom,
                    Patient = patientSurgeonFewerAppts
                };
                juliaSmithSurgeonFewerAppts.Appointments.Add(juliaSurgeonFewerApptsAppointment);
                db.Appointments.Add(juliaSurgeonFewerApptsAppointment);

                var johnSurgeonMoreApptsAppointment = new Appointment
                {
                    AppointmentID = appointmentIDSurgeonMoreAppts,
                    PatientID = patientIDSurgeonMoreAppts,
                    DoctorID = doctorIDSurgeonMoreAppts,
                    ExamRoomID = examRoomIDForTest,
                    CheckinDateTime = DateTime.Now.AddHours(-2),
                    Doctor = johnSmithSurgeonMoreAppts,
                    ExamRoom = examRoom,
                    Patient = patientSurgeonMoreAppts
                };
                johnSmithSurgeonMoreAppts.Appointments.Add(johnSurgeonMoreApptsAppointment);
                db.Appointments.Add(johnSurgeonMoreApptsAppointment);

                var johnSurgeonMoreApptsAppointmentAnother = new Appointment
                {
                    AppointmentID = appointmentIDSurgeonMoreApptsAnother,
                    PatientID = patientIDSurgeonMoreApptsAnother,
                    DoctorID = doctorIDSurgeonMoreAppts,
                    ExamRoomID = examRoomIDForTest,
                    CheckinDateTime = DateTime.Now.AddHours(-1),
                    Doctor = johnSmithSurgeonMoreAppts,
                    ExamRoom = examRoom,
                    Patient = patientSurgeonMoreApptsAnother
                };
                johnSmithSurgeonMoreAppts.Appointments.Add(johnSurgeonMoreApptsAppointmentAnother);
                db.Appointments.Add(johnSurgeonMoreApptsAppointmentAnother);

                #endregion

                using (var scheduler = new AppointmentScheduler(db))
                {
                    //ACT
                    scheduler.CreateAppointment(patientcheck.PatientCheckID);

                    //ASSERT
                    Assert.IsTrue(db.Appointments.Count() > 0);
                    var appointment = db.Appointments.Last();

                    // Verify that the patient is assigned to surgeon with fewer appointments
                    Assert.IsTrue(appointment.PatientID == patientID16Over
                                    && appointment.DoctorID == doctorIDSurgeonFewerAppts);
                }
            }
        }

        [TestMethod]
        public void PatientUnder16GetsGeneralPracticeIfNoPediatricians()
        {
            int doctorIDGeneralPractice = 1;
            int doctorIDSurgeon = 2;
            int doctorIDPediatrician = 3;

            int doctorCheckIDGeneralPractice = 1;
            int doctorCheckIDSurgeon = 2;
            int doctorCheckIDPediatrician = 3;

            int examRoomIDForTest = 1;

            int patientIDUnder16 = 1;

            int patientCheckIDUnder16 = 1;

            int specialtyIDSurgeon = 1;
            int specialtyIDGeneralPractice = 2;
            int specialtyIDPediatrics = 3;

            using (IMedAgendaDbContext db = new TestMedAgendaDbContext())
            {
                //Arrange
                #region Add some specialties
                Specialty surgeonSpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDSurgeon,
                    SpecialtyName = "Surgeon"
                });

                Specialty generalPracticeSpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDGeneralPractice,
                    SpecialtyName = "General Practice"
                });

                Specialty pediatricSpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDPediatrics,
                    SpecialtyName = "Pediatrics"
                });

                #endregion

                #region Add a patient under 16
                Patient patient = db.Patients.Add(new Patient
                {
                    PatientID = patientIDUnder16,
                    Birthdate = DateTime.Now.AddYears(-8),
                    FirstName = "Young",
                    LastName = "Ster"
                });
                #endregion

                #region Add some doctors

                Doctor docGeneralPractice = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDGeneralPractice,
                    FirstName = "Julia",
                    LastName = "Smith",
                    SpecialtyID = generalPracticeSpecialty.SpecialtyID,
                    Specialty = generalPracticeSpecialty
                });

                Doctor docSurgeon = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDSurgeon,
                    FirstName = "John",
                    LastName = "Smith",
                    SpecialtyID = surgeonSpecialty.SpecialtyID,
                    Specialty = surgeonSpecialty
                });

                Doctor docPediatrician = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDPediatrician,
                    FirstName = "Bob",
                    LastName = "Smith",
                    SpecialtyID = pediatricSpecialty.SpecialtyID,
                    Specialty = pediatricSpecialty
                });
                #endregion

                #region Add an exam room
                ExamRoom examroom =
                    db.ExamRooms.Add(new ExamRoom
                    { ExamRoomID = examRoomIDForTest, ExamRoomName = "Room One" });
                #endregion

                #region Check in the doctors

                // Check in the GP
                var docGeneralPracticeCheckIn = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDGeneralPractice,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = doctorIDGeneralPractice,
                    Doctor = docGeneralPractice,
                    ExamRoomID = examRoomIDForTest,
                    ExamRoom = examroom
                };
                docGeneralPractice.DoctorChecks.Add(docGeneralPracticeCheckIn);
                db.DoctorChecks.Add(docGeneralPracticeCheckIn);

                // Check in the surgeon
                var docSurgeonCheckIn = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDSurgeon,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = doctorIDSurgeon,
                    ExamRoomID = examRoomIDForTest,
                    ExamRoom = examroom,
                    Doctor = docSurgeon
                };
                docSurgeon.DoctorChecks.Add(docSurgeonCheckIn);
                db.DoctorChecks.Add(docSurgeonCheckIn);

                // Check in and out the pediatrician
                var docPediatricianCheckIn = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDPediatrician,
                    CheckinDateTime = DateTime.Now.AddHours(-1),
                    CheckoutDateTime = DateTime.Now,
                    DoctorID = doctorIDPediatrician,
                    ExamRoomID = examRoomIDForTest,
                    ExamRoom = examroom,
                    Doctor = docPediatrician
                };
                docPediatrician.DoctorChecks.Add(docPediatricianCheckIn);
                db.DoctorChecks.Add(docPediatricianCheckIn);

                #endregion

                #region Check in a patient

                // Check in a patient
                PatientCheck patientcheck = db.PatientChecks.Add(new PatientCheck
                {
                    PatientCheckID = patientCheckIDUnder16,
                    PatientID = patientIDUnder16,
                    Patient = patient,
                    CheckinDateTime = DateTime.Now,
                    SpecialtyID = specialtyIDSurgeon,
                    Specialty = surgeonSpecialty
                });

                #endregion

                using (var scheduler = new AppointmentScheduler(db))
                {
                    //ACT
                    scheduler.CreateAppointment(patientcheck.PatientCheckID);

                    //ASSERT: verify that appointment is created and
                    // patient is assigned to GP
                    Assert.IsTrue(db.Appointments.Count() > 0);
                    var appointment = db.Appointments.First();

                    Assert.IsTrue(appointment.PatientID == patientIDUnder16
                                    && appointment.DoctorID == doctorIDGeneralPractice);
                }
            }
        }

        [TestMethod]
        public void Patient16OverGetsGeneralPracticeIfRequestedSpecialtyNotAvailable()
        {
            int doctorIDGeneralPractice = 1;
            int doctorIDSurgeon = 2;
            int doctorIDNeurologistReq = 3;

            int doctorCheckIDGeneralPractice = 1;
            int doctorCheckIDSurgeon = 2;
            int doctorCheckIDNeurologistReq = 3;

            int examRoomIDForTest = 1;

            int patientID16Over = 1;

            int patientCheckID16Over = 1;

            int specialtyIDSurgeon = 1;           
            int specialtyIDGeneralPractice = 2;
            int specialtyIDNeurologyReq = 3;

            using (IMedAgendaDbContext db = new TestMedAgendaDbContext())
            {
                //Arrange
                #region Add some specialties
                Specialty surgeonSpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDSurgeon,
                    SpecialtyName = "Surgeon"
                });               

                Specialty generalPracticeSpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDGeneralPractice,
                    SpecialtyName = "General Practice"
                });

                Specialty neurologySpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDNeurologyReq,
                    SpecialtyName = "Neurology"
                });
                #endregion

                #region Add a patient 16 or over
                Patient patient = db.Patients.Add(new Patient
                {
                    PatientID = patientID16Over,
                    Birthdate = DateTime.Now.AddYears(-78),
                    FirstName = "Old",
                    LastName = "Ster"
                });
                #endregion

                #region Add some doctors

                Doctor docGeneralPractice = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDGeneralPractice,
                    FirstName = "Julia",
                    LastName = "Smith",
                    SpecialtyID = generalPracticeSpecialty.SpecialtyID,
                    Specialty = generalPracticeSpecialty
                });

                Doctor docSurgeon = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDSurgeon,
                    FirstName = "John",
                    LastName = "Smith",
                    SpecialtyID = surgeonSpecialty.SpecialtyID,
                    Specialty = surgeonSpecialty
                });

                Doctor docNeurologist = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDNeurologistReq,
                    FirstName = "Bob",
                    LastName = "Smith",
                    SpecialtyID = neurologySpecialty.SpecialtyID,
                    Specialty = neurologySpecialty
                });

                #endregion

                #region Add an exam room
                ExamRoom examroom =
                    db.ExamRooms.Add(new ExamRoom
                    { ExamRoomID = examRoomIDForTest, ExamRoomName = "Room One" });
                #endregion

                #region Check in the doctors

                // Check in the GP
                var docGeneralPracticeCheckIn = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDGeneralPractice,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = doctorIDGeneralPractice,
                    Doctor = docGeneralPractice,
                    ExamRoomID = examRoomIDForTest,
                    ExamRoom = examroom
                };
                docGeneralPractice.DoctorChecks.Add(docGeneralPracticeCheckIn);
                db.DoctorChecks.Add(docGeneralPracticeCheckIn);

                // Check in the surgeon
                var docSurgeonCheckIn = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDSurgeon,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = doctorIDSurgeon,
                    ExamRoomID = examRoomIDForTest,
                    ExamRoom = examroom,
                    Doctor = docSurgeon
                };
                docSurgeon.DoctorChecks.Add(docSurgeonCheckIn);
                db.DoctorChecks.Add(docSurgeonCheckIn);

                // Check in and out the neurologist
                var docNeurologistCheckIn = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDNeurologistReq,
                    CheckinDateTime = DateTime.Now.AddHours(-1),
                    CheckoutDateTime = DateTime.Now,
                    DoctorID = doctorIDNeurologistReq,
                    ExamRoomID = examRoomIDForTest,
                    ExamRoom = examroom,
                    Doctor = docNeurologist
                };
                docNeurologist.DoctorChecks.Add(docNeurologistCheckIn);
                db.DoctorChecks.Add(docNeurologistCheckIn);

                #endregion

                #region Check in a patient

                // Check in a patient, with preferred specialty neurology
                PatientCheck patientcheck = db.PatientChecks.Add(new PatientCheck
                {
                    PatientCheckID = patientCheckID16Over,
                    PatientID = patientID16Over,
                    Patient = patient,
                    CheckinDateTime = DateTime.Now,
                    SpecialtyID = specialtyIDNeurologyReq,
                    Specialty = neurologySpecialty
                });

                #endregion

                using (var scheduler = new AppointmentScheduler(db))
                {
                    //ACT
                    scheduler.CreateAppointment(patientcheck.PatientCheckID);

                    //ASSERT: verify that appointment is created and
                    // patient is assigned to GP
                    Assert.IsTrue(db.Appointments.Count() > 0);
                    var appointment = db.Appointments.First();

                    Assert.IsTrue(appointment.PatientID == patientID16Over
                                    && appointment.DoctorID == doctorIDGeneralPractice);
                }
            }
        }

        [TestMethod]
        public void PatientUnder16GetsDoctorWithFewestApptsIfNoPediatricianAndNoGP()
        {
            using (IMedAgendaDbContext db = new TestMedAgendaDbContext())
            {
                int appointmentIDNeurologistReqFewerAppts = 1;
                int appointmentIDSurgeonMoreAppts = 2;
                int appointmentIDSurgeonMoreApptsAnother = 3;

                int doctorIDSurgeonMoreAppts = 1;
                int doctorIDNeurologistReqFewerAppts = 2;

                int doctorCheckIDSurgeonMoreAppts = 1;
                int doctorCheckIDNeurologistReqFewerAppts = 2;

                int examRoomIDForTest = 1;

                int patientIDUnder16 = 1;
                int patientIDNeurologistReqFewerAppts = 2;
                int patientIDSurgeonMoreAppts = 3;
                int patientIDSurgeonMoreApptsAnother = 4;

                int patientCheckIDUnder16 = 1;

                int specialtyIDSurgeon = 1;
                int specialtyIDNeurology = 2;

                #region Add some patients
                // Patient under 16
                Patient patient = db.Patients.Add(new Patient
                {
                    PatientID = patientIDUnder16,
                    Birthdate = DateTime.Now.AddYears(-8),
                    FirstName = "Young",
                    LastName = "Ster"
                });

                // Patients to be used in appointments
                Patient patientNeurologistFewerAppts = db.Patients.Add(new Patient
                {
                    PatientID = patientIDNeurologistReqFewerAppts,
                    Birthdate = DateTime.Now.AddYears(-68),
                    FirstName = "Ima",
                    LastName = "Patient"
                });

                Patient patientSurgeonMoreAppts = db.Patients.Add(new Patient
                {
                    PatientID = patientIDSurgeonMoreAppts,
                    Birthdate = DateTime.Now.AddYears(-48),
                    FirstName = "Ineeda",
                    LastName = "Doctor"
                });

                Patient patientSurgeonMoreApptsAnother = db.Patients.Add(new Patient
                {
                    PatientID = patientIDSurgeonMoreApptsAnother,
                    Birthdate = DateTime.Now.AddYears(-58),
                    FirstName = "Iwanta",
                    LastName = "Doctor"
                });

                #endregion

                #region Add some specialties

                Specialty surgeonSpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDSurgeon,
                    SpecialtyName = "Surgeon"
                });

                Specialty neurologistSpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDNeurology,
                    SpecialtyName = "Neurology"
                });
                #endregion

                #region Check in a patient

                // Check in the patient under 16 with requested specialty surgeon
                PatientCheck patientcheck = db.PatientChecks.Add(new PatientCheck
                {
                    PatientCheckID = patientCheckIDUnder16,
                    PatientID = patientIDUnder16,
                    Patient = patient,
                    CheckinDateTime = DateTime.Now,
                    SpecialtyID = specialtyIDSurgeon,
                    Specialty = surgeonSpecialty
                });
                #endregion

                #region Add some doctors

                // Add a surgeon
                Doctor johnSmithSurgeonMoreAppts = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDSurgeonMoreAppts,
                    FirstName = "John",
                    LastName = "Smith",
                    SpecialtyID = surgeonSpecialty.SpecialtyID,
                    Specialty = surgeonSpecialty
                });

                // Add a neurologist
                Doctor juliaSmithNeurologistFewerAppts = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDNeurologistReqFewerAppts,
                    FirstName = "Julia",
                    LastName = "Smith",
                    SpecialtyID = neurologistSpecialty.SpecialtyID,
                    Specialty = neurologistSpecialty
                });
                #endregion

                #region Add an Exam Room

                ExamRoom examRoom = db.ExamRooms.Add(new ExamRoom
                {
                    ExamRoomID = examRoomIDForTest,
                    ExamRoomName = "ExamRoom 1"
                });
                #endregion

                #region Check in Doctors
                // Check in both doctors
                var johnSmithSurgeonMoreApptsCheckIn = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDSurgeonMoreAppts,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = doctorIDSurgeonMoreAppts,
                    ExamRoomID = examRoomIDForTest,
                    ExamRoom = examRoom,
                    Doctor = johnSmithSurgeonMoreAppts
                };

                johnSmithSurgeonMoreAppts.DoctorChecks.Add(johnSmithSurgeonMoreApptsCheckIn);
                db.DoctorChecks.Add(johnSmithSurgeonMoreApptsCheckIn);

                var juliaSmithNeurologistFewerApptsCheckIn = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDNeurologistReqFewerAppts,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = doctorIDNeurologistReqFewerAppts,
                    ExamRoomID = examRoomIDForTest,
                    ExamRoom = examRoom,
                    Doctor = juliaSmithNeurologistFewerAppts
                };

                juliaSmithNeurologistFewerAppts.DoctorChecks.Add(juliaSmithNeurologistFewerApptsCheckIn);
                db.DoctorChecks.Add(juliaSmithNeurologistFewerApptsCheckIn);
                #endregion

                #region Create appointments

                // Create 1 appointment for the neurologist, and 2 appointments for the surgeon
                var juliaNeurologistFewerApptsAppointment = new Appointment
                {
                    AppointmentID = appointmentIDNeurologistReqFewerAppts,
                    PatientID = patientIDNeurologistReqFewerAppts,
                    DoctorID = doctorIDNeurologistReqFewerAppts,
                    ExamRoomID = examRoomIDForTest,
                    CheckinDateTime = DateTime.Now.AddHours(-1),
                    Doctor = juliaSmithNeurologistFewerAppts,
                    ExamRoom = examRoom,
                    Patient = patientNeurologistFewerAppts
                };
                juliaSmithNeurologistFewerAppts.Appointments.Add(juliaNeurologistFewerApptsAppointment);
                db.Appointments.Add(juliaNeurologistFewerApptsAppointment);

                var johnSurgeonMoreApptsAppointment = new Appointment
                {
                    AppointmentID = appointmentIDSurgeonMoreAppts,
                    PatientID = patientIDSurgeonMoreAppts,
                    DoctorID = doctorIDSurgeonMoreAppts,
                    ExamRoomID = examRoomIDForTest,
                    CheckinDateTime = DateTime.Now.AddHours(-2),
                    Doctor = johnSmithSurgeonMoreAppts,
                    ExamRoom = examRoom,
                    Patient = patientSurgeonMoreAppts
                };
                johnSmithSurgeonMoreAppts.Appointments.Add(johnSurgeonMoreApptsAppointment);
                db.Appointments.Add(johnSurgeonMoreApptsAppointment);

                var johnSurgeonMoreApptsAppointmentAnother = new Appointment
                {
                    AppointmentID = appointmentIDSurgeonMoreApptsAnother,
                    PatientID = patientIDSurgeonMoreApptsAnother,
                    DoctorID = doctorIDSurgeonMoreAppts,
                    ExamRoomID = examRoomIDForTest,
                    CheckinDateTime = DateTime.Now.AddHours(-1),
                    Doctor = johnSmithSurgeonMoreAppts,
                    ExamRoom = examRoom,
                    Patient = patientSurgeonMoreApptsAnother
                };
                johnSmithSurgeonMoreAppts.Appointments.Add(johnSurgeonMoreApptsAppointmentAnother);
                db.Appointments.Add(johnSurgeonMoreApptsAppointmentAnother);

                #endregion

                using (var scheduler = new AppointmentScheduler(db))
                {
                    //ACT
                    scheduler.CreateAppointment(patientcheck.PatientCheckID);

                    //ASSERT
                    Assert.IsTrue(db.Appointments.Count() > 0);
                    var appointment = db.Appointments.Last();

                    // Verify that the patient is assigned to surgeon with fewer appointments
                    Assert.IsTrue(appointment.PatientID == patientIDUnder16
                                    && appointment.DoctorID == doctorIDNeurologistReqFewerAppts);
                }
            }
        }

        [TestMethod]
        public void Patient16OverGetsDoctorWithFewestApptsIfNoSpecialistAndNoGP()
        {
            using (IMedAgendaDbContext db = new TestMedAgendaDbContext())
            {
                int appointmentIDNeurologistFewerAppts = 1;
                int appointmentIDSurgeonMoreAppts = 2;
                int appointmentIDSurgeonMoreApptsAnother = 3;

                int doctorIDSurgeonMoreAppts = 1;
                int doctorIDNeurologistFewerAppts = 2;
                int doctorIDCardiologyReq = 3;

                int doctorCheckIDSurgeonMoreAppts = 1;
                int doctorCheckIDNeurologistFewerAppts = 2;
                int doctorCheckIDCardiologyReq = 3;

                int examRoomIDForTest = 1;

                int patientID16Over = 1;
                int patientIDNeurologistFewerAppts = 2;
                int patientIDSurgeonMoreAppts = 3;
                int patientIDSurgeonMoreApptsAnother = 4;

                int patientCheckID16Over = 1;

                int specialtyIDSurgeon = 1;
                int specialtyIDNeurology = 2;
                int specialtyIDCardiology = 3;

                #region Add some patients
                // Patient over 16
                Patient patient = db.Patients.Add(new Patient
                {
                    PatientID = patientID16Over,
                    Birthdate = DateTime.Now.AddYears(-78),
                    FirstName = "Old",
                    LastName = "Man"
                });

                // Patients to be used in appointments
                Patient patientNeurologistFewerAppts = db.Patients.Add(new Patient
                {
                    PatientID = patientIDNeurologistFewerAppts,
                    Birthdate = DateTime.Now.AddYears(-68),
                    FirstName = "Ima",
                    LastName = "Patient"
                });

                Patient patientSurgeonMoreAppts = db.Patients.Add(new Patient
                {
                    PatientID = patientIDSurgeonMoreAppts,
                    Birthdate = DateTime.Now.AddYears(-48),
                    FirstName = "Ineeda",
                    LastName = "Doctor"
                });

                Patient patientSurgeonMoreApptsAnother = db.Patients.Add(new Patient
                {
                    PatientID = patientIDSurgeonMoreApptsAnother,
                    Birthdate = DateTime.Now.AddYears(-58),
                    FirstName = "Iwanta",
                    LastName = "Doctor"
                });

                #endregion

                #region Add some specialties

                Specialty surgeonSpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDSurgeon,
                    SpecialtyName = "Surgeon"
                });

                Specialty neurologistSpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDNeurology,
                    SpecialtyName = "Neurology"
                });               

                Specialty cardiologySpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDCardiology,
                    SpecialtyName = "Cardiology"
                });
                #endregion

                #region Check in a patient

                // Check in the patient under 16 with requested specialty cardiology
                PatientCheck patientcheck = db.PatientChecks.Add(new PatientCheck
                {
                    PatientCheckID = patientCheckID16Over,
                    PatientID = patientID16Over,
                    Patient = patient,
                    CheckinDateTime = DateTime.Now,
                    SpecialtyID = specialtyIDCardiology,
                    Specialty = cardiologySpecialty
                });
                #endregion

                #region Add some doctors

                // Add a surgeon
                Doctor johnSmithSurgeonMoreAppts = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDSurgeonMoreAppts,
                    FirstName = "John",
                    LastName = "Smith",
                    SpecialtyID = surgeonSpecialty.SpecialtyID,
                    Specialty = surgeonSpecialty
                });

                // Add a neurologist
                Doctor juliaSmithNeurologistFewerAppts = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDNeurologistFewerAppts,
                    FirstName = "Julia",
                    LastName = "Smith",
                    SpecialtyID = neurologistSpecialty.SpecialtyID,
                    Specialty = neurologistSpecialty
                });

                // Add a cardiologist
                Doctor cardiologistReq = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDCardiologyReq,
                    FirstName = "Bob",
                    LastName = "Smith",
                    SpecialtyID = cardiologySpecialty.SpecialtyID,
                    Specialty = cardiologySpecialty
                });
                #endregion

                #region Add an Exam Room

                ExamRoom examRoom = db.ExamRooms.Add(new ExamRoom
                {
                    ExamRoomID = examRoomIDForTest,
                    ExamRoomName = "ExamRoom 1"
                });
                #endregion

                #region Check in Doctors
                // Check in surgeon and neurologist
                var johnSmithSurgeonMoreApptsCheckIn = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDSurgeonMoreAppts,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = doctorIDSurgeonMoreAppts,
                    ExamRoomID = examRoomIDForTest,
                    ExamRoom = examRoom,
                    Doctor = johnSmithSurgeonMoreAppts
                };

                johnSmithSurgeonMoreAppts.DoctorChecks.Add(johnSmithSurgeonMoreApptsCheckIn);
                db.DoctorChecks.Add(johnSmithSurgeonMoreApptsCheckIn);

                var juliaSmithNeurologistFewerApptsCheckIn = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDNeurologistFewerAppts,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = doctorIDNeurologistFewerAppts,
                    ExamRoomID = examRoomIDForTest,
                    ExamRoom = examRoom,
                    Doctor = juliaSmithNeurologistFewerAppts
                };

                juliaSmithNeurologistFewerAppts.DoctorChecks.Add(juliaSmithNeurologistFewerApptsCheckIn);
                db.DoctorChecks.Add(juliaSmithNeurologistFewerApptsCheckIn);

                // Check in and out cardiologist
                var cardiologistReqCheckInOut = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDCardiologyReq,
                    CheckinDateTime = DateTime.Now.AddHours(-2),
                    CheckoutDateTime = DateTime.Now,
                    DoctorID = doctorIDCardiologyReq,
                    ExamRoomID = examRoomIDForTest,
                    ExamRoom = examRoom,
                    Doctor = cardiologistReq
                };

                cardiologistReq.DoctorChecks.Add(cardiologistReqCheckInOut);
                db.DoctorChecks.Add(cardiologistReqCheckInOut);
                #endregion

                #region Create appointments


                // Create 1 appointment for the neurologist, and 2 appointments for the surgeon
                var juliaNeurologistFewerApptsAppointment = new Appointment
                {
                    AppointmentID = appointmentIDNeurologistFewerAppts,
                    PatientID = patientIDNeurologistFewerAppts,
                    DoctorID = doctorIDNeurologistFewerAppts,
                    ExamRoomID = examRoomIDForTest,
                    CheckinDateTime = DateTime.Now.AddHours(-1),
                    Doctor = juliaSmithNeurologistFewerAppts,
                    ExamRoom = examRoom,
                    Patient = patientNeurologistFewerAppts
                };
                juliaSmithNeurologistFewerAppts.Appointments.Add(juliaNeurologistFewerApptsAppointment);
                db.Appointments.Add(juliaNeurologistFewerApptsAppointment);

                var johnSurgeonMoreApptsAppointment = new Appointment
                {
                    AppointmentID = appointmentIDSurgeonMoreAppts,
                    PatientID = patientIDSurgeonMoreAppts,
                    DoctorID = doctorIDSurgeonMoreAppts,
                    ExamRoomID = examRoomIDForTest,
                    CheckinDateTime = DateTime.Now.AddHours(-2),
                    Doctor = johnSmithSurgeonMoreAppts,
                    ExamRoom = examRoom,
                    Patient = patientSurgeonMoreAppts
                };
                johnSmithSurgeonMoreAppts.Appointments.Add(johnSurgeonMoreApptsAppointment);
                db.Appointments.Add(johnSurgeonMoreApptsAppointment);

                var johnSurgeonMoreApptsAppointmentAnother = new Appointment
                {
                    AppointmentID = appointmentIDSurgeonMoreApptsAnother,
                    PatientID = patientIDSurgeonMoreApptsAnother,
                    DoctorID = doctorIDSurgeonMoreAppts,
                    ExamRoomID = examRoomIDForTest,
                    CheckinDateTime = DateTime.Now.AddHours(-1),
                    Doctor = johnSmithSurgeonMoreAppts,
                    ExamRoom = examRoom,
                    Patient = patientSurgeonMoreApptsAnother
                };
                johnSmithSurgeonMoreAppts.Appointments.Add(johnSurgeonMoreApptsAppointmentAnother);
                db.Appointments.Add(johnSurgeonMoreApptsAppointmentAnother);

                #endregion

                using (var scheduler = new AppointmentScheduler(db))
                {
                    //ACT
                    scheduler.CreateAppointment(patientcheck.PatientCheckID);

                    //ASSERT
                    Assert.IsTrue(db.Appointments.Count() > 0);
                    var appointment = db.Appointments.Last();

                    // Verify that the patient is assigned to doctor with fewer appointments
                    Assert.IsTrue(appointment.PatientID == patientID16Over
                                    && appointment.DoctorID == doctorIDNeurologistFewerAppts);
                }
            }
        }
        [TestMethod]
        public void ApptInPreferredExamRoomIfNoUpcomingAppointments()
        {
            using (IMedAgendaDbContext db = new TestMedAgendaDbContext())
            {
                int appointmentIDPastExamRoomPreferred = 1;

                int doctorIDSurgeonReq = 1;

                int doctorCheckIDSurgeonReq = 1;

                int examRoomIDForTest = 1;
                int examRoomIDPreferred = 2;

                int patientID16Over = 1;
                int patientIDForTest = 2;

                int patientCheckID16Over = 1;
                int patientCheckIDForTest = 2;

                int specialtyIDSurgeonReq = 2;

                #region Add a patient over 16, and another for appointment
                Patient patient = db.Patients.Add(new Patient
                {
                    PatientID = patientID16Over,
                    Birthdate = DateTime.Now.AddYears(-78),
                    FirstName = "Old",
                    LastName = "Man"
                });

                Patient patientForTest = db.Patients.Add(new Patient
                {
                    PatientID = patientIDForTest,
                    Birthdate = DateTime.Now.AddYears(-68),
                    FirstName = "Old",
                    LastName = "Ster"
                });

                #endregion

                #region Add some specialties

                Specialty surgeonSpecialtyReq = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDSurgeonReq,
                    SpecialtyName = "Surgeon"
                });
                #endregion

                #region Check in a patient

                // Check in a patient
                PatientCheck patientcheck = db.PatientChecks.Add(new PatientCheck
                {
                    PatientCheckID = patientCheckID16Over,
                    PatientID = patientID16Over,
                    Patient = patient,
                    CheckinDateTime = DateTime.Now,
                    SpecialtyID = specialtyIDSurgeonReq,
                    Specialty = surgeonSpecialtyReq
                });

                PatientCheck patientcheckForTest = db.PatientChecks.Add(new PatientCheck
                {
                    PatientCheckID = patientCheckIDForTest,
                    PatientID = patientIDForTest,
                    Patient = patientForTest,
                    CheckinDateTime = DateTime.Now.AddHours(-1),
                    SpecialtyID = specialtyIDSurgeonReq,
                    Specialty = surgeonSpecialtyReq
                });
                #endregion

                #region Add some doctors
                Doctor docSurgeon = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDSurgeonReq,
                    FirstName = "Julia",
                    LastName = "Smith",
                    SpecialtyID = surgeonSpecialtyReq.SpecialtyID,
                    Specialty = surgeonSpecialtyReq
                });

                #endregion

                #region Add some Exam Rooms

                ExamRoom examRoomForTest = db.ExamRooms.Add(new ExamRoom
                {
                    ExamRoomID = examRoomIDForTest,
                    ExamRoomName = "ExamRoom 1"
                });

                ExamRoom examRoomPreferred = db.ExamRooms.Add(new ExamRoom
                {
                    ExamRoomID = examRoomIDPreferred,
                    ExamRoomName = "ExamRoom 2"
                });
                #endregion

                #region Check in Doctors

                // Check in the Surgeon
                var docSurgeonCheckIn = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDSurgeonReq,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = doctorIDSurgeonReq,
                    ExamRoomID = examRoomIDPreferred,
                    ExamRoom = examRoomPreferred,
                    Doctor = docSurgeon
                };

                docSurgeon.DoctorChecks.Add(docSurgeonCheckIn);
                db.DoctorChecks.Add(docSurgeonCheckIn);
                #endregion

                #region Create appointments

                // Create a past appointment for the surgeon in the preferred exam room
                var surgeonPastAppointment = new Appointment
                {
                    AppointmentID = appointmentIDPastExamRoomPreferred,
                    PatientID = patientIDForTest,
                    DoctorID = doctorIDSurgeonReq,
                    ExamRoomID = examRoomIDPreferred,
                    CheckinDateTime = DateTime.Now.AddHours(-8),
                    CheckoutDateTime = DateTime.Now,
                    Doctor = docSurgeon,
                    ExamRoom = examRoomPreferred,
                    Patient = patient
                };
                docSurgeon.Appointments.Add(surgeonPastAppointment);
                examRoomPreferred.Appointments.Add(surgeonPastAppointment);
                db.Appointments.Add(surgeonPastAppointment);

                #endregion

                using (var scheduler = new AppointmentScheduler(db))
                {
                    //ACT
                    scheduler.CreateAppointment(patientcheck.PatientCheckID);

                    //ASSERT
                    Assert.IsTrue(db.Appointments.Count() > 0);
                    var appointment = db.Appointments.Last();

                    // Verify that the appointment is in preferred exam room
                    Assert.IsTrue(appointment.PatientID == patientID16Over
                                    && appointment.ExamRoomID == examRoomIDPreferred);
                }
            }
        }

            [TestMethod]
        public void ApptInExamRoomWithFewestUpcomingApptsIfUpcomingApptsForPreferredExamRoom()
        {

            using (IMedAgendaDbContext db = new TestMedAgendaDbContext())
            {                              
                int appointmentIDExamRoomMoreUpcomingAppts = 1;
                int appointmentIDExamRoomMoreUpcomingApptsAnother = 2;
                int appointmentIDExamRoomMoreUpcomingApptsYetAnother = 3;
                int appointmentIDExamRoomFewerUpcomingAppts = 4;
                int appointmentIDExamRoomFewerUpcomingApptsAnother = 5;
                int appointmentIDUpcomingExamRoomPreferred = 6;

                int doctorIDSurgeonReq = 1;

                int doctorCheckIDSurgeonReq = 1;

                int examRoomIDMoreUpcomingAppts = 1;
                int examRoomIDFewerUpcomingAppts = 2;
                int examRoomIDPreferred = 3;

                int patientID16Over = 1;
                int patientIDMoreUpcomingAppts = 2;
                int patientIDMoreUpcomingApptsAnother = 3;
                int patientIDMoreUpcomingApptsYetAnother = 4;
                int patientIDFewerUpcomingAppts = 5;
                int patientIDFewerUpcomingApptsAnother = 6;
                int patientIDExamRoomPreferred = 7;

                int patientCheckID16Over = 1;
                int patientCheckIDMoreUpcomingAppts = 2;
                int patientCheckIDMoreUpcomingApptsAnother = 3;
                int patientCheckIDMoreUpcomingApptsYetAnother = 4;
                int patientCheckIDFewerUpcomingAppts = 5;
                int patientCheckIDFewerUpcomingApptsAnother = 6;
                int patientCheckIDExamRoomPreferred = 7;

                int specialtyIDSurgeonReq = 2;

                #region Add a patient over 16, and patients for appointments
                Patient patient = db.Patients.Add(new Patient
                {
                    PatientID = patientID16Over,
                    Birthdate = DateTime.Now.AddYears(-78),
                    FirstName = "Old",
                    LastName = "Man"
                });

                Patient patientMoreUpcomingAppts = db.Patients.Add(new Patient
                {
                    PatientID = patientIDMoreUpcomingAppts,
                    Birthdate = DateTime.Now.AddYears(-68),
                    FirstName = "Old",
                    LastName = "Ster"
                });

                Patient patientMoreUpcomingApptsAnother = db.Patients.Add(new Patient
                {
                    PatientID = patientIDMoreUpcomingApptsAnother,
                    Birthdate = DateTime.Now.AddYears(-58),
                    FirstName = "Ray",
                    LastName = "Ster"
                });

                Patient patientMoreUpcomingApptsYetAnother = db.Patients.Add(new Patient
                {
                    PatientID = patientIDMoreUpcomingApptsYetAnother,
                    Birthdate = DateTime.Now.AddYears(-55),
                    FirstName = "Ray",
                    LastName = "Man"
                });

                Patient patientFewerUpcomingAppts = db.Patients.Add(new Patient
                {
                    PatientID = patientIDFewerUpcomingAppts,
                    Birthdate = DateTime.Now.AddYears(-48),
                    FirstName = "Old",
                    LastName = "Biz"
                });

                Patient patientFewerUpcomingApptsAnother = db.Patients.Add(new Patient
                {
                    PatientID = patientIDFewerUpcomingApptsAnother,
                    Birthdate = DateTime.Now.AddYears(-42),
                    FirstName = "Old",
                    LastName = "Fence"
                });

                Patient patientExamRoomPreferred = db.Patients.Add(new Patient
                {
                    PatientID = patientIDExamRoomPreferred,
                    Birthdate = DateTime.Now.AddYears(-88),
                    FirstName = "Alte",
                    LastName = "Manner"
                });

                #endregion

                #region Add some specialties

                Specialty surgeonSpecialtyReq = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDSurgeonReq,
                    SpecialtyName = "Surgeon"
                });
                #endregion

                #region Check in the patients

                PatientCheck patientcheck = db.PatientChecks.Add(new PatientCheck
                {
                    PatientCheckID = patientCheckID16Over,
                    PatientID = patientID16Over,
                    Patient = patient,
                    CheckinDateTime = DateTime.Now,
                    SpecialtyID = specialtyIDSurgeonReq,
                    Specialty = surgeonSpecialtyReq
                });

                PatientCheck patientMoreUpcomingApptscheck = db.PatientChecks.Add(new PatientCheck
                {
                    PatientCheckID = patientCheckIDMoreUpcomingAppts,
                    PatientID = patientIDMoreUpcomingAppts,
                    Patient = patientMoreUpcomingAppts,
                    CheckinDateTime = DateTime.Now.AddHours(-3),
                    SpecialtyID = specialtyIDSurgeonReq,
                    Specialty = surgeonSpecialtyReq
                });

                PatientCheck patientMoreUpcomingApptsAnothercheck = db.PatientChecks.Add(new PatientCheck
                {
                    PatientCheckID = patientCheckIDMoreUpcomingApptsAnother,
                    PatientID = patientIDMoreUpcomingApptsAnother,
                    Patient = patientMoreUpcomingApptsAnother,
                    CheckinDateTime = DateTime.Now.AddHours(-2),
                    SpecialtyID = specialtyIDSurgeonReq,
                    Specialty = surgeonSpecialtyReq
                });

                PatientCheck patientMoreUpcomingApptsYetAnothercheck = db.PatientChecks.Add(new PatientCheck
                {
                    PatientCheckID = patientCheckIDMoreUpcomingApptsYetAnother,
                    PatientID = patientIDMoreUpcomingApptsYetAnother,
                    Patient = patientMoreUpcomingApptsYetAnother,
                    CheckinDateTime = DateTime.Now.AddHours(-1),
                    SpecialtyID = specialtyIDSurgeonReq,
                    Specialty = surgeonSpecialtyReq
                });

                PatientCheck patientFewerUpcomingApptscheck = db.PatientChecks.Add(new PatientCheck
                {
                    PatientCheckID = patientCheckIDFewerUpcomingAppts,
                    PatientID = patientIDFewerUpcomingAppts,
                    Patient = patientFewerUpcomingAppts,
                    CheckinDateTime = DateTime.Now.AddHours(-2),
                    SpecialtyID = specialtyIDSurgeonReq,
                    Specialty = surgeonSpecialtyReq
                });

                PatientCheck patientFewerUpcomingApptscheckAnother = db.PatientChecks.Add(new PatientCheck
                {
                    PatientCheckID = patientCheckIDFewerUpcomingApptsAnother,
                    PatientID = patientIDFewerUpcomingApptsAnother,
                    Patient = patientFewerUpcomingApptsAnother,
                    CheckinDateTime = DateTime.Now.AddHours(-1),
                    SpecialtyID = specialtyIDSurgeonReq,
                    Specialty = surgeonSpecialtyReq
                });

                PatientCheck patientExamRoomPreferredcheck = db.PatientChecks.Add(new PatientCheck
                {
                    PatientCheckID = patientCheckIDExamRoomPreferred,
                    PatientID = patientIDExamRoomPreferred,
                    Patient = patientExamRoomPreferred,
                    CheckinDateTime = DateTime.Now.AddHours(-2),
                    SpecialtyID = specialtyIDSurgeonReq,
                    Specialty = surgeonSpecialtyReq
                });

                #endregion

                #region Add some doctors
                Doctor docSurgeon = db.Doctors.Add(new Doctor
                {
                    DoctorID = doctorIDSurgeonReq,
                    FirstName = "Julia",
                    LastName = "Smith",
                    SpecialtyID = surgeonSpecialtyReq.SpecialtyID,
                    Specialty = surgeonSpecialtyReq
                });

                #endregion

                #region Add some Exam Rooms

                ExamRoom examRoomMoreUpcomingAppts = db.ExamRooms.Add(new ExamRoom
                {
                    ExamRoomID = examRoomIDMoreUpcomingAppts,
                    ExamRoomName = "ExamRoom 1"
                });

                ExamRoom examRoomFewerUpcomingAppts = db.ExamRooms.Add(new ExamRoom
                {
                    ExamRoomID = examRoomIDFewerUpcomingAppts,
                    ExamRoomName = "ExamRoom 2"
                });

                ExamRoom examRoomPreferred = db.ExamRooms.Add(new ExamRoom
                {
                    ExamRoomID = examRoomIDPreferred,
                    ExamRoomName = "ExamRoom 3"
                });
                #endregion

                #region Check in Doctors

                var docSurgeonCheckIn = new DoctorCheck
                {
                    DoctorCheckID = doctorCheckIDSurgeonReq,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = doctorIDSurgeonReq,
                    ExamRoomID = examRoomIDPreferred,
                    ExamRoom = examRoomPreferred,
                    Doctor = docSurgeon
                };

                docSurgeon.DoctorChecks.Add(docSurgeonCheckIn);
                db.DoctorChecks.Add(docSurgeonCheckIn);
                #endregion

                #region Create appointments

                // Create three upcoming appointment in one exam room with more appointments, 
                // two in the exam room with fewer appointments, and one in the preferred
                var upcomingAppointmentMore = new Appointment
                {
                    AppointmentID = appointmentIDExamRoomMoreUpcomingAppts,
                    PatientID = patientIDMoreUpcomingAppts,
                    DoctorID = doctorIDSurgeonReq,
                    ExamRoomID = examRoomIDMoreUpcomingAppts,
                    CheckinDateTime = DateTime.Now.AddHours(-8),
                    Doctor = docSurgeon,
                    ExamRoom = examRoomMoreUpcomingAppts,
                    Patient = patientMoreUpcomingAppts
                };
                docSurgeon.Appointments.Add(upcomingAppointmentMore);
                examRoomMoreUpcomingAppts.Appointments.Add(upcomingAppointmentMore);
                db.Appointments.Add(upcomingAppointmentMore);

                var upcomingAppointmentMoreAnother = new Appointment
                {
                    AppointmentID = appointmentIDExamRoomMoreUpcomingApptsAnother,
                    PatientID = patientIDMoreUpcomingApptsAnother,
                    DoctorID = doctorIDSurgeonReq,
                    ExamRoomID = examRoomIDMoreUpcomingAppts,
                    CheckinDateTime = DateTime.Now.AddHours(-6),
                    Doctor = docSurgeon,
                    ExamRoom = examRoomMoreUpcomingAppts,
                    Patient = patientMoreUpcomingApptsAnother
                };
                docSurgeon.Appointments.Add(upcomingAppointmentMoreAnother);
                examRoomMoreUpcomingAppts.Appointments.Add(upcomingAppointmentMoreAnother);
                db.Appointments.Add(upcomingAppointmentMoreAnother);

                var upcomingAppointmentMoreYetAnother = new Appointment
                {
                    AppointmentID = appointmentIDExamRoomMoreUpcomingApptsYetAnother,
                    PatientID = patientIDMoreUpcomingApptsYetAnother,
                    DoctorID = doctorIDSurgeonReq,
                    ExamRoomID = examRoomIDMoreUpcomingAppts,
                    CheckinDateTime = DateTime.Now.AddHours(-6),
                    Doctor = docSurgeon,
                    ExamRoom = examRoomMoreUpcomingAppts,
                    Patient = patientMoreUpcomingApptsYetAnother
                };
                docSurgeon.Appointments.Add(upcomingAppointmentMoreYetAnother);
                examRoomMoreUpcomingAppts.Appointments.Add(upcomingAppointmentMoreYetAnother);
                db.Appointments.Add(upcomingAppointmentMoreYetAnother);

                var upcomingAppointmentFewer = new Appointment
                {
                    AppointmentID = appointmentIDExamRoomFewerUpcomingAppts,
                    PatientID = patientIDFewerUpcomingAppts,
                    DoctorID = doctorIDSurgeonReq,
                    ExamRoomID = examRoomIDFewerUpcomingAppts,
                    CheckinDateTime = DateTime.Now.AddHours(-8),
                    Doctor = docSurgeon,
                    ExamRoom = examRoomFewerUpcomingAppts,
                    Patient = patientFewerUpcomingAppts
                };
                docSurgeon.Appointments.Add(upcomingAppointmentFewer);
                examRoomFewerUpcomingAppts.Appointments.Add(upcomingAppointmentFewer);
                db.Appointments.Add(upcomingAppointmentFewer);

                var upcomingAppointmentFewerAnother = new Appointment
                {
                    AppointmentID = appointmentIDExamRoomFewerUpcomingApptsAnother,
                    PatientID = patientIDFewerUpcomingApptsAnother,
                    DoctorID = doctorIDSurgeonReq,
                    ExamRoomID = examRoomIDFewerUpcomingAppts,
                    CheckinDateTime = DateTime.Now.AddHours(-6),
                    Doctor = docSurgeon,
                    ExamRoom = examRoomFewerUpcomingAppts,
                    Patient = patientFewerUpcomingApptsAnother
                };
                docSurgeon.Appointments.Add(upcomingAppointmentFewerAnother);
                examRoomFewerUpcomingAppts.Appointments.Add(upcomingAppointmentFewerAnother);
                db.Appointments.Add(upcomingAppointmentFewerAnother);
               
                var upcomingAppointmentPreferredExamRoom = new Appointment
                {
                    AppointmentID = appointmentIDUpcomingExamRoomPreferred,
                    PatientID = patientIDExamRoomPreferred,
                    DoctorID = doctorIDSurgeonReq,
                    ExamRoomID = examRoomIDPreferred,
                    CheckinDateTime = DateTime.Now.AddHours(-2),                   
                    Doctor = docSurgeon,
                    ExamRoom = examRoomPreferred,
                    Patient = patientExamRoomPreferred
                };
                docSurgeon.Appointments.Add(upcomingAppointmentPreferredExamRoom);
                examRoomPreferred.Appointments.Add(upcomingAppointmentPreferredExamRoom);
                db.Appointments.Add(upcomingAppointmentPreferredExamRoom);

                #endregion

                using (var scheduler = new AppointmentScheduler(db))
                {
                    //ACT
                    scheduler.CreateAppointment(patientcheck.PatientCheckID);

                    //ASSERT
                    Assert.IsTrue(db.Appointments.Count() > 0);
                    var appointment = db.Appointments.Last();

                    // Verify that appointment is in exam room with fewer appointments
                    Assert.IsTrue(appointment.PatientID == patientID16Over
                                    && appointment.ExamRoomID == examRoomIDFewerUpcomingAppts);
                }
            }
        }

    }

}



