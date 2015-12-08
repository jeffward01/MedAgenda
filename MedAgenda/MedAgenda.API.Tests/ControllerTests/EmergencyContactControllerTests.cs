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
    /// <summary>
    /// Summary description for EmergencyContactControllerTests
    /// </summary>
    [TestClass]
    public class EmergencyContactTests : BaseTest
    {


        [TestMethod] // Get all EmergencyContacts | [0]
        public void GetEmergencyContactsReturnEmergencyContacts()
        {
            //Arrange
            using (var EmergencyContactController = new EmergencyContactsController())
            {

                //Act: Call the GetEmergencyContact Method
                IEnumerable<EmergencyContactModel> emergencyContacts = EmergencyContactController.GetEmergencyContacts();

                if(!emergencyContacts.Any()) Assert.Inconclusive();

                //Assert
                Assert.IsTrue(emergencyContacts.Count() > 0);
            }
        }

        [TestMethod] //Get EmergencyContact by ID | [1]
        public void GetEmergencyContactReturnEmergencyContact()
        {
            int createdPatientID;
            int emergencyContactIDForTest;

            // Create test patient and emergency contact
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
                IHttpActionResult result = patientController.PostPatient(patient);
                CreatedAtRouteNegotiatedContentResult<PatientModel> createdContentResult =
                    (CreatedAtRouteNegotiatedContentResult<PatientModel>)result;
                createdPatientID = createdContentResult.Content.PatientID;
            }

            using (var EmergencyContactController = new EmergencyContactsController())
            {
                //Create EmergencyContact
                var newEmergencyContact = new EmergencyContactModel
                {                   
                    PatientID = createdPatientID,
                    FirstName = "Ronnie",
                    LastName = "Dio",
                    Telephone = "666-666-6666",
                    Email = "LastInLine@HolyDiver.com",
                    Relationship = "Celestial Being"
                };

                //Act
                IHttpActionResult result = EmergencyContactController.PostEmergencyContact(newEmergencyContact);
               
                CreatedAtRouteNegotiatedContentResult<EmergencyContactModel> contentResult = 
                    (CreatedAtRouteNegotiatedContentResult<EmergencyContactModel>)result;
                emergencyContactIDForTest = contentResult.Content.EmergencyContactID;
            }

            // Assert: Get the test emergency contact and verify the ID
            using (var EmergencyContactController = new EmergencyContactsController())
            {

                IHttpActionResult result = EmergencyContactController.GetEmergencyContact(emergencyContactIDForTest);

                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<EmergencyContactModel>));

                OkNegotiatedContentResult<EmergencyContactModel> contentResult = 
                    (OkNegotiatedContentResult<EmergencyContactModel>)result;
                Assert.IsTrue(contentResult.Content.EmergencyContactID == emergencyContactIDForTest);
            }

            //Delete the Test EmergencyContact
            using (var emergencyContactController = new EmergencyContactsController())
            {
                IHttpActionResult result = 
                    emergencyContactController.DeleteEmergencyContact(emergencyContactIDForTest);
            }

            // Remove the test patient from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Patient dbPatient = db.Patients.Find(createdPatientID);
                db.Patients.Remove(dbPatient);
                db.SaveChanges();
            }

        }

        [TestMethod] // Create EmergencyContact [2]
        public void PostEmergencyContactCreateEmergencyContact()
        {
            int createdPatientID;
            int emergencyContactIDForTest;

            // Create test patient and emergency contact
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
                IHttpActionResult result = patientController.PostPatient(patient);
                CreatedAtRouteNegotiatedContentResult<PatientModel> createdContentResult =
                    (CreatedAtRouteNegotiatedContentResult<PatientModel>)result;
                createdPatientID = createdContentResult.Content.PatientID;
            }

            //Arrange
            using (var EmergencyContactController = new EmergencyContactsController())
            {
                //Create EmergencyContact
                var newEmergencyContact = new EmergencyContactModel
                {
                    PatientID = createdPatientID,
                    FirstName = "Ronnie",
                    LastName = "Dio",
                    Telephone = "666-666-6666",
                    Email = "LastInLine@HolyDiver.com",
                    Relationship = "Celestial Being"
                };

                //Act
                IHttpActionResult result = EmergencyContactController.PostEmergencyContact(newEmergencyContact);

                //Assert
                Assert.IsInstanceOfType(result, typeof(CreatedAtRouteNegotiatedContentResult<EmergencyContactModel>));

                CreatedAtRouteNegotiatedContentResult<EmergencyContactModel> contentResult = 
                    (CreatedAtRouteNegotiatedContentResult<EmergencyContactModel>)result;

                Assert.IsTrue(contentResult.Content.EmergencyContactID != 0);
                emergencyContactIDForTest = contentResult.Content.EmergencyContactID;
          }

            //Delete the test EmergencyContact
            using (var emergencyContactController = new EmergencyContactsController())
            {
                IHttpActionResult result =
                    emergencyContactController.DeleteEmergencyContact(emergencyContactIDForTest);
            }

            // Remove the test patient from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Patient dbPatient = db.Patients.Find(createdPatientID);
                db.Patients.Remove(dbPatient);
                db.SaveChanges();
            }
        }


        [TestMethod] //Update EmergencyContact [3]
        public void PutEmergencyContactUpdateEmergencyContact()
        {            
            //Test Properties
            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<EmergencyContactModel> contentResult;
            OkNegotiatedContentResult<EmergencyContactModel> emergencyContactResult;
            OkNegotiatedContentResult<EmergencyContactModel> readContentResult;

            int createdPatientID;

            // Create test patient 
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

            using (var EmergencyContactController = new EmergencyContactsController())
            {
                //Create EmergencyContact
                var newEmergencyContact = new EmergencyContactModel
                {                    
                    PatientID = createdPatientID,
                    FirstName = "Ronnie",
                    LastName = "Dio",
                    Telephone = "666-666-6666",
                    Email = "LastInLine@HolyDiver.com",
                    Relationship = "Celestial Being"
                };
                //Insert EmergencyContactModelObject into Database so 
                //that I can take it out and test for update.
                result = EmergencyContactController.PostEmergencyContact(newEmergencyContact);

                //Cast result as Content Result so that I can gather information from ContentResult
                contentResult = (CreatedAtRouteNegotiatedContentResult<EmergencyContactModel>)result;
            }

            using (var SecondEmergencyContactController = new EmergencyContactsController())
            {
                //Result contains the new EmergencyContact
                result = SecondEmergencyContactController.GetEmergencyContact(contentResult.Content.EmergencyContactID);

                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<EmergencyContactModel>));

                //Get EmergencyContactModel from 'result'
                emergencyContactResult = (OkNegotiatedContentResult<EmergencyContactModel>)result;

            }

            using (var ThirdEmergencyContactController = new EmergencyContactsController())
            {
                var modifiedEmergencyContact = emergencyContactResult.Content;

                modifiedEmergencyContact.FirstName = "Whip";

                //Act
                //The result of the Put Request
                result = ThirdEmergencyContactController.PutEmergencyContact(emergencyContactResult.Content.EmergencyContactID, modifiedEmergencyContact);

                //Assert
                Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            }


            using (var FourthEmergencyContactController = new EmergencyContactsController())
            {
                //Act
                IHttpActionResult resultAlteredEmergencyContact = FourthEmergencyContactController.GetEmergencyContact(emergencyContactResult.Content.EmergencyContactID);

                OkNegotiatedContentResult<EmergencyContactModel> alteredResult = (OkNegotiatedContentResult<EmergencyContactModel>)resultAlteredEmergencyContact;
                EmergencyContactModel updatedEmergencyContact = (EmergencyContactModel)alteredResult.Content;

                //Assert
                Assert.IsInstanceOfType(resultAlteredEmergencyContact, typeof(OkNegotiatedContentResult<EmergencyContactModel>));

                readContentResult =
                    (OkNegotiatedContentResult<EmergencyContactModel>)resultAlteredEmergencyContact;

                Assert.IsTrue(readContentResult.Content.FirstName == "Whip");
            }

            using (var FifthEmergencyContactController = new EmergencyContactsController())
            {
                //Delete the Test EmergencyContact
                result = FifthEmergencyContactController.DeleteEmergencyContact(readContentResult.Content.EmergencyContactID);
            }

            // Remove the test patient from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Patient dbPatient = db.Patients.Find(createdPatientID);
                db.Patients.Remove(dbPatient);
                db.SaveChanges();
            }
        }

        [TestMethod] // Delete EmergencyContact [4]
        public void DeleteEmergencyContact()
        {
            CreatedAtRouteNegotiatedContentResult<EmergencyContactModel> contentResult;
            int createdPatientID;

            // Create test patient 
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
                IHttpActionResult result = patientController.PostPatient(patient);
                CreatedAtRouteNegotiatedContentResult<PatientModel> createdContentResult =
                    (CreatedAtRouteNegotiatedContentResult<PatientModel>)result;
                createdPatientID = createdContentResult.Content.PatientID;
            }

            using (var EmergencyContactController = new EmergencyContactsController())
            {
                //Create EmergencyContact
                var newEmergencyContact = new EmergencyContactModel
                {
                    PatientID = createdPatientID,
                    FirstName = "Ronnie",
                    LastName = "Dio",
                    Telephone = "666-666-6666",
                    Email = "LastInLine@HolyDiver.com",
                    Relationship = "Celestial Being"
                };

                //Insert object to be removed by test
                var result = EmergencyContactController.PostEmergencyContact(newEmergencyContact);

                //Cast result as Content Result so that I can gather information from ContentResult
                contentResult = (CreatedAtRouteNegotiatedContentResult<EmergencyContactModel>)result;
            }

            using (var secondDocController = new EmergencyContactsController())
            {
                //Delete the Test EmergencyContact
                var result = secondDocController.DeleteEmergencyContact(contentResult.Content.EmergencyContactID);

                //Assert
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<EmergencyContactModel>));
            }
            using (var thirdDocController = new EmergencyContactsController())
            {
                var result = thirdDocController.GetEmergencyContact(contentResult.Content.EmergencyContactID);
                //Assert
                Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            }

            // Remove the test patient from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Patient dbPatient = db.Patients.Find(createdPatientID);
                db.Patients.Remove(dbPatient);
                db.SaveChanges();
            }
        }

    }
}
