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
    /// Summary description for ExamControllerTest
    /// </summary>
    [TestClass]
    public class ExamRoomControllerTest : BaseTest
    {

        [TestMethod] // Get all Exam Rooms [0]
        public void GetRoomsReturnRooms()
        {
            using (var ExamRoomController = new ExamRoomsController())
            {

                //Act: Call the Get ExamRoom Method
                IEnumerable<ExamRoomModel> examRooms = ExamRoomController.GetExamRooms();

                //Assert
                Assert.IsTrue(examRooms.Count() > 0);
            }
        }

        [TestMethod] // Get Exam Room by ID [1]
        public void GetRoomReturnRoom()
        {
            //Arrange
            using (var examRoomController = new ExamRoomsController())
            {
                //Act
                IHttpActionResult result = examRoomController.GetExamRoom(1);

                //Assert
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<ExamRoomModel>));

                OkNegotiatedContentResult<ExamRoomModel> contentResult = (OkNegotiatedContentResult<ExamRoomModel>)result;

                Assert.IsTrue(contentResult.Content.ExamRoomID == 1);

            }
        }

        [TestMethod] // Create Exam Room [2]
        public void PostRoomCreateRoom()
        {
            //Arrange
            using (var ExamRoomController = new ExamRoomsController())
            {
                //Create Exam Room
                var newExamRoom = new ExamRoomModel
                {
                    ExamRoomName = "Test Room",
                };

                //Act
                IHttpActionResult result = ExamRoomController.PostExamRoom(newExamRoom);

                //Assert
                Assert.IsInstanceOfType(result, typeof(CreatedAtRouteNegotiatedContentResult<ExamRoomModel>));

                CreatedAtRouteNegotiatedContentResult<ExamRoomModel> contentResult = (CreatedAtRouteNegotiatedContentResult<ExamRoomModel>)result;

                Assert.IsTrue(contentResult.Content.ExamRoomID != 0);

                //Delete the Test Exam Room
                result = ExamRoomController.DeleteExamRoom(contentResult.Content.ExamRoomID);

            }
        }

        [TestMethod] //Update Exam Room [3]
        public void PutRoomUpdateRoom()
        {
            //Test Exam Rooms
            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<ExamRoomModel> contentResult;
            OkNegotiatedContentResult<ExamRoomModel> examRoomResult;
            OkNegotiatedContentResult<ExamRoomModel> readContentResult;


            using (var ExamRoomController = new ExamRoomsController())
            {
                //Create Doctor
                var newExamRoom = new ExamRoomModel
                {
                    ExamRoomName = "Test Room",

                };
                //Insert DoctorModelObject into Database so 
                //that I can take it out and test for update.
                result = ExamRoomController.PostExamRoom(newExamRoom);

                //Cast result as Content Result so that I can gather information from ContentResult
                contentResult = (CreatedAtRouteNegotiatedContentResult<ExamRoomModel>)result;
            }
            using (var SecondExamRoomController = new ExamRoomsController())
            {
                //Result contains the Doctor I had JUST createad
                result = SecondExamRoomController.GetExamRoom(contentResult.Content.ExamRoomID);

                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<ExamRoomModel>));

                //Get ExamRoomModel from 'result'
                examRoomResult = (OkNegotiatedContentResult<ExamRoomModel>)result;

            }

            using (var ThirdExamRoomController = new ExamRoomsController())
            {
                var modifiedExamRoom = examRoomResult.Content;

                modifiedExamRoom.ExamRoomName = "Updated Test Room";

                //Act
                //The result of the Put Request
                result = ThirdExamRoomController.PutExamRoom(examRoomResult.Content.ExamRoomID, modifiedExamRoom);

                //Assert
                Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            }
            using (var FourthExamRoomController = new ExamRoomsController())
            {
                //Act
                IHttpActionResult resultAlteredExamRoom = FourthExamRoomController.GetExamRoom(examRoomResult.Content.ExamRoomID);

                OkNegotiatedContentResult<ExamRoomModel> alteredResult = (OkNegotiatedContentResult<ExamRoomModel>)resultAlteredExamRoom;
                ExamRoomModel updatedExamRoom = (ExamRoomModel)alteredResult.Content;

                //Assert
                Assert.IsInstanceOfType(resultAlteredExamRoom, typeof(OkNegotiatedContentResult<ExamRoomModel>));

                readContentResult =
                    (OkNegotiatedContentResult<ExamRoomModel>)resultAlteredExamRoom;

                Assert.IsTrue(readContentResult.Content.ExamRoomName == "Updated Test Room");
            }
            using (var FifthExamRoomController = new ExamRoomsController())
            {
                //Delete the Test Exam Room
                result = FifthExamRoomController.DeleteExamRoom(readContentResult.Content.ExamRoomID);
            }
        }

        [TestMethod] // Delete Exam Room [4]
        public void DeleteExamRoom()
        {
            CreatedAtRouteNegotiatedContentResult<ExamRoomModel> contentResult;


            using (var ExamRoomController = new ExamRoomsController())
            {
                //Creat Exam Room
                var newExamRoom = new ExamRoomModel
                {
                    ExamRoomName = "Test Room"
                };
                //Insert ExamRoomModelObject into Database so 
                //that I can take it out and test for update.
                var result = ExamRoomController.PostExamRoom(newExamRoom);

                //Cast result as Content Result so that I can gather information from ContentResult
                contentResult = (CreatedAtRouteNegotiatedContentResult<ExamRoomModel>)result;
            }
            using (var SecondExamRoomController = new ExamRoomsController())
            {
                //Delete the Test Exam Room
                var result = SecondExamRoomController.DeleteExamRoom(contentResult.Content.ExamRoomID);

                //Assert
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<ExamRoomModel>));
            }
            using (var ThirdExamRoomController = new ExamRoomsController())
            {
                var result = ThirdExamRoomController.GetExamRoom(contentResult.Content.ExamRoomID);

                //Assert
                Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            }

        }

    }

}  