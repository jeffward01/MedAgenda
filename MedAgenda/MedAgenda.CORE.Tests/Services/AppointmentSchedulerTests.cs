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
                int doctorIDSurgeon = 2;

                int doctorCheckIDPediatrics = 1;
                int doctorCheckIDSurgeon = 2;

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
                int doctorIDSurgeon = 2;

                int doctorCheckIDCardiology = 1;
                int doctorCheckIDSurgeon = 2;

                int examRoomIDForTest = 1;

                int patientID16Over = 1;

                int patientCheckID16Over = 1;

                int specialtyIDCardiology = 1;
                int specialtyIDSurgeon = 2;

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
                    SpecialtyID = specialtyIDSurgeon,
                    Specialty = surgeonSpecialty
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

                    // Verify that the patient is assigned to surgeon, not cardiologist
                    Assert.IsTrue(appointment.PatientID == patientID16Over
                                    && appointment.DoctorID == doctorIDSurgeon);
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

            int doctorCheckIDGeneralPractice = 1;
            int doctorCheckIDSurgeon = 2;

            int examRoomIDForTest = 1;

            int patientIDUnder16 = 1;

            int patientCheckIDUnder16 = 1;

            int specialtyIDSurgeon = 1;
            int specialtyIDGeneralPractice = 2;

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

            int doctorCheckIDGeneralPractice = 1;
            int doctorCheckIDSurgeon = 2;

            int examRoomIDForTest = 1;

            int patientID16Over = 1;

            int patientCheckID16Over = 1;

            int specialtyIDSurgeon = 1;
            int specialtyIDNeurology = 2;
            int specialtyIDGeneralPractice = 3;

            using (IMedAgendaDbContext db = new TestMedAgendaDbContext())
            {
                //Arrange
                #region Add some specialties
                Specialty surgeonSpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDSurgeon,
                    SpecialtyName = "Surgeon"
                });

                Specialty neurologySpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDNeurology,
                    SpecialtyName = "Neurology"
                });

                Specialty generalPracticeSpecialty = db.Specialties.Add(new Specialty
                {
                    SpecialtyID = specialtyIDGeneralPractice,
                    SpecialtyName = "General Practice"
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

                // Add GP and surgeon; no neurologist
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

                #endregion

                #region Check in a patient

                // Check in a patient, with preferred specialty neurology
                PatientCheck patientcheck = db.PatientChecks.Add(new PatientCheck
                {
                    PatientCheckID = patientCheckID16Over,
                    PatientID = patientID16Over,
                    Patient = patient,
                    CheckinDateTime = DateTime.Now,
                    SpecialtyID = specialtyIDNeurology,
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
                int appointmentIDNeurologistFewerAppts = 1;
                int appointmentIDSurgeonMoreAppts = 2;
                int appointmentIDSurgeonMoreApptsAnother = 3;

                int doctorIDSurgeonMoreAppts = 1;
                int doctorIDNeurologistFewerAppts = 2;

                int doctorCheckIDSurgeonMoreAppts = 1;
                int doctorCheckIDNeurologistFewerAppts = 2;

                int examRoomIDForTest = 1;

                int patientIDUnder16 = 1;
                int patientIDNeurologistFewerAppts = 2;
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
                    DoctorID = doctorIDNeurologistFewerAppts,
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
                    DoctorCheckID = doctorCheckIDNeurologistFewerAppts,
                    CheckinDateTime = DateTime.Now,
                    DoctorID = doctorIDNeurologistFewerAppts,
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

                    // Verify that the patient is assigned to surgeon with fewer appointments
                    Assert.IsTrue(appointment.PatientID == patientIDUnder16
                                    && appointment.DoctorID == doctorIDNeurologistFewerAppts);
                }
            }
        }
    }


}

