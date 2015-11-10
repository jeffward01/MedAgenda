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
    /// Summary description for DoctorControllerTests
    /// </summary>
    [TestClass]
    public class DoctorControllerTests : BaseTest
    {


        [TestMethod] // Get all Doctors | [0]
        public void GetDoctorsReturnDoctors()
        {
            //Arrange
            using (var DoctorController = new DoctorsController())
            {

                //Act: Call the GetDoctors Method
                IEnumerable<DoctorModel> doctors = DoctorController.GetDoctors();

                //Assert
                Assert.IsTrue(doctors.Count() > 0);
            }
        }

        [TestMethod] //Get Doctor by ID | [1]
        public void GetDoctorReturnDoctor()
        {
            //Arrange
            using (var doctorController = new DoctorsController())
            {
                //Act
                IHttpActionResult result = doctorController.GetDoctor(1);

                //Assert
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<DoctorModel>));

                OkNegotiatedContentResult<DoctorModel> contentResult = (OkNegotiatedContentResult<DoctorModel>)result;

                Assert.IsTrue(contentResult.Content.DoctorID == 1);
            }

        }

        [TestMethod] // Create Doctor [2]
        public void PostDoctorCreateDoctor()
        {
            //Arrange
            using (var DoctorController = new DoctorsController())
            {
                //Create Doctor
                var newDoctor = new DoctorModel
                {
                    FirstName = "Alex",
                    LastName = "Smith",
                    Email = "example@example.com",
                    SpecialtyID = 2,
                    Telephone = "111-111-1111",
                    CreatedDate = DateTime.Today
                };

                //Act
                IHttpActionResult result = DoctorController.PostDoctor(newDoctor);

                //Assert
                Assert.IsInstanceOfType(result, typeof(CreatedAtRouteNegotiatedContentResult<DoctorModel>));

                CreatedAtRouteNegotiatedContentResult<DoctorModel> contentResult = (CreatedAtRouteNegotiatedContentResult<DoctorModel>)result;

                Assert.IsTrue(contentResult.Content.DoctorID != 0);

                //Delete the Test Doctor
                result = DoctorController.DeleteDoctor(contentResult.Content.DoctorID);
            }
        }


        [TestMethod] //Update Doctor [3]
        public void PutDoctorUpdateDoctor()
        {
            //Test Properties
            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<DoctorModel> contentResult;
            OkNegotiatedContentResult<DoctorModel> doctorResult;
            OkNegotiatedContentResult<DoctorModel> readContentResult;


            using (var DoctorController = new DoctorsController())
            {
                //Create Doctor
                var newDoctor = new DoctorModel
                {
                    FirstName = "Alex",
                    LastName = "Smith",
                    Email = "example@example.com",
                    SpecialtyID = 2,
                    Telephone = "111-111-1111",
                    CreatedDate = DateTime.Today
                };
                //Insert DoctorModelObject into Database so 
                //that I can take it out and test for update.
                result = DoctorController.PostDoctor(newDoctor);

                //Cast result as Content Result so that I can gather information from ContentResult
                contentResult = (CreatedAtRouteNegotiatedContentResult<DoctorModel>)result;
            }

            using (var SecondDoctorController = new DoctorsController())
            {
                //Result contains the Doctor I had JUST createad
                result = SecondDoctorController.GetDoctor(contentResult.Content.DoctorID);

                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<DoctorModel>));

                //Get DoctorModel from 'result'
                doctorResult = (OkNegotiatedContentResult<DoctorModel>)result;

            }

            using (var ThirdDoctorController = new DoctorsController())
            {
                var modifiedDoctor = doctorResult.Content;

                modifiedDoctor.FirstName = "John";

                //Act
                //The result of the Put Request
                result = ThirdDoctorController.PutDoctor(doctorResult.Content.DoctorID, modifiedDoctor);

                //Assert
                Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            }

            using (var FourthDoctorController = new DoctorsController())
            {
                //Act
                IHttpActionResult resultAlteredDoctor = FourthDoctorController.GetDoctor(doctorResult.Content.DoctorID);

                OkNegotiatedContentResult<DoctorModel> alteredResult = (OkNegotiatedContentResult<DoctorModel>)resultAlteredDoctor;
                DoctorModel updatedDoctor = (DoctorModel)alteredResult.Content;

                //Assert
                Assert.IsInstanceOfType(resultAlteredDoctor, typeof(OkNegotiatedContentResult<DoctorModel>));

                readContentResult =
                    (OkNegotiatedContentResult<DoctorModel>)resultAlteredDoctor;

                Assert.IsTrue(readContentResult.Content.FirstName == "John");
            }

            using (var FifthDoctorController = new DoctorsController())
            {
                //Delete the Test Doctor
                result = FifthDoctorController.DeleteDoctor(readContentResult.Content.DoctorID);
            }
        }

        [TestMethod] // Delete Doctor [4]
        public void DeleteDoctor()
        {
            CreatedAtRouteNegotiatedContentResult<DoctorModel> contentResult;
        

            using (var DoctorController = new DoctorsController())
            {
                //Create Doctor
                var newDoctor = new DoctorModel
                {
                    FirstName = "Alex",
                    LastName = "Smith",
                    Email = "example@example.com",
                    SpecialtyID = 2,
                    Telephone = "111-111-1111",
                    CreatedDate = DateTime.Today
                };
                //Insert DoctorModelObject into Database so 
                //that I can take it out and test for update.
               var result = DoctorController.PostDoctor(newDoctor);

                //Cast result as Content Result so that I can gather information from ContentResult
               contentResult = (CreatedAtRouteNegotiatedContentResult<DoctorModel>)result;
            }

            using (var secondDocController = new DoctorsController())
            {
                //Delete the Test Doctor
                var result = secondDocController.DeleteDoctor(contentResult.Content.DoctorID);

                //Assert
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<DoctorModel>));
            }
            using (var thirdDocController = new DoctorsController())
            {
                var result = thirdDocController.GetDoctor(contentResult.Content.DoctorID);

                //Assert
                Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            }


        }



    }
}
