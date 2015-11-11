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

                //Assert
                Assert.IsTrue(emergencyContacts.Count() > 0);
            }
        }

        [TestMethod] //Get EmergencyContact by ID | [1]
        public void GetEmergencyContactReturnEmergencyContact()
        {
            //Arrange
            using (var emergencyContactController = new EmergencyContactsController())
            {
                //Act
                IHttpActionResult result = emergencyContactController.GetEmergencyContact(1);

                //Assert
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<EmergencyContactModel>));

                OkNegotiatedContentResult<EmergencyContactModel> contentResult = (OkNegotiatedContentResult<EmergencyContactModel>)result;

                Assert.IsTrue(contentResult.Content.EmergencyContactID == 1);
            }

        }

        [TestMethod] // Create EmergencyContact [2]
        public void PostEmergencyContactCreateEmergencyContact()
        {
            //Arrange
            using (var EmergencyContactController = new EmergencyContactsController())
            {
                //Create EmergencyContact
                var newEmergencyContact = new EmergencyContactModel
                {
                    EmergencyContactID = 1,
                    PatientID = 1, 
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

                CreatedAtRouteNegotiatedContentResult<EmergencyContactModel> contentResult = (CreatedAtRouteNegotiatedContentResult<EmergencyContactModel>)result;

                Assert.IsTrue(contentResult.Content.EmergencyContactID != 0);

                //Delete the Test EmergencyContact
                result = EmergencyContactController.DeleteEmergencyContact(contentResult.Content.EmergencyContactID);
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


            using (var EmergencyContactController = new EmergencyContactsController())
            {
                //Create EmergencyContact
                var newEmergencyContact = new EmergencyContactModel
                {
                    EmergencyContactID = 1,
                    PatientID = 1,
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
        }

        [TestMethod] // Delete EmergencyContact [4]
        public void DeleteEmergencyContact()
        {
            CreatedAtRouteNegotiatedContentResult<EmergencyContactModel> contentResult;


            using (var EmergencyContactController = new EmergencyContactsController())
            {
                //Create EmergencyContact
                var newEmergencyContact = new EmergencyContactModel
                {
                    EmergencyContactID = 1,
                    PatientID = 1,
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


        }



    }
}
