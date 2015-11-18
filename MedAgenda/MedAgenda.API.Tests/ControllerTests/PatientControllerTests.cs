using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MedAgenda.API.Tests.Infrastructure;
using MedAgenda.API.Controllers;
using System.Collections.Generic;
using System.Linq;
using MedAgenda.CORE.Models;
using System.Web.Http;
using System.Web.Http.Results;
using MedAgenda.CORE.Infrastructure;
using MedAgenda.CORE.Domain;

namespace MedAgenda.API.Tests.ControllerTests
{
    [TestClass]
    public class PatientControllerTests : BaseTest
    {
        [TestMethod]
        public void GetPatientsReturnsNonArchivedPatients()
        {

            //Arrange: Instantiate PatientsController so its methods can be called
            using (var patientController = new PatientsController())
            {
                //Act: Call the GetPatients method
                IEnumerable<PatientModel> patients = patientController.GetPatients();

                // Verify if patients were returned
                if (patients.Count() == 0) Assert.Inconclusive("No non-archived patients found");

                // If patients were returned, verify that all of the patients are not archived               
                Assert.IsTrue(patients.Where(p => p.Archived).Count() == 0);

            }
        }

        [TestMethod]
        public void GetArchivedPatientsReturnsArchivedPatients()
        {
            //Arrange: Instantiate PatientsController so its methods can be called
            using (var patientController = new PatientsController())
            {
                //Act: Call the GetPatients method
                IEnumerable<PatientModel> patients = patientController.GetArchivedPatients();

                // Verify if patients were returned
                if (patients.Count() == 0) Assert.Inconclusive("No archived patients found");

                // If patients were returned, verify that all of the patients are archived                                               
                Assert.IsTrue(patients.Where(p => !p.Archived).Count() == 0);
            }
        }

        [TestMethod]
        public void GetPatientReturnsPatient()
        {
            int patientIDForTest;
            string patientFirstNameForTest = "Impatient";
            string patientLastNameForTest = "Patience";

            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<PatientModel> createdContentResult;
            OkNegotiatedContentResult<PatientModel> OkContentResult;

            // Create a new test patient, and get its patient ID
            using (var patientController = new PatientsController())
            {
                var patient = new PatientModel
                {
                    FirstName = patientFirstNameForTest,
                    LastName = patientLastNameForTest,
                    Birthdate = new DateTime(1968, 12, 27),
                    Email = "a@b.com",
                    BloodType = "A+",
                    CreatedDate = new DateTime(2015, 11, 10),
                    Archived = false
                };
                result = patientController.PostPatient(patient);
                createdContentResult =
                    (CreatedAtRouteNegotiatedContentResult<PatientModel>)result;
                patientIDForTest = createdContentResult.Content.PatientID;
            }

            //Get the created patient, and check that it is the same that was created
            using (var patientController = new PatientsController())
            {
                //Act: Call the GetPatient method
                result = patientController.GetPatient(patientIDForTest);

                //Assert: 
                // Verify that HTTP status code is OK
                // Verify that returned patient is the same patient that was created
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<PatientModel>));

                OkContentResult =
                    (OkNegotiatedContentResult<PatientModel>)result;
                Assert.IsTrue(OkContentResult.Content.FirstName == patientFirstNameForTest);
                Assert.IsTrue(OkContentResult.Content.LastName == patientLastNameForTest);

            }

