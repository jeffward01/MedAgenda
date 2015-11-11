using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MedAgenda.API.Tests.Infrastructure;
using MedAgenda.API.Controllers;
using System.Collections.Generic;
using System.Linq;
using MedAgenda.CORE.Models;
using System.Web.Http;
using System.Web.Http.Results;
using System.Collections;

namespace MedAgenda.API.Tests.ControllerTests
{
    [TestClass]
    public class DoctorsCheckControllerTests : BaseTest
    {
        [TestMethod]
        public void GetDoctorsCheckReturnsDoctorsCheck()
        {

            //Arrange: Instantiate DoctorCheckController so its methods can be called
            using (var doctorsCheckController = new DoctorsCheckController())
            {
                //Act: Call the GetDoctorCheck method
                IEnumerable<DoctorCheckModel> doctorCheck = doctorsCheckController.GetDoctorChecks();

                //Assert: Verify that an array was returned with at least one element
                Assert.IsTrue(doctorCheck.Count() > 0);
            }
        }

        [TestMethod]
        public void GetDoctorCheckReturnDoctorCheck()
        {
            int DoctorCheckIDForTest = 4;

            //Arrange: Instantiate DoctorCheckController so its methods can be called
            var doctorCheckController = new DoctorsCheckController();

            //Act: Call the GetDoctorCheck method
            IHttpActionResult result = doctorCheckController.GetDoctorCheck(DoctorCheckIDForTest);

            //Assert: 
            // Verify that HTTP status code is OK
            // Verify that returned patient ID is correct
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<DoctorCheckModel>));

            OkNegotiatedContentResult<DoctorCheckModel> contentResult =
                (OkNegotiatedContentResult<DoctorCheckModel>)result;
            Assert.IsTrue(contentResult.Content.DoctorCheckID == DoctorCheckIDForTest);
        }

        [TestMethod]
        public void PostDoctorCheckCreatesDoctorCheck()
        {
            //Arrange: Instantiate DoctorCheckController so its methods can be called
            var doctorCheckController = new DoctorsCheckController();

            //Act: 
            // Create a DcotorCheckModel object populated with test data,
            //  and call PostDoctorCheck
            var newDoctorCheck = new DoctorCheckModel
            {
                DoctorID = 1,
                ExamRoomID = 1,
                CheckinDateTime = new DateTime(1968, 12, 27),
                CheckoutDateTime = new DateTime(1968, 12, 27),               
           
            };
            IHttpActionResult result = doctorCheckController.PostDoctorCheck(newDoctorCheck);

            //Assert:
            // Verify that the HTTP result is CreatedAtRouteNegotiatedContentResult
            // Verify that the HTTP result body contains a nonzero doctorCheck ID
            Assert.IsInstanceOfType
                (result, typeof(CreatedAtRouteNegotiatedContentResult<DoctorCheckModel>));
            CreatedAtRouteNegotiatedContentResult<DoctorCheckModel> contentResult =
                (CreatedAtRouteNegotiatedContentResult<DoctorCheckModel>)result;
            Assert.IsTrue(contentResult.Content.DoctorCheckID != 0);

            // Delete the test doctorCheck 
            result = doctorCheckController.DeleteDoctorCheck(contentResult.Content.DoctorCheckID);
        }

        [TestMethod]
        public void PutDoctorCheckUpdateDoctorCheck()
        {
            int doctorCheckIDForTest;
            int examRoomIDForTest = 1;
           // string patientLastNameForTest = "McTesterson";de

            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<DoctorCheckModel> createdContentResult;
            OkNegotiatedContentResult<DoctorCheckModel> OkContentResult;
            DoctorCheckModel updatedDoctorCheck;

            //Arrange: Add new doctorCheck, and save its doctorCheckID
            using (var doctorsCheckController = new DoctorsCheckController())
            {
                var newDoctorCheck = new DoctorCheckModel
                {
                    DoctorID = 1,
                    ExamRoomID = 2,
                    CheckinDateTime = new DateTime(1968, 12, 27),
                    CheckoutDateTime = new DateTime(1968, 12, 27),
                };
                result = doctorsCheckController.PostDoctorCheck(newDoctorCheck);
                createdContentResult =
                    (CreatedAtRouteNegotiatedContentResult<DoctorCheckModel>)result;
                doctorCheckIDForTest = createdContentResult.Content.DoctorCheckID;
            }

            // Get the doctorCheck from the DB
            using (var doctorsCheckController = new DoctorsCheckController())
            {
                result = doctorsCheckController.GetDoctorCheck(doctorCheckIDForTest);
                OkContentResult =
                    (OkNegotiatedContentResult<DoctorCheckModel>)result;
                updatedDoctorCheck = (DoctorCheckModel)createdContentResult.Content;
            }

                              
            using (var doctorsCheckController = new DoctorsCheckController())
            {
                updatedDoctorCheck.ExamRoomID = examRoomIDForTest;
               

                result = doctorsCheckController.PutDoctorCheck
                                         (updatedDoctorCheck.DoctorCheckID, updatedDoctorCheck);
            }

            // Verify that HTTP status code is OK
            // Get the patient and verify that it was updated

            var statusCode = (StatusCodeResult)result;
            Assert.IsTrue(statusCode.StatusCode == System.Net.HttpStatusCode.NoContent);

            using (var doctorsCheckController = new DoctorsCheckController())
            {
                result = doctorsCheckController.GetDoctorCheck(doctorCheckIDForTest);

                Assert.IsInstanceOfType(result,
                    typeof(OkNegotiatedContentResult<DoctorCheckModel>));

                OkContentResult =
                    (OkNegotiatedContentResult<DoctorCheckModel>)result;
                updatedDoctorCheck = (DoctorCheckModel)OkContentResult.Content;
            }

            Assert.IsTrue(updatedDoctorCheck.ExamRoomID == examRoomIDForTest);
           

            // Delete the test doctorCheck
            using (var doctorsCheckController = new DoctorsCheckController())
            {
                result = doctorsCheckController.DeleteDoctorCheck(doctorCheckIDForTest);
            }

        }

        [TestMethod]
        public void DeleteDoctorsCheck()
        {
            int doctorsCheckIDForTest ;


            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<DoctorCheckModel> createdContentResult;

            // Create a new test doctorCheck, and get its doctorCheckID
            using (var doctorsCheckController = new DoctorsCheckController())
            {
                var doctorsCheck = new DoctorCheckModel
                {
                    DoctorID = 1,
                    ExamRoomID = 2,
                    CheckinDateTime = new DateTime(1968, 12, 27),
                    CheckoutDateTime = new DateTime(1968, 12, 27),
                };
                result = doctorsCheckController.PostDoctorCheck(doctorsCheck);
                createdContentResult =
                    (CreatedAtRouteNegotiatedContentResult<DoctorCheckModel>)result;
                doctorsCheckIDForTest = createdContentResult.Content.DoctorCheckID;
            }

            //Delete the doctorCheck
            using (var doctorsCheckController = new DoctorsCheckController())
            {
                result = doctorsCheckController.DeleteDoctorCheck(doctorsCheckIDForTest);

                // Verify that HTTP result is OK
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<DoctorCheckModel>));
            }

            // Verify that reading deleted patient returns result not found
            using (var doctorsCheckController = new DoctorsCheckController())
            {
                result = doctorsCheckController.GetDoctorCheck(doctorsCheckIDForTest);
                Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            }

       
        }
    }
}