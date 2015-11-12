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
    /// Summary description for SpecailtyControllerTest
    /// </summary>
    [TestClass]
    public class SpecailtyControllerTest : BaseTest
    {
        [TestMethod] // Get all Specialties
        public void GetSpecialtiesReturnSpecialties()
        {
            using (var SpecialtyController = new SpecialtiesController())
            {

                //Act: Call the Get Specialty Method
                IEnumerable<SpecialtyModel> specialties = SpecialtyController.GetSpecialties();

                //Assert
                Assert.IsTrue(specialties.Count() > 0);
            }
        
        }

        [TestMethod] // Get Specialty by ID [1]
        public void GetSpecialtyReturnSpecialt()
        {
            //Arrange
            using (var specialtyController = new SpecialtiesController())
            {
                //Act
                IHttpActionResult result = specialtyController.GetSpecialty(1);

                //Assert
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<SpecialtyModel>));
                     OkNegotiatedContentResult<SpecialtyModel> contentResult = (OkNegotiatedContentResult<SpecialtyModel>)result;

                Assert.IsTrue(contentResult.Content.SpecialtyID == 1);
            }
        }

        [TestMethod] // Create Specialty [2]
        public void PostSpecialtyCreateSpecialty()
        {
            //Arrange
            using (var SpecialtyController = new SpecialtiesController())
            {
                //Create Specialty
                var newSpecialty = new SpecialtyModel
                {
                    SpecialtyName = "Testologist",
                };

                //Act
                IHttpActionResult result = SpecialtyController.PostSpecialty(newSpecialty);

                //Assert
                Assert.IsInstanceOfType(result, typeof(CreatedAtRouteNegotiatedContentResult<SpecialtyModel>));

                CreatedAtRouteNegotiatedContentResult<SpecialtyModel> contentResult = (CreatedAtRouteNegotiatedContentResult<SpecialtyModel>)result;

                Assert.IsTrue(contentResult.Content.SpecialtyID != 0);

                //Delete the test Specialty
                result = SpecialtyController.DeleteSpecialty(contentResult.Content.SpecialtyID);
            }
        }

        [TestMethod] //Update Specialty [3]
        public void PutSpecialtyUpdateSpecialty()
        {
            //Test Specialty
            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<SpecialtyModel> contentResult;
            OkNegotiatedContentResult<SpecialtyModel> specialtyResult;
            OkNegotiatedContentResult<SpecialtyModel> readContentResult;


            using (var SpecialtyController = new SpecialtiesController())
            {
                //Create Specialty
                var newSpecialty = new SpecialtyModel
                {
                    SpecialtyName = "Testologist",

                };
                //Insert DoctorModelObject into Database so 
                //that I can take it out and test for update.
                result = SpecialtyController.PostSpecialty(newSpecialty);

                //Cast result as Content Result so that I can gather information from ContentResult
                contentResult = (CreatedAtRouteNegotiatedContentResult<SpecialtyModel>)result;
            }
            using (var SecondSpecialtyController = new SpecialtiesController())
            {
                //Result contains the Doctor I had JUST createad
                result = SecondSpecialtyController.GetSpecialty(contentResult.Content.SpecialtyID);

                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<SpecialtyModel>));

                //Get ExamRoomModel from 'result'
                specialtyResult = (OkNegotiatedContentResult<SpecialtyModel>)result;

            }

            using (var ThirdSpecialtyController = new SpecialtiesController())
            {
                var modifiedSpecialty = specialtyResult.Content;

                modifiedSpecialty.SpecialtyName = "Updated Testologist";

                //Act
                //The result of the Put Request
                result = ThirdSpecialtyController.PutSpecialty(specialtyResult.Content.SpecialtyID, modifiedSpecialty);

                //Assert
                Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            }
            using (var FourthSpecialtyController = new SpecialtiesController())
            {
                //Act
                IHttpActionResult resultAlteredSpecialty = FourthSpecialtyController.GetSpecialty(specialtyResult.Content.SpecialtyID);

                OkNegotiatedContentResult<SpecialtyModel> alteredResult = (OkNegotiatedContentResult<SpecialtyModel>)resultAlteredSpecialty;
                SpecialtyModel updatedSpecialty = (SpecialtyModel)alteredResult.Content;

                //Assert
                Assert.IsInstanceOfType(resultAlteredSpecialty, typeof(OkNegotiatedContentResult<SpecialtyModel>));

                readContentResult =
                    (OkNegotiatedContentResult<SpecialtyModel>)resultAlteredSpecialty;

                Assert.IsTrue(readContentResult.Content.SpecialtyName == "Updated Testologist");
            }
            using (var FifthSpecialtyController = new SpecialtiesController())
            {
                //Delete the Test Specialty
                result = FifthSpecialtyController.DeleteSpecialty(readContentResult.Content.SpecialtyID);
            }
        }

        [TestMethod] // Delete Specialty [4]
        public void DeleteSpecialty()
        {
            CreatedAtRouteNegotiatedContentResult<SpecialtyModel> contentResult;


            using (var SpecialtyController = new SpecialtiesController())
            {
                //Creat Exam Room
                var newSpecialty = new SpecialtyModel
                {
                    SpecialtyName = "Testologist"
                };
                //Insert ExamRoomModelObject into Database so 
                //that I can take it out and test for update.
                var result = SpecialtyController.PostSpecialty(newSpecialty);

                //Cast result as Content Result so that I can gather information from ContentResult
                contentResult = (CreatedAtRouteNegotiatedContentResult<SpecialtyModel>)result;
            }
            using (var SecondSpecialtyController = new SpecialtiesController())
            {
                //Delete the Test Exam Room
                var result = SecondSpecialtyController.DeleteSpecialty(contentResult.Content.SpecialtyID);

                //Assert
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<SpecialtyModel>));
            }
            using (var ThirdSpecialtyController = new SpecialtiesController())
            {
                var result = ThirdSpecialtyController.GetSpecialty(contentResult.Content.SpecialtyID);

                //Assert
                Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            }

        }
    }


}
