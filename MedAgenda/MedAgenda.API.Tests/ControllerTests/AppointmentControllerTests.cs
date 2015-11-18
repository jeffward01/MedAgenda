using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MedAgenda.API.Controllers;
using MedAgenda.CORE.Models;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using MedAgenda.API.Tests.Infrastructure;
using MedAgenda.CORE.Infrastructure;
using MedAgenda.CORE.Domain;

namespace MedAgenda.API.Tests.ControllerTests
{
    /// <summary>
    /// Summary description for AccountControllerTests
    /// </summary>
    [TestClass]
    public class AppointmentControllerTests : BaseTest
    {    
        [TestMethod] // Get all appointments | [0]
        public void GetAppointmentsReturnAppointments()
        {
            //Arrange
            using (var apptController = new AppointmentsController())
            {
                //Act
                IEnumerable<AppointmentModel> appointments = apptController.GetAppointments();

                //Assert
                if (appointments.Count() == 0) Assert.Inconclusive("Appointments table is empty");

                Assert.IsTrue(appointments.Count() > 0);
            }
        }

        [TestMethod] // Get an Appointment | [1]
        public void GetAppointmentReturnAppointment()
        {
            int createdPatientID;
            int appointmentIDForTest;
            int createdDoctorID;
            int createdExamRoomID;
            int createdSpecialtyID;
            IHttpActionResult result;

            //Arrange: create test patient, specialty, doctor, exam room, and appointment
            // Create a new test patient, and get its patient ID
            using (var patientController = new PatientsController())
            {
                var patient = new PatientModel
                {
                    FirstName = "Impatient",
                    LastName = "Patience",
                    Birthdate = new DateTime(1968, 12, 27),
                    Email = "a@b.com",
                    BloodType = "A+",
                    CreatedDate = DateTime.Now,
                    Archived = false
                };
                result = patientController.PostPatient(patient);
                CreatedAtRouteNegotiatedContentResult<PatientModel> patientContentResult =
                    (CreatedAtRouteNegotiatedContentResult<PatientModel>)result;
                createdPatientID = patientContentResult.Content.PatientID;
            }

            // Create a new test specialty, and get its specialty ID
            using (var specialtyController = new SpecialtiesController())
            {
                var specialty = new SpecialtyModel
                {
                    SpecialtyName = "Very Special Doctor"
                };
                result = specialtyController.PostSpecialty(specialty);
                CreatedAtRouteNegotiatedContentResult<SpecialtyModel> specialtyContentResult =
                    (CreatedAtRouteNegotiatedContentResult<SpecialtyModel>)result;
                createdSpecialtyID = specialtyContentResult.Content.SpecialtyID;
            }

            // Create a new test doctor, and get its doctor ID
            using (var doctorController = new DoctorsController())
            {
                var doctor = new DoctorModel
                {
                    FirstName = "Imdoctor",
                    LastName = "Hippocrates",
                    Email = "a@b.com",
                    Telephone = "555-1212",
                    CreatedDate = DateTime.Now,
                    SpecialtyID = createdSpecialtyID,
                    Archived = false
                };
                result = doctorController.PostDoctor(doctor);
                CreatedAtRouteNegotiatedContentResult<DoctorModel> doctorContentResult =
                    (CreatedAtRouteNegotiatedContentResult<DoctorModel>)result;
                createdDoctorID = doctorContentResult.Content.DoctorID;
            }

            // Create a new test exam room, and get its exam room ID
            using (var examRoomController = new ExamRoomsController())
            {
                var examRoom = new ExamRoomModel
                {
                    ExamRoomName = "ImexamRoom"
                };
                result = examRoomController.PostExamRoom(examRoom);
                CreatedAtRouteNegotiatedContentResult<ExamRoomModel> examRoomContentResult =
                    (CreatedAtRouteNegotiatedContentResult<ExamRoomModel>)result;
                createdExamRoomID = examRoomContentResult.Content.ExamRoomID;
            }

            // Create test appointment
            using (var appointmentController = new AppointmentsController())
            {
                var appointment = new AppointmentModel
                {
                    PatientID = createdPatientID,
                    DoctorID = createdDoctorID,
                    ExamRoomID = createdExamRoomID,
                    CheckinDateTime = DateTime.Now,
                    CheckoutDateTime = DateTime.Now
                };

                result = appointmentController.PostAppointment(appointment);
                CreatedAtRouteNegotiatedContentResult<AppointmentModel> appointmentContentResult =
                (CreatedAtRouteNegotiatedContentResult<AppointmentModel>)result;
                appointmentIDForTest = appointmentContentResult.Content.AppointmentID;
            }

            using (var apptController = new AppointmentsController())
            {
                //Act
                result = apptController.GetAppointment(appointmentIDForTest);

                //Assert
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<AppointmentModel>));
               
