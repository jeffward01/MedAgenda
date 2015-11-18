using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MedAgenda.API.Tests.Infrastructure;
using MedAgenda.API.Controllers;
using MedAgenda.CORE.Models;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using MedAgenda.CORE.Infrastructure;
using MedAgenda.CORE.Domain;

namespace MedAgenda.API.Tests.ControllerTests
{
    [TestClass]
    public class PatientCheckControllerTests : BaseTest
    {
        [TestMethod] //Get all PatientChecks [0]
        public void GetPatientChecksReturnPatientChecks()
        {
            using (var patientCheckController = new PatientChecksController())
            {
                IEnumerable<PatientCheckModel> patientChecks = patientCheckController.GetPatientChecks();

                if (patientChecks.Count() == 0) Assert.Inconclusive("Patient checks table is empty");

                Assert.IsTrue(patientChecks.Count() > 0);
            }
        }

        [TestMethod] //Get a PatientCheck [1]
        public void GetPatientCheckReturnPatientChecks()
        {
            int createdPatientID;
            int createdSpecialtyID;
            int patientCheckIDForTest;
            IHttpActionResult result;

            // Create a new test patient, and get its patient ID
            using (var patientController = new PatientsController())
            {
                var patient = new PatientModel
                {
                    FirstName = "Testpatient",
                    LastName = "Testerson",
                    Birthdate = new DateTime(1968, 12, 27),
                    Email = "a@b.com",
                    BloodType = "A+",
                    CreatedDate = new DateTime(2015, 11, 10),
                    Archived = false
                };
                result = patientController.PostPatient(patient);
                CreatedAtRouteNegotiatedContentResult<PatientModel> createdContentResult =
                    (CreatedAtRouteNegotiatedContentResult<PatientModel>)result;
                createdPatientID = createdContentResult.Content.PatientID;
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

            // Create test patient check, and save its ID
            using (var patientCheckController = new PatientChecksController())
            {
                var newPatientCheck = new PatientCheckModel
                {
                    PatientID = createdPatientID,
                    SpecialtyID = createdSpecialtyID,
                    CheckinDateTime = DateTime.Now,
                    CheckoutDateTime = DateTime.Now.AddHours(2)
                };

                result = patientCheckController.PostPatientCheck(newPatientCheck);
                CreatedAtRouteNegotiatedContentResult<PatientCheckModel> contentResult =
                    (CreatedAtRouteNegotiatedContentResult<PatientCheckModel>)result;
                patientCheckIDForTest = contentResult.Content.PatientCheckID;
            }

            // Get the test patient check, and verify that it has the same ID
            using (var patientCheckController = new PatientChecksController())
            {
                result = patientCheckController.GetPatientCheck(patientCheckIDForTest);

                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<PatientCheckModel>));

                OkNegotiatedContentResult<PatientCheckModel> contentResult =
                                (OkNegotiatedContentResult<PatientCheckModel>)result;

                Assert.IsTrue(contentResult.Content.PatientCheckID == patientCheckIDForTest);
            }

            // Delete the test patient check-in
            using (var patientCheckController = new PatientChecksController())
            {
                result = patientCheckController.DeletePatientCheck(patientCheckIDForTest);
            }

            // Remove the test patient from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Patient dbPatient = db.Patients.Find(createdPatientID);
                db.Patients.Remove(dbPatient);
                db.SaveChanges();
            }