            // Remove the patient from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Patient dbPatient = db.Patients.Find(OkContentResult.Content.PatientID);
                db.Patients.Remove(dbPatient);
                db.SaveChanges();
            }
        }

        [TestMethod]
        public void GetAppointmentsForPatientReturnsAppointments()
        {
            int patientIDForTest;
            int createdAppointmentID;
            int createdDoctorID;
            int createdExamRoomID;
            int createdSpecialtyID;

            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<PatientModel> createdContentResult;
            OkNegotiatedContentResult<IEnumerable<AppointmentModel>> OkAppointmentContentResult;

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
                createdContentResult =
                    (CreatedAtRouteNegotiatedContentResult<PatientModel>)result;
                patientIDForTest = createdContentResult.Content.PatientID;
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

            // Create appointment for patient
            using (var appointmentController = new AppointmentsController())
            {
                var appointment = new AppointmentModel
                {
                    PatientID = patientIDForTest,
                    DoctorID = createdDoctorID,
                    ExamRoomID = createdExamRoomID,
                    CheckinDateTime = DateTime.Now,
                    CheckoutDateTime = DateTime.Now
                };

                result = appointmentController.PostAppointment(appointment);
                CreatedAtRouteNegotiatedContentResult<AppointmentModel> appointmentContentResult =
                (CreatedAtRouteNegotiatedContentResult<AppointmentModel>)result;
                createdAppointmentID = appointmentContentResult.Content.AppointmentID;
            }

            // Call GetAppointmentsForPatient
            using (var patientController = new PatientsController())
            {
                result = patientController.GetAppointmentsForPatient(patientIDForTest);

                // Verify that HTTP status code is OK
                Assert.IsInstanceOfType(result,
                typeof(OkNegotiatedContentResult<IEnumerable<AppointmentModel>>));

                // Verify that at least one element was returned
                OkAppointmentContentResult =
                (OkNegotiatedContentResult<IEnumerable<AppointmentModel>>)result;
                Assert.IsTrue(OkAppointmentContentResult.Content.Count() > 0);

                // Verify that all the returned appointment(s) correspond to the test patient ID
                Assert.IsTrue(OkAppointmentContentResult.Content.
                    Where(a => a.PatientID != patientIDForTest).Count() == 0);
            }

            // Delete the test appointment
            using (var apptController = new AppointmentsController())
            {
                result = apptController.DeleteAppointment(createdAppointmentID);
            }

            // Delete the test patient and doctor (actual deletion, not archiving) 
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {                              
                Doctor dbDoctor = db.Doctors.Find(createdDoctorID);
                db.Doctors.Remove(dbDoctor);               
                Patient dbPatient = db.Patients.Find(patientIDForTest);
                db.Patients.Remove(dbPatient);
                db.SaveChanges();
            }

            // Delete the test exam room
            using (var examRoomController = new ExamRoomsController())
            {
                result = examRoomController.DeleteExamRoom(createdExamRoomID);
            }

            // Delete the test specialty
            using (var specialtyController = new SpecialtiesController())
            {
                result = specialtyController.DeleteSpecialty(createdSpecialtyID);
            }

        }

        [TestMethod]
        public void GetEmergencyContactsForPatientReturnsEmergencyContacts()
        {
            int patientIDForTest;
            int createdEmergencyContactID;

            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<PatientModel> createdContentResult;
            OkNegotiatedContentResult<IEnumerable<EmergencyContactModel>> OkEmergencyContactContentResult;

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
                    CreatedDate = new DateTime(2015, 11, 10),
                    Archived = false
                };
                result = patientController.PostPatient(patient);
                createdContentResult =
                    (CreatedAtRouteNegotiatedContentResult<PatientModel>)result;
                patientIDForTest = createdContentResult.Content.PatientID;
            }

            // Create emergency contact for patient
            using (var emergencyContactController = new EmergencyContactsController())
            {
                var emergencyContact = new EmergencyContactModel
                {
                    PatientID = patientIDForTest,
                    FirstName = "Tester",
                    LastName = "Testerson",
                    Telephone = "555-1212",
                    Email = "c@d.com",
                    Relationship = "Probation Officer"
                };

                result = emergencyContactController.PostEmergencyContact(emergencyContact);
                CreatedAtRouteNegotiatedContentResult<EmergencyContactModel> emergencyContactContentResult =
                (CreatedAtRouteNegotiatedContentResult<EmergencyContactModel>)result;
                createdEmergencyContactID = emergencyContactContentResult.Content.EmergencyContactID;
            }

            // Call GetEmergencyContactsForPatient
            using (var patientController = new PatientsController())
            {
                result = patientController.GetEmergencyContactsForPatient(patientIDForTest);

                // Verify that HTTP status code is OK
                Assert.IsInstanceOfType(result,
                typeof(OkNegotiatedContentResult<IEnumerable<EmergencyContactModel>>));

                // Verify that at least one element was returned
                OkEmergencyContactContentResult =
                (OkNegotiatedContentResult<IEnumerable<EmergencyContactModel>>)result;
                Assert.IsTrue(OkEmergencyContactContentResult.Content.Count() > 0);

                // Verify that all the returned emergency contact(s) correspond to the test patient ID
                Assert.IsTrue(OkEmergencyContactContentResult.Content.
                    Where(ec => ec.PatientID != patientIDForTest).Count() == 0);
            }

            // Delete the test patient (actual deletion, not archiving) 
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {                
                Patient dbPatient = db.Patients.Find(patientIDForTest);
                db.Patients.Remove(dbPatient);
                db.SaveChanges();
            }

            //Delete the test EmergencyContact
            using (var emergencyContactController = new EmergencyContactsController())
            {
                result =
                    emergencyContactController.DeleteEmergencyContact(createdEmergencyContactID);
            }

        }

        [TestMethod]
        public void GetPatientChecksForPatientReturnsPatientChecks()
        {
            int patientIDForTest;
            int createdSpecialtyID;
            int createdPatientCheckID;

            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<PatientModel> createdContentResult;
            OkNegotiatedContentResult<IEnumerable<PatientCheckModel>> OkPatientCheckContentResult;

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
                    CreatedDate = new DateTime(2015, 11, 10),
                    Archived = false
                };
                result = patientController.PostPatient(patient);
                createdContentResult =
                    (CreatedAtRouteNegotiatedContentResult<PatientModel>)result;
                patientIDForTest = createdContentResult.Content.PatientID;
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

            // Create patient check-in for patient
            using (var patientCheckController = new PatientChecksController())
            {
                var patientCheck = new PatientCheckModel
                {
                    PatientID = patientIDForTest,
                    SpecialtyID = createdSpecialtyID,                    
                    CheckinDateTime = DateTime.Now,
                    CheckoutDateTime = DateTime.Now
                };

                result = patientCheckController.PostPatientCheck(patientCheck);
                CreatedAtRouteNegotiatedContentResult<PatientCheckModel> patientCheckContentResult =
                (CreatedAtRouteNegotiatedContentResult<PatientCheckModel>)result;
                createdPatientCheckID = patientCheckContentResult.Content.PatientCheckID;
            }

            // Call GetPatientChecksForPatient
            using (var patientController = new PatientsController())
            {
                result = patientController.GetPatientChecksForPatient(patientIDForTest);

                // Verify that HTTP status code is OK
                Assert.IsInstanceOfType(result,
                typeof(OkNegotiatedContentResult<IEnumerable<PatientCheckModel>>));

                // Verify that at least one element was returned
                OkPatientCheckContentResult =
                (OkNegotiatedContentResult<IEnumerable<PatientCheckModel>>)result;
                Assert.IsTrue(OkPatientCheckContentResult.Content.Count() > 0);

                // Verify that all the returned patientCheck(s) correspond to the test patient ID
                Assert.IsTrue(OkPatientCheckContentResult.Content.
                    Where(a => a.PatientID != patientIDForTest).Count() == 0);
            }

            // Delete the test patient check-in
            using (var patientCheckController = new PatientChecksController())
            {
                result = patientCheckController.DeletePatientCheck(createdPatientCheckID);
            }

            // Delete the test patient (actual deletion, not archiving) 
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {               
                Patient dbPatient = db.Patients.Find(patientIDForTest);
                db.Patients.Remove(dbPatient);
                db.SaveChanges();
            }

            // Delete the test specialty
            using (var specialtyController = new SpecialtiesController())
            {
                result = specialtyController.DeleteSpecialty(createdSpecialtyID);
            }
        }

        [TestMethod]
        public void PostPatientCreatesPatient()
        {
            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<PatientModel> contentResult;

            //Arrange: Instantiate PatientsController so its methods can be called
            using (var patientController = new PatientsController())
            {
                //Act: 
                // Create a PatientModel object populated with test data,
                //  and call PostPatient
                var newPatient = new PatientModel
                {
                    FirstName = "Impatient",
                    LastName = "Patience",
                    Birthdate = new DateTime(1968, 12, 27),
                    Email = "a@b.com",
                    BloodType = "A+",
                    CreatedDate = new DateTime(2015, 11, 10),
                    Archived = false
                };
                result = patientController.PostPatient(newPatient);

                //Assert:
                // Verify that the HTTP result is CreatedAtRouteNegotiatedContentResult
                // Verify that the HTTP result body contains a nonzero patient ID
                Assert.IsInstanceOfType
                    (result, typeof(CreatedAtRouteNegotiatedContentResult<PatientModel>));
                contentResult =
                    (CreatedAtRouteNegotiatedContentResult<PatientModel>)result;
                Assert.IsTrue(contentResult.Content.PatientID != 0);
            }

            // Remove the patient from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Patient dbPatient = db.Patients.Find(contentResult.Content.PatientID);
                db.Patients.Remove(dbPatient);
                db.SaveChanges();
            }
        }

        [TestMethod]
        public void PutPatientUpdatesPatient()
        {
            int patientIDForTest;
            string patientFirstNameForTest = "Testy";
            string patientLastNameForTest = "McTesterson";

            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<PatientModel> createdContentResult;
            OkNegotiatedContentResult<PatientModel> OkContentResult;
            PatientModel updatedPatient;

            //Arrange: Add new patient, and save its patient ID
            using (var patientController = new PatientsController())
            {
                var newPatient = new PatientModel
                {
                    FirstName = "Impatient",
                    LastName = "Patience",
                    Birthdate = new DateTime(1968, 12, 27),
                    Email = "a@b.com",
                    BloodType = "A+",
                    CreatedDate = new DateTime(2015, 11, 10),
                    Archived = false
                };
                result = patientController.PostPatient(newPatient);
                createdContentResult =
                    (CreatedAtRouteNegotiatedContentResult<PatientModel>)result;
                patientIDForTest = createdContentResult.Content.PatientID;
            }

            // Get the patient from the DB
            using (var patientController = new PatientsController())
            {
                result = patientController.GetPatient(patientIDForTest);
                OkContentResult =
                    (OkNegotiatedContentResult<PatientModel>)result;               
                updatedPatient = (PatientModel)createdContentResult.Content;
            }

            // Change the patient and pass it to PutPatient                      
            using (var patientController = new PatientsController())
            {
                updatedPatient.FirstName = patientFirstNameForTest;
                updatedPatient.LastName = patientLastNameForTest;

                result = patientController.PutPatient
                                         (updatedPatient.PatientID, updatedPatient);
            }

            // Verify that HTTP status code is OK
            // Get the patient and verify that it was updated

            var statusCode = (StatusCodeResult)result;
            Assert.IsTrue(statusCode.StatusCode == System.Net.HttpStatusCode.NoContent);

            using (var patientController = new PatientsController())
            {
                result = patientController.GetPatient(patientIDForTest);

                Assert.IsInstanceOfType(result,
                    typeof(OkNegotiatedContentResult<PatientModel>));

                OkContentResult =
                    (OkNegotiatedContentResult<PatientModel>)result;
                updatedPatient = (PatientModel)OkContentResult.Content;
            }
            
            Assert.IsTrue(updatedPatient.FirstName == patientFirstNameForTest);
            Assert.IsTrue(updatedPatient.LastName == patientLastNameForTest);

            // Remove the patient from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Patient dbPatient = db.Patients.Find(patientIDForTest);
                db.Patients.Remove(dbPatient);
                db.SaveChanges();
            }
        }

        [TestMethod]
        // Test that DeletePatient sets patient's archived indicator to true
        public void DeletePatientDeletesPatient()
        {
            int patientIDForTest;
                      
            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<PatientModel> createdContentResult;
            OkNegotiatedContentResult<PatientModel> OkcontentResult;

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
                    CreatedDate = new DateTime(2015, 11, 10),
                    Archived = false
                };
                result = patientController.PostPatient(patient);
                createdContentResult =
                    (CreatedAtRouteNegotiatedContentResult<PatientModel>)result;
                patientIDForTest = createdContentResult.Content.PatientID;
            }

            //Call the procedure to delete the patient, which sets its archived indicator to true
            using (var patientController = new PatientsController())
            {
                result = patientController.DeletePatient(patientIDForTest);

                // Verify that HTTP result is OK
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<PatientModel>));
                // Verify that the returned PatientModel object has archived indicator set to true
                OkcontentResult =
                    (OkNegotiatedContentResult<PatientModel>)result;
                Assert.IsTrue(OkcontentResult.Content.Archived);
            }

            // Get the patient and verify that the patient has archived indicator set to true
            using (var patientController = new PatientsController())
            {
                result = patientController.GetPatient(patientIDForTest);
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<PatientModel>));
                OkcontentResult =
                    (OkNegotiatedContentResult<PatientModel>)result;
                Assert.IsTrue(OkcontentResult.Content.Archived);
            }

            // Remove the patient from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Patient dbPatient = db.Patients.Find(patientIDForTest);
                db.Patients.Remove(dbPatient);
                db.SaveChanges();
            }
        }
    }
}
