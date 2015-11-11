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