            // Delete the test specialty
            using (var specialtyController = new SpecialtiesController())
            {
                result = specialtyController.DeleteSpecialty(createdSpecialtyID);
            }
        }

        [TestMethod] //Create a PatientCheck [2]
        public void PostPatientCheck()
        {
            int createdPatientID;
            int createdSpecialtyID;
            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<PatientCheckModel> contentResult;

            // Create a new test patient, and get its patient ID
            using (var patientController = new PatientsController())
            {
                var patient = new PatientModel
                {
                    FirstName = "Testpatient",
                    LastName = "Testerson",
                    Birthdate = new DateTime(1968, 12, 27),
                    Email = "a@b.com",
                    BloodType = "A+",
                    CreatedDate = new DateTime(2015, 11, 10),
                    Archived = false
                };
                result = patientController.PostPatient(patient);
                CreatedAtRouteNegotiatedContentResult<PatientModel> createdContentResult =
                    (CreatedAtRouteNegotiatedContentResult<PatientModel>)result;
                createdPatientID = createdContentResult.Content.PatientID;
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

            using (var patientCheckController = new PatientChecksController())
            {
                var newPatientCheck = new PatientCheckModel
                {
                    PatientID = createdPatientID,
                    SpecialtyID = createdSpecialtyID,
                    CheckinDateTime = DateTime.Now,
                    CheckoutDateTime = DateTime.Now.AddHours(2)
                };

                result = patientCheckController.PostPatientCheck(newPatientCheck);

                Assert.IsInstanceOfType(result, typeof(CreatedAtRouteNegotiatedContentResult<PatientCheckModel>));

                contentResult = (CreatedAtRouteNegotiatedContentResult<PatientCheckModel>)result;

                Assert.IsTrue(contentResult.Content.PatientCheckID != 0);
            }

            //Delete the PatientCheck
            using (var SecondPatientCheckController = new PatientChecksController())
            {
                result = SecondPatientCheckController.DeletePatientCheck(contentResult.Content.PatientCheckID);
            }

            // Delete the test patient (actual deletion, not archiving) 
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Patient dbPatient = db.Patients.Find(createdPatientID);
                db.Patients.Remove(dbPatient);
                db.SaveChanges();
            }

            // Delete the test specialty
            using (var specialtyController = new SpecialtiesController())
            {
                result = specialtyController.DeleteSpecialty(createdSpecialtyID);
            }
        }

        [TestMethod] //Update PatientCheck [3]
        public void PutPatientCheckReturnPatientCheck()
        {
            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<PatientCheckModel> contentResult;
            OkNegotiatedContentResult<PatientCheckModel> patientCheckResult;
            OkNegotiatedContentResult<PatientCheckModel> readContentResult;
            DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 
                                        DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            int createdPatientID;
            int createdSpecialtyID;

            // Create a new test patient, and get its patient ID
            using (var patientController = new PatientsController())
            {
                var patient = new PatientModel
                {
                    FirstName = "Testpatient",
                    LastName = "Testerson",
                    Birthdate = new DateTime(1968, 12, 27),
                    Email = "a@b.com",
                    BloodType = "A+",
                    CreatedDate = new DateTime(2015, 11, 10),
                    Archived = false
                };
                result = patientController.PostPatient(patient);
                CreatedAtRouteNegotiatedContentResult<PatientModel> createdContentResult =
                    (CreatedAtRouteNegotiatedContentResult<PatientModel>)result;
                createdPatientID = createdContentResult.Content.PatientID;
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

            using (var patientCheckController = new PatientChecksController())
            {
                var newPatientCheck = new PatientCheckModel
                {
                    PatientID = createdPatientID,
                    SpecialtyID = createdSpecialtyID,
                    CheckinDateTime = now,
                    CheckoutDateTime = now.AddHours(2)
                };

                result = patientCheckController.PostPatientCheck(newPatientCheck);

                contentResult = (CreatedAtRouteNegotiatedContentResult<PatientCheckModel>)result;

            }
            using (var SecondPatientCheckController = new PatientChecksController())
            {
                result = SecondPatientCheckController.GetPatientCheck(contentResult.Content.PatientCheckID);

                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<PatientCheckModel>));

                patientCheckResult = (OkNegotiatedContentResult<PatientCheckModel>)result;
            }
            using (var ThirdPatientCheckController = new PatientChecksController())
            {
                var modifiedPatientCheck = patientCheckResult.Content;

                modifiedPatientCheck.CheckoutDateTime = now;

                result = ThirdPatientCheckController.PutPatientCheck(patientCheckResult.Content.PatientCheckID, modifiedPatientCheck);

                Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            }

            using (var FourthPatientCheckController = new PatientChecksController())
            {
                IHttpActionResult resultAlteredPatientCheck = FourthPatientCheckController.GetPatientCheck(patientCheckResult.Content.PatientCheckID);

                OkNegotiatedContentResult<PatientCheckModel> alteredResult = (OkNegotiatedContentResult<PatientCheckModel>)resultAlteredPatientCheck;

                PatientCheckModel updatedPatientCheck = (PatientCheckModel)alteredResult.Content;

                Assert.IsInstanceOfType(resultAlteredPatientCheck, typeof(OkNegotiatedContentResult<PatientCheckModel>));

                readContentResult = (OkNegotiatedContentResult<PatientCheckModel>)resultAlteredPatientCheck;

                Assert.IsTrue(readContentResult.Content.CheckoutDateTime == now);
            }

            // Delete the test patient check
            using (var FifthPatientCheckController = new PatientChecksController())
            {
                result = FifthPatientCheckController.DeletePatientCheck(readContentResult.Content.PatientCheckID);
            }

            // Delete the test patient (actual deletion, not archiving) 
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Patient dbPatient = db.Patients.Find(createdPatientID);
                db.Patients.Remove(dbPatient);
                db.SaveChanges();
            }

            // Delete the test specialty
            using (var specialtyController = new SpecialtiesController())
            {
                result = specialtyController.DeleteSpecialty(createdSpecialtyID);
            }
        }

        [TestMethod] //Delete PatientCheck [4]
        public void DeletePatientCheck()
        {
            int createdPatientID;
            int createdSpecialtyID;
            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<PatientCheckModel> contentResult;

            // Create a new test patient, and get its patient ID
            using (var patientController = new PatientsController())
            {
                var patient = new PatientModel
                {
                    FirstName = "Testpatient",
                    LastName = "Testerson",
                    Birthdate = new DateTime(1968, 12, 27),
                    Email = "a@b.com",
                    BloodType = "A+",
                    CreatedDate = new DateTime(2015, 11, 10),
                    Archived = false
                };
                result = patientController.PostPatient(patient);
                CreatedAtRouteNegotiatedContentResult<PatientModel> createdContentResult =
                    (CreatedAtRouteNegotiatedContentResult<PatientModel>)result;
                createdPatientID = createdContentResult.Content.PatientID;
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

            using (var patientCheckController = new PatientChecksController())
            {
                var newPatientCheck = new PatientCheckModel
                {
                    PatientID = createdPatientID,
                    SpecialtyID = createdSpecialtyID,
                    CheckinDateTime = DateTime.Now,
                    CheckoutDateTime = DateTime.Now.AddHours(2)
                };
                result = patientCheckController.PostPatientCheck(newPatientCheck);

                contentResult = (CreatedAtRouteNegotiatedContentResult<PatientCheckModel>)result;
            }
            using (var SecondPatientCheckController = new PatientChecksController())
            {
                result = SecondPatientCheckController.DeletePatientCheck(contentResult.Content.PatientCheckID);

                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<PatientCheckModel>));
            }
            using (var ThirdPatientCheckController = new PatientChecksController())
            {
                result = ThirdPatientCheckController.GetPatientCheck(contentResult.Content.PatientCheckID);

                Assert.IsInstanceOfType(result, typeof(NotFoundResult));

            }

            // Delete the test patient (actual deletion, not archiving) 
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Patient dbPatient = db.Patients.Find(createdPatientID);
                db.Patients.Remove(dbPatient);
                db.SaveChanges();
            }

            // Delete the test specialty
            using (var specialtyController = new SpecialtiesController())
            {
                result = specialtyController.DeleteSpecialty(createdSpecialtyID);
            }
        }
    }
}