                OkNegotiatedContentResult<AppointmentModel> contentResult = 
                    (OkNegotiatedContentResult<AppointmentModel>)result;

                Assert.IsTrue(contentResult.Content.AppointmentID == appointmentIDForTest);               
            }

            // Delete the test appointment
            using (var apptController = new AppointmentsController())
            {
                //Delete test Appointment 
                result = apptController.DeleteAppointment(appointmentIDForTest);
            }

            // Remove the test doctor and patient from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Doctor dbDoctor = db.Doctors.Find(createdDoctorID);
                db.Doctors.Remove(dbDoctor);
                Patient dbPatient = db.Patients.Find(createdPatientID);
                db.Patients.Remove(dbPatient);
                db.SaveChanges();
            }

            // Delete the test exam room
            using (var SecondExamRoomController = new ExamRoomsController())
            {               
                result = SecondExamRoomController.DeleteExamRoom(createdExamRoomID);
            }

            // Delete the test specialty
            using (var specialtyController = new SpecialtiesController())
            {
                result = specialtyController.DeleteSpecialty(createdSpecialtyID);
            }
        }

        [TestMethod] // Create an Appointment | [2]
        public void PostAppointmentCreateAppointment()
        {
            int createdPatientID;
            int appointmentIDForTest;
            int createdDoctorID;
            int createdExamRoomID;
            int createdSpecialtyID;
            CreatedAtRouteNegotiatedContentResult<AppointmentModel> contentResult;

            //Create test patient, specialty, doctor, and exam room for appointment
            // Create a new test patient, and get its patient ID
            using (var patientController = new PatientsController())
            {
                var patient = new PatientModel
                {
                    FirstName = "Impatient",
                    LastName = "Patience",
                    Birthdate = new DateTime(1968, 12, 27),
                    Email = "a@b.com",
                    BloodType = "A+",
                    CreatedDate = DateTime.Now,
                    Archived = false
                };
                IHttpActionResult result = patientController.PostPatient(patient);
                CreatedAtRouteNegotiatedContentResult<PatientModel> patientContentResult =
                    (CreatedAtRouteNegotiatedContentResult<PatientModel>)result;
                createdPatientID = patientContentResult.Content.PatientID;
            }

            // Create a new test specialty, and get its specialty ID
            using (var specialtyController = new SpecialtiesController())
            {
                var specialty = new SpecialtyModel
                {
                    SpecialtyName = "Very Special Doctor"
                };
                IHttpActionResult result = specialtyController.PostSpecialty(specialty);
                CreatedAtRouteNegotiatedContentResult<SpecialtyModel> specialtyContentResult =
                    (CreatedAtRouteNegotiatedContentResult<SpecialtyModel>)result;
                createdSpecialtyID = specialtyContentResult.Content.SpecialtyID;
            }

            // Create a new test doctor, and get its doctor ID
            using (var doctorController = new DoctorsController())
            {
                var doctor = new DoctorModel
                {
                    FirstName = "Imdoctor",
                    LastName = "Hippocrates",
                    Email = "a@b.com",
                    Telephone = "555-1212",
                    CreatedDate = DateTime.Now,
                    SpecialtyID = createdSpecialtyID,
                    Archived = false
                };
                IHttpActionResult result = doctorController.PostDoctor(doctor);
                CreatedAtRouteNegotiatedContentResult<DoctorModel> doctorContentResult =
                    (CreatedAtRouteNegotiatedContentResult<DoctorModel>)result;
                createdDoctorID = doctorContentResult.Content.DoctorID;
            }

            // Create a new test exam room, and get its exam room ID
            using (var examRoomController = new ExamRoomsController())
            {
                var examRoom = new ExamRoomModel
                {
                    ExamRoomName = "ImexamRoom"
                };
                IHttpActionResult result = examRoomController.PostExamRoom(examRoom);
                CreatedAtRouteNegotiatedContentResult<ExamRoomModel> examRoomContentResult =
                    (CreatedAtRouteNegotiatedContentResult<ExamRoomModel>)result;
                createdExamRoomID = examRoomContentResult.Content.ExamRoomID;
            }

            //Arrange:
            using (var apptController = new AppointmentsController())
            {
                var newAppt = new AppointmentModel
                {
                    DoctorID = createdDoctorID,
                    CheckinDateTime = DateTime.Now,
                    CheckoutDateTime = DateTime.Now.AddHours(2),
                    ExamRoomID = createdExamRoomID,
                    PatientID = createdPatientID
                };

                //Act
                IHttpActionResult result = apptController.PostAppointment(newAppt);

                //Assert
                Assert.IsInstanceOfType(result, typeof(CreatedAtRouteNegotiatedContentResult<AppointmentModel>));

                contentResult = (CreatedAtRouteNegotiatedContentResult<AppointmentModel>)result;

                Assert.IsTrue(contentResult.Content.AppointmentID != 0);
                appointmentIDForTest = contentResult.Content.AppointmentID;
            }

            //Delete the Appointment
            using (var SecondapptController = new AppointmentsController())
            {
              IHttpActionResult result = SecondapptController.DeleteAppointment(appointmentIDForTest);
            }

            // Remove the test doctor and patient from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Doctor dbDoctor = db.Doctors.Find(createdDoctorID);
                db.Doctors.Remove(dbDoctor);
                Patient dbPatient = db.Patients.Find(createdPatientID);
                db.Patients.Remove(dbPatient);
                db.SaveChanges();
            }

            // Delete the test exam room
            using (var SecondExamRoomController = new ExamRoomsController())
            {
                IHttpActionResult result = SecondExamRoomController.DeleteExamRoom(createdExamRoomID);
            }

            // Delete the test specialty
            using (var specialtyController = new SpecialtiesController())
            {
                IHttpActionResult result = specialtyController.DeleteSpecialty(createdSpecialtyID);
            }
        }

        [TestMethod] // Update Appointment | [3]
        public void PutAppointmentReturnAppointment()
        {           
            //Test Properties
            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<AppointmentModel> contentResult;
            OkNegotiatedContentResult<AppointmentModel> appointmentResult;
            OkNegotiatedContentResult<AppointmentModel> readContentResult;            

            int createdPatientID;            
            int createdDoctorID;
            int createdExamRoomID;
            int changedExamRoomID;
            int createdSpecialtyID;

            //Arrange: create test patient, specialty, doctor, exam rooms, and appointment
            // Create a new test patient, and get its patient ID
            using (var patientController = new PatientsController())
            {
                var patient = new PatientModel
                {
                    FirstName = "Impatient",
                    LastName = "Patience",
                    Birthdate = new DateTime(1968, 12, 27),
                    Email = "a@b.com",
                    BloodType = "A+",
                    CreatedDate = DateTime.Now,
                    Archived = false
                };
                result = patientController.PostPatient(patient);
                CreatedAtRouteNegotiatedContentResult<PatientModel> patientContentResult =
                    (CreatedAtRouteNegotiatedContentResult<PatientModel>)result;
                createdPatientID = patientContentResult.Content.PatientID;
            }

            // Create a new test specialty, and get its specialty ID
            using (var specialtyController = new SpecialtiesController())
            {
                var specialty = new SpecialtyModel
                {
                    SpecialtyName = "Very Special Doctor"
                };
                result = specialtyController.PostSpecialty(specialty);
                CreatedAtRouteNegotiatedContentResult<SpecialtyModel> specialtyContentResult =
                    (CreatedAtRouteNegotiatedContentResult<SpecialtyModel>)result;
                createdSpecialtyID = specialtyContentResult.Content.SpecialtyID;
            }

            // Create a new test doctor, and get its doctor ID
            using (var doctorController = new DoctorsController())
            {
                var doctor = new DoctorModel
                {
                    FirstName = "Imdoctor",
                    LastName = "Hippocrates",
                    Email = "a@b.com",
                    Telephone = "555-1212",
                    CreatedDate = DateTime.Now,
                    SpecialtyID = createdSpecialtyID,
                    Archived = false
                };
                result = doctorController.PostDoctor(doctor);
                CreatedAtRouteNegotiatedContentResult<DoctorModel> doctorContentResult =
                    (CreatedAtRouteNegotiatedContentResult<DoctorModel>)result;
                createdDoctorID = doctorContentResult.Content.DoctorID;
            }

            // Create new test exam rooms, and get the exam room IDs
            using (var examRoomController = new ExamRoomsController())
            {
                var examRoom = new ExamRoomModel
                {
                    ExamRoomName = "ImexamRoom"
                };
                result = examRoomController.PostExamRoom(examRoom);
                CreatedAtRouteNegotiatedContentResult<ExamRoomModel> examRoomContentResult =
                    (CreatedAtRouteNegotiatedContentResult<ExamRoomModel>)result;
                createdExamRoomID = examRoomContentResult.Content.ExamRoomID;
            }

            using (var examRoomController = new ExamRoomsController())
            {
                var examRoom = new ExamRoomModel
                {
                    ExamRoomName = "AnotherexamRoom"
                };
                result = examRoomController.PostExamRoom(examRoom);
                CreatedAtRouteNegotiatedContentResult<ExamRoomModel> examRoomContentResult =
                    (CreatedAtRouteNegotiatedContentResult<ExamRoomModel>)result;
                changedExamRoomID = examRoomContentResult.Content.ExamRoomID;
            }

            using (var apptController = new AppointmentsController())
            {
                //Create Appointment
                var newAppt = new AppointmentModel
                {
                    DoctorID = createdDoctorID,
                    CheckinDateTime = DateTime.Now,
                    CheckoutDateTime = DateTime.Now.AddHours(2),
                    ExamRoomID = createdExamRoomID,
                    PatientID = createdPatientID
                };

                //Insert Appointment Model Object into Database
                //So that I can take it out and test for update
                result = apptController.PostAppointment(newAppt);

                //Cast result as Content Result so that I can gather information from Content Result
                contentResult = (CreatedAtRouteNegotiatedContentResult<AppointmentModel>)result;
            }

            using (var SecondAppointmentController = new AppointmentsController())
            {
                //Result contains the Appoint I have JUST creatd
                result = SecondAppointmentController.GetAppointment(contentResult.Content.AppointmentID);

                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<AppointmentModel>));

                //Get AppointmentModel from 'result'
                appointmentResult = (OkNegotiatedContentResult<AppointmentModel>)result;
            }

            using (var ThirdAppointmentController = new AppointmentsController())
            {
                var modifiedAppointment = appointmentResult.Content;

                modifiedAppointment.ExamRoomID = changedExamRoomID;

                //Act
                //The result of the PUT request
                result = ThirdAppointmentController.PutAppointment(appointmentResult.Content.AppointmentID, modifiedAppointment);

                //Assert
                Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            }

            //Modify Appointment
            using (var FourthAppointmentController = new AppointmentsController())
            {
                IHttpActionResult resultAlteredAppointment = FourthAppointmentController.GetAppointment(appointmentResult.Content.AppointmentID);

                OkNegotiatedContentResult<AppointmentModel> alteredResult = (OkNegotiatedContentResult<AppointmentModel>)resultAlteredAppointment;
                AppointmentModel updatedAppointment = (AppointmentModel)alteredResult.Content;

                //Assert
                Assert.IsInstanceOfType(resultAlteredAppointment, typeof(OkNegotiatedContentResult<AppointmentModel>));

                readContentResult = (OkNegotiatedContentResult<AppointmentModel>)resultAlteredAppointment;

                Assert.IsTrue(readContentResult.Content.ExamRoomID == changedExamRoomID);
            }

            using (var fifthAppointmentController = new AppointmentsController())
            {
                //Delete test Appointment 
                result = fifthAppointmentController.DeleteAppointment(readContentResult.Content.AppointmentID);
            }

            // Remove the test doctor and patient from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Doctor dbDoctor = db.Doctors.Find(createdDoctorID);
                db.Doctors.Remove(dbDoctor);
                Patient dbPatient = db.Patients.Find(createdPatientID);
                db.Patients.Remove(dbPatient);
                db.SaveChanges();
            }

            // Delete the test exam rooms
            using (var SecondExamRoomController = new ExamRoomsController())
            {
                result = SecondExamRoomController.DeleteExamRoom(createdExamRoomID);
            }
            using (var SecondExamRoomController = new ExamRoomsController())
            {
                result = SecondExamRoomController.DeleteExamRoom(changedExamRoomID);
            }

            // Delete the test specialty
            using (var specialtyController = new SpecialtiesController())
            {
                result = specialtyController.DeleteSpecialty(createdSpecialtyID);
            }
        }

        [TestMethod] //Delete Appointment | [4]
        public void DeleteAppointment()
        {
            int createdPatientID;            
            int createdDoctorID;
            int createdExamRoomID;
            int createdSpecialtyID;

            CreatedAtRouteNegotiatedContentResult<AppointmentModel> contentResult;

            //Arrange: create test patient, specialty, doctor, exam room, and appointment
            // Create a new test patient, and get its patient ID
            using (var patientController = new PatientsController())
            {
                var patient = new PatientModel
                {
                    FirstName = "Impatient",
                    LastName = "Patience",
                    Birthdate = new DateTime(1968, 12, 27),
                    Email = "a@b.com",
                    BloodType = "A+",
                    CreatedDate = DateTime.Now,
                    Archived = false
                };
                IHttpActionResult result = patientController.PostPatient(patient);
                CreatedAtRouteNegotiatedContentResult<PatientModel> patientContentResult =
                    (CreatedAtRouteNegotiatedContentResult<PatientModel>)result;
                createdPatientID = patientContentResult.Content.PatientID;
            }

            // Create a new test specialty, and get its specialty ID
            using (var specialtyController = new SpecialtiesController())
            {
                var specialty = new SpecialtyModel
                {
                    SpecialtyName = "Very Special Doctor"
                };
                IHttpActionResult result = specialtyController.PostSpecialty(specialty);
                CreatedAtRouteNegotiatedContentResult<SpecialtyModel> specialtyContentResult =
                    (CreatedAtRouteNegotiatedContentResult<SpecialtyModel>)result;
                createdSpecialtyID = specialtyContentResult.Content.SpecialtyID;
            }

            // Create a new test doctor, and get its doctor ID
            using (var doctorController = new DoctorsController())
            {
                var doctor = new DoctorModel
                {
                    FirstName = "Imdoctor",
                    LastName = "Hippocrates",
                    Email = "a@b.com",
                    Telephone = "555-1212",
                    CreatedDate = DateTime.Now,
                    SpecialtyID = createdSpecialtyID,
                    Archived = false
                };
                IHttpActionResult result = doctorController.PostDoctor(doctor);
                CreatedAtRouteNegotiatedContentResult<DoctorModel> doctorContentResult =
                    (CreatedAtRouteNegotiatedContentResult<DoctorModel>)result;
                createdDoctorID = doctorContentResult.Content.DoctorID;
            }

            // Create a new test exam room, and get its exam room ID
            using (var examRoomController = new ExamRoomsController())
            {
                var examRoom = new ExamRoomModel
                {
                    ExamRoomName = "ImexamRoom"
                };
                IHttpActionResult result = examRoomController.PostExamRoom(examRoom);
                CreatedAtRouteNegotiatedContentResult<ExamRoomModel> examRoomContentResult =
                    (CreatedAtRouteNegotiatedContentResult<ExamRoomModel>)result;
                createdExamRoomID = examRoomContentResult.Content.ExamRoomID;
            }
            using (var ApptController = new AppointmentsController())
            {
                //Create Doctor
                var newAppt = new AppointmentModel
                {
                    DoctorID = createdDoctorID,
                    CheckinDateTime = DateTime.Now,
                    CheckoutDateTime = DateTime.Now.AddHours(2),
                    ExamRoomID = createdExamRoomID,
                    PatientID = createdPatientID
                };
                //Insert AppointmentModel into Database so 
                //that I can take it out and test for update.
                var result = ApptController.PostAppointment(newAppt);

                //Cast result as Content Result so that I can gather information from ContentResult
                contentResult = (CreatedAtRouteNegotiatedContentResult<AppointmentModel>)result;
            }

            using (var secondApptController = new AppointmentsController())
            {
                //Delete the Test Appointment
                var result = secondApptController.DeleteAppointment(contentResult.Content.AppointmentID);

                //Assert
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<AppointmentModel>));
            }
            using (var thirdApptController = new AppointmentsController())
            {
                var result = thirdApptController.GetAppointment(contentResult.Content.AppointmentID);

                //Assert
                Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            }

            // Remove the test doctor and patient from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Doctor dbDoctor = db.Doctors.Find(createdDoctorID);
                db.Doctors.Remove(dbDoctor);
                Patient dbPatient = db.Patients.Find(createdPatientID);
                db.Patients.Remove(dbPatient);
                db.SaveChanges();
            }

            // Delete the test exam room
            using (var SecondExamRoomController = new ExamRoomsController())
            {
                IHttpActionResult result = SecondExamRoomController.DeleteExamRoom(createdExamRoomID);
            }

            // Delete the test specialty
            using (var specialtyController = new SpecialtiesController())
            {
                IHttpActionResult result = specialtyController.DeleteSpecialty(createdSpecialtyID);
            }
        }

    }
}
