using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MedAgenda.API.Tests.Infrastructure;
using MedAgenda.API.Controllers;
using System.Collections.Generic;
using System.Linq;
using MedAgenda.CORE.Models;
using System.Web.Http;
using System.Web.Http.Results;

namespace MedAgenda.API.Tests.ControllerTests
{
    [TestClass]
    public class PatientControllerTests : BaseTest
    {
        [TestMethod]
        public void GetPatientsReturnsPatients()
        {

            //Arrange: Instantiate PatientsController so its methods can be called
            using (var patientController = new PatientsController())
            {
                //Act: Call the GetPatients method
                IEnumerable<PatientModel> patients = patientController.GetPatients();

                //Assert: Verify that an array was returned with at least one element
                Assert.IsTrue(patients.Count() > 0);
            }
        }

        [TestMethod]
        public void GetPatientReturnsPatient()
        {
            int PatientIDForTest = 1;

            //Arrange: Instantiate PatientsController so its methods can be called
            var patientController = new PatientsController();

            //Act: Call the GetPatient method
            IHttpActionResult result = patientController.GetPatient(PatientIDForTest);

            //Assert: 
            // Verify that HTTP status code is OK
            // Verify that returned patient ID is correct
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<PatientModel>));

            OkNegotiatedContentResult<PatientModel> contentResult =
                (OkNegotiatedContentResult<PatientModel>)result;
            Assert.IsTrue(contentResult.Content.PatientID == PatientIDForTest);
        }

        [TestMethod]
        public void PostPatientCreatesPatient()
        {
            //Arrange: Instantiate PatientsController so its methods can be called
            var patientController = new PatientsController();

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
                CreatedDate = new DateTime(2015, 11, 10)               
            };
            IHttpActionResult result = patientController.PostPatient(newPatient);

            //Assert:
            // Verify that the HTTP result is CreatedAtRouteNegotiatedContentResult
            // Verify that the HTTP result body contains a nonzero patient ID
            Assert.IsInstanceOfType
                (result, typeof(CreatedAtRouteNegotiatedContentResult<PatientModel>));
            CreatedAtRouteNegotiatedContentResult<PatientModel> contentResult =
                (CreatedAtRouteNegotiatedContentResult<PatientModel>)result;
            Assert.IsTrue(contentResult.Content.PatientID != 0);

            // Delete the test patient 
            result = patientController.DeletePatient(contentResult.Content.PatientID);
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
                    CreatedDate = new DateTime(2015, 11, 10)
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

            // Get the patient, change it, and pass it to PutPatient                      
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

            // Delete the test patient
            using (var patientController = new PatientsController())
            {
                result = patientController.DeletePatient(patientIDForTest);
            }
                
        }

        [TestMethod]
        public void DeletePatientDeletesPatient()
        {
            int patientIDForTest;
            int doctorIDForTest = 1;
            int examRoomIDForTest = 1;
            int specialtyIDForTest = 1;
            int createdAppointmentID;
            int createdEmergencyContactID;
            int createdPatientCheckID;
            DateTime nowTime = DateTime.Now;

            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<PatientModel> createdContentResult;            

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
                    CreatedDate = new DateTime(2015, 11, 10)
                };
                result = patientController.PostPatient(patient);
                createdContentResult =
                    (CreatedAtRouteNegotiatedContentResult<PatientModel>)result;
                patientIDForTest = createdContentResult.Content.PatientID;
            }

            // Add an emergency contact corresponding to the patient          
            using (var emergencyContactController = new EmergencyContactsController())
            {
                var emergencyContact = new EmergencyContactModel
                {
                    PatientID = patientIDForTest,
                    FirstName = "Abe",
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

            // Add a patient check-in corresponding to the patient          
            using (var patientCheckController = new PatientChecksController())
            {
                var patientCheck = new PatientCheckModel
                {
                    PatientID = patientIDForTest,
                    SpecialtyID = specialtyIDForTest,                   
                    CheckinDateTime = nowTime
                };
                result = patientCheckController.PostPatientCheck(patientCheck);
                CreatedAtRouteNegotiatedContentResult<PatientCheckModel> patientCheckContentResult =
                    (CreatedAtRouteNegotiatedContentResult<PatientCheckModel>)result;
                createdPatientCheckID = patientCheckContentResult.Content.PatientCheckID;
            }

            // Add an appointment corresponding to the patient          
            using (var appointmentController = new AppointmentsController())
            {
                var appointment = new AppointmentModel
                {                   
                    PatientID = patientIDForTest,
                    DoctorID = doctorIDForTest,
                    ExamRoomID = examRoomIDForTest,
                    CheckinDateTime = nowTime,
                };
                result = appointmentController.PostAppointment(appointment);
                CreatedAtRouteNegotiatedContentResult<AppointmentModel> appointmentContentResult =
                    (CreatedAtRouteNegotiatedContentResult<AppointmentModel>)result;
                createdAppointmentID = appointmentContentResult.Content.AppointmentID;
            }


            //Delete the patient
            using (var patientController = new PatientsController())
            {
                result = patientController.DeletePatient(patientIDForTest);

                // Verify that HTTP result is OK
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<PatientModel>));
            }

            // Verify that reading deleted patient returns result not found
            using (var patientController = new PatientsController())
            {
                result = patientController.GetPatient(patientIDForTest);
                Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            }

            // Verify that the emergency contact created above was deleted
            using (var emergencyContactController = new EmergencyContactsController())
            {
                result = emergencyContactController.GetEmergencyContact(createdEmergencyContactID);
                Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            }

            // Verify that the patient check-in created above was deleted
            using (var patientCheckController = new PatientChecksController())
            {
                result = patientCheckController.GetPatientCheck(createdPatientCheckID);
                Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            }

            // Verify that the appointment created above was deleted
            using (var appointmentController = new AppointmentsController())
            {
                result = appointmentController.GetAppointment(createdAppointmentID);
                Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            }

        }
    }
}
