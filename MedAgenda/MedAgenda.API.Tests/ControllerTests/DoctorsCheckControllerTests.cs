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
using MedAgenda.CORE.Infrastructure;
using MedAgenda.CORE.Domain;

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
                if (doctorCheck.Count() == 0) Assert.Inconclusive("Doctor check table is empty");

                Assert.IsTrue(doctorCheck.Count() > 0);
            }
        }

        [TestMethod]
        public void GetDoctorCheckReturnDoctorCheck()
        {
            int createdDoctorID;
            int createdExamRoomID;
            int createdSpecialtyID;
            int doctorCheckIDForTest;
            IHttpActionResult result;

            //Arrange: create test specialty, doctor, exam room, and doctor check-in
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

            using (var doctorCheckController = new DoctorsCheckController())
            {
                var doctorCheck = new DoctorCheckModel
                {
                    DoctorID = createdDoctorID,
                    ExamRoomID = createdExamRoomID,
                    CheckinDateTime = DateTime.Now,
                    CheckoutDateTime = DateTime.Now.AddHours(2)
                };
                result = doctorCheckController.PostDoctorCheck(doctorCheck);
                CreatedAtRouteNegotiatedContentResult<DoctorCheckModel> contentResult =
                (CreatedAtRouteNegotiatedContentResult<DoctorCheckModel>)result;
                doctorCheckIDForTest = contentResult.Content.DoctorCheckID;
            }

            //Act: Call the GetDoctorCheck method
            using (var doctorCheckController = new DoctorsCheckController())
            {
                result = doctorCheckController.GetDoctorCheck(doctorCheckIDForTest);

                //Assert: 
                // Verify that HTTP status code is OK
                // Verify that returned doctor check ID is correct
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<DoctorCheckModel>));

                OkNegotiatedContentResult<DoctorCheckModel> contentResult =
                    (OkNegotiatedContentResult<DoctorCheckModel>)result;
                Assert.IsTrue(contentResult.Content.DoctorCheckID == doctorCheckIDForTest);
            }

            // Delete the test doctor check-in
            using (var doctorCheckController = new DoctorsCheckController())
            {
                result = doctorCheckController.DeleteDoctorCheck(doctorCheckIDForTest);
            }

            // Remove the test doctor from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Doctor dbDoctor = db.Doctors.Find(createdDoctorID);
                db.Doctors.Remove(dbDoctor);                
                db.SaveChanges();
            }

            // Delete the test exam room
            using (var SecondExamRoomController = new ExamRoomsController())
            {
                result = SecondExamRoomController.DeleteExamRoom(createdExamRoomID);
            }

            // Delete the test specialty
            using (var specialtyController = new SpecialtiesController())
            {
                result = specialtyController.DeleteSpecialty(createdSpecialtyID);
            }

        }

        [TestMethod]
        public void PostDoctorCheckCreatesDoctorCheck()
        {
            int createdDoctorID;
            int createdExamRoomID;
            int createdSpecialtyID;
            int doctorCheckIDForTest;
            IHttpActionResult result;

            //Arrange: create test specialty, doctor, and exam room
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

            //Arrange: Instantiate DoctorCheckController so its methods can be called
            using (var doctorCheckController = new DoctorsCheckController())
            {
                //Act: 
                // Create a DcotorCheckModel object populated with test data,
                //  and call PostDoctorCheck
                var newDoctorCheck = new DoctorCheckModel
                {
                    DoctorID = createdDoctorID,
                    ExamRoomID = createdExamRoomID,
                    CheckinDateTime = DateTime.Now,
                    CheckoutDateTime = DateTime.Now.AddHours(2)
                };
                result = doctorCheckController.PostDoctorCheck(newDoctorCheck);

                //Assert:
                // Verify that the HTTP result is CreatedAtRouteNegotiatedContentResult
                // Verify that the HTTP result body contains a nonzero doctorCheck ID
                Assert.IsInstanceOfType
                    (result, typeof(CreatedAtRouteNegotiatedContentResult<DoctorCheckModel>));
                CreatedAtRouteNegotiatedContentResult<DoctorCheckModel> contentResult =
                    (CreatedAtRouteNegotiatedContentResult<DoctorCheckModel>)result;
                Assert.IsTrue(contentResult.Content.DoctorCheckID != 0);
                doctorCheckIDForTest = contentResult.Content.DoctorCheckID;
            }

            // Delete the test doctorCheck 
            using (var doctorCheckController = new DoctorsCheckController())
            {
                result = doctorCheckController.DeleteDoctorCheck(doctorCheckIDForTest);
            }

            // Remove the test doctor from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Doctor dbDoctor = db.Doctors.Find(createdDoctorID);
                db.Doctors.Remove(dbDoctor);
                db.SaveChanges();
            }

            // Delete the test exam room
            using (var SecondExamRoomController = new ExamRoomsController())
            {
                result = SecondExamRoomController.DeleteExamRoom(createdExamRoomID);
            }

            // Delete the test specialty
            using (var specialtyController = new SpecialtiesController())
            {
                result = specialtyController.DeleteSpecialty(createdSpecialtyID);
            }

        }

        [TestMethod]
        public void PutDoctorCheckUpdateDoctorCheck()
        {
            int createdDoctorID;
            int createdExamRoomID;
            int changedExamRoomID;
            int createdSpecialtyID;
            int doctorCheckIDForTest;                      

            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<DoctorCheckModel> createdContentResult;
            OkNegotiatedContentResult<DoctorCheckModel> OkContentResult;
            DoctorCheckModel updatedDoctorCheck;

            //Arrange: create test specialty, doctor, and exam rooms
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

            // Create new test exam rooms, and save exam room IDs
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
            using (var examRoomController = new ExamRoomsController())
            {
                var examRoom = new ExamRoomModel
                {
                    ExamRoomName = "AnotherexamRoom"
                };
                result = examRoomController.PostExamRoom(examRoom);
                CreatedAtRouteNegotiatedContentResult<ExamRoomModel> examRoomContentResult =
                    (CreatedAtRouteNegotiatedContentResult<ExamRoomModel>)result;
                changedExamRoomID = examRoomContentResult.Content.ExamRoomID;
            }

            //Arrange: Add new doctorCheck, and save its doctorCheckID
            using (var doctorsCheckController = new DoctorsCheckController())
            {
                var newDoctorCheck = new DoctorCheckModel
                {
                    DoctorID = createdDoctorID,
                    ExamRoomID = createdExamRoomID,
                    CheckinDateTime = DateTime.Now,
                    CheckoutDateTime = DateTime.Now.AddHours(2),
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
                updatedDoctorCheck.ExamRoomID = changedExamRoomID;
               

                result = doctorsCheckController.PutDoctorCheck
                                         (updatedDoctorCheck.DoctorCheckID, updatedDoctorCheck);
            }

            // Verify that HTTP status code is OK
            // Get the doctor check and verify that it was updated

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

            Assert.IsTrue(updatedDoctorCheck.ExamRoomID == changedExamRoomID);
           
            // Delete the test doctorCheck
            using (var doctorsCheckController = new DoctorsCheckController())
            {
                result = doctorsCheckController.DeleteDoctorCheck(doctorCheckIDForTest);
            }

            // Remove the test doctor from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Doctor dbDoctor = db.Doctors.Find(createdDoctorID);
                db.Doctors.Remove(dbDoctor);
                db.SaveChanges();
            }

            // Delete the test exam rooms
            using (var SecondExamRoomController = new ExamRoomsController())
            {
                result = SecondExamRoomController.DeleteExamRoom(createdExamRoomID);
            }
            using (var SecondExamRoomController = new ExamRoomsController())
            {
                result = SecondExamRoomController.DeleteExamRoom(changedExamRoomID);
            }

            // Delete the test specialty
            using (var specialtyController = new SpecialtiesController())
            {
                result = specialtyController.DeleteSpecialty(createdSpecialtyID);
            }

        }

        [TestMethod]
        public void DeleteDoctorsCheck()
        {
            int createdDoctorID;
            int createdExamRoomID;
            int createdSpecialtyID;
            int doctorsCheckIDForTest;

            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<DoctorCheckModel> createdContentResult;

            //Arrange: create test specialty, doctor, exam room, and doctor check-in
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
          
            // Create a new test doctorCheck, and get its doctorCheckID
            using (var doctorsCheckController = new DoctorsCheckController())
            {
                var doctorsCheck = new DoctorCheckModel
                {
                    DoctorID = createdDoctorID,
                    ExamRoomID = createdExamRoomID,
                    CheckinDateTime = DateTime.Now,
                    CheckoutDateTime = DateTime.Now.AddHours(2)
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

            // Verify that reading deleted doctor check returns result not found
            using (var doctorsCheckController = new DoctorsCheckController())
            {
                result = doctorsCheckController.GetDoctorCheck(doctorsCheckIDForTest);
                Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            }

            // Remove the test doctor from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Doctor dbDoctor = db.Doctors.Find(createdDoctorID);
                db.Doctors.Remove(dbDoctor);
                db.SaveChanges();
            }

            // Delete the test exam room
            using (var SecondExamRoomController = new ExamRoomsController())
            {
                result = SecondExamRoomController.DeleteExamRoom(createdExamRoomID);
            }

            // Delete the test specialty
            using (var specialtyController = new SpecialtiesController())
            {
                result = specialtyController.DeleteSpecialty(createdSpecialtyID);
            }
        }
    }
}