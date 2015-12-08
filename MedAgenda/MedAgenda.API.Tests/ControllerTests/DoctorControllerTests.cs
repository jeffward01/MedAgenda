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

                if (!doctors.Any()) Assert.Inconclusive();

                //Assert
                Assert.IsTrue(doctors.Count() > 0);

                //Assert
                Assert.IsTrue(doctors.Where(d => d.Archived).Count() == 0);
            }
        }


        [TestMethod]
        public void GetArchivedDoctorReturnArchivedDoctors()
        {
            //Arrange
            using (var DoctorController = new DoctorsController())
            {
                //Act: Call the GetDoctors Method
                IEnumerable<DoctorModel> doctors = DoctorController.GetDoctors();

                //Assert
                if (doctors.Count() == 0) Assert.Inconclusive("No archived doctors found");
                
                Assert.IsTrue(doctors.Count() > 0);

                Assert.IsTrue(doctors.Any(d => !d.Archived));
            }  
        }

        [TestMethod] //Get Doctor by ID | [1]
        public void GetDoctorReturnDoctor()
        {
            int createdSpecialtyID;
            int createdDoctorID;
            //Arrange: create test specialty and doctor
            // Create a new test specialty, and get its specialty ID
            using (var specialtyController = new SpecialtiesController())
            {
                var specialty = new SpecialtyModel
                {
                    SpecialtyName = "Very Special Doctor"
                };
                IHttpActionResult result = specialtyController.PostSpecialty(specialty);
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
                IHttpActionResult result = doctorController.PostDoctor(doctor);
                CreatedAtRouteNegotiatedContentResult<DoctorModel> doctorContentResult =
                    (CreatedAtRouteNegotiatedContentResult<DoctorModel>)result;
                createdDoctorID = doctorContentResult.Content.DoctorID;
            }

            using (var doctorController = new DoctorsController())
            {
                //Act
                IHttpActionResult result = doctorController.GetDoctor(createdDoctorID);

                //Assert
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<DoctorModel>));

                OkNegotiatedContentResult<DoctorModel> contentResult = (OkNegotiatedContentResult<DoctorModel>)result;

                Assert.IsTrue(contentResult.Content.DoctorID == createdDoctorID);
            }

            // Remove the test doctor from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Doctor dbDoctor = db.Doctors.Find(createdDoctorID);
                db.Doctors.Remove(dbDoctor);                
                db.SaveChanges();
            }

            // Delete the test specialty
            using (var specialtyController = new SpecialtiesController())
            {
                IHttpActionResult result = specialtyController.DeleteSpecialty(createdSpecialtyID);
            }
        }

        [TestMethod] // Create Doctor [2]
        public void PostDoctorCreateDoctor()
        {
            CreatedAtRouteNegotiatedContentResult<DoctorModel> contentResult;
            int createdSpecialtyID;

            //Arrange
            // Create a new test specialty, and get its specialty ID
            using (var specialtyController = new SpecialtiesController())
            {
                var specialty = new SpecialtyModel
                {
                    SpecialtyName = "Very Special Doctor"
                };
                IHttpActionResult result = specialtyController.PostSpecialty(specialty);
                CreatedAtRouteNegotiatedContentResult<SpecialtyModel> specialtyContentResult =
                    (CreatedAtRouteNegotiatedContentResult<SpecialtyModel>)result;
                createdSpecialtyID = specialtyContentResult.Content.SpecialtyID;
            }

            using (var DoctorController = new DoctorsController())
            {
                //Create Doctor
                var newDoctor = new DoctorModel
                {
                    FirstName = "Alex",
                    LastName = "Smith",
                    Email = "example@example.com",
                    SpecialtyID = createdSpecialtyID,
                    Telephone = "111-111-1111",
                    CreatedDate = DateTime.Today
                };

                //Act
                IHttpActionResult result = DoctorController.PostDoctor(newDoctor);

                //Assert
                Assert.IsInstanceOfType(result, typeof(CreatedAtRouteNegotiatedContentResult<DoctorModel>));

                 contentResult = (CreatedAtRouteNegotiatedContentResult<DoctorModel>)result;

                Assert.IsTrue(contentResult.Content.DoctorID != 0);                
            }

            // Remove the test doctor from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Doctor dbDoctor = db.Doctors.Find(contentResult.Content.DoctorID);
                db.Doctors.Remove(dbDoctor);
                db.SaveChanges();
            }

            // Delete the test specialty
            using (var specialtyController = new SpecialtiesController())
            {
                IHttpActionResult result = specialtyController.DeleteSpecialty(createdSpecialtyID);
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
            int createdSpecialtyID;

            //Arrange
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

            using (var DoctorController = new DoctorsController())
            {
                //Create Doctor
                var newDoctor = new DoctorModel
                {
                    FirstName = "Alex",
                    LastName = "Smith",
                    Email = "example@example.com",
                    SpecialtyID = createdSpecialtyID,
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

                readContentResult = (OkNegotiatedContentResult<DoctorModel>)resultAlteredDoctor;

                Assert.IsTrue(readContentResult.Content.FirstName == "John");
            }

            // Remove the test doctor from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Doctor dbDoctor = db.Doctors.Find(readContentResult.Content.DoctorID);
                db.Doctors.Remove(dbDoctor);
                db.SaveChanges();
            }

            // Delete the test specialty
            using (var specialtyController = new SpecialtiesController())
            {
                result = specialtyController.DeleteSpecialty(createdSpecialtyID);
            }

        }

        [TestMethod] // Delete Doctor [4]
        public void DeleteDoctor()
        {
            int doctorIDForTest;
            int createdSpecialtyID;

            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<DoctorModel> createdContentResult;
            OkNegotiatedContentResult<DoctorModel> OkcontentResult;
            
            //Arrange
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
            using (var DoctorController = new DoctorsController())
            {
                var newDoctor = new DoctorModel
                {
                    FirstName = "Alex",
                    LastName = "Smith",
                    Email = "example@example.com",
                    SpecialtyID = createdSpecialtyID,
                    Telephone = "111-111-1111",
                    CreatedDate = DateTime.Today
                };
                result = DoctorController.PostDoctor(newDoctor);
                createdContentResult =
                    (CreatedAtRouteNegotiatedContentResult<DoctorModel>)result;
                doctorIDForTest = createdContentResult.Content.DoctorID;
            }

            //Call the procedure to delete the doctor, which sets its archived indicator to true
            using (var docController = new DoctorsController())
            {
                result = docController.DeleteDoctor(doctorIDForTest);

                // Verify that HTTP result is OK
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<DoctorModel>));
                // Verify that the returned DoctorModel object has archived indicator set to true
                OkcontentResult =
                    (OkNegotiatedContentResult<DoctorModel>)result;
                Assert.IsTrue(OkcontentResult.Content.Archived);
            }

            // Get the doctor and verify that the doctor has archived indicator set to true
            using (var DocController = new DoctorsController())
            {
                result = DocController.GetDoctor(doctorIDForTest);
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<DoctorModel>));
                OkcontentResult =
                    (OkNegotiatedContentResult<DoctorModel>)result;
                Assert.IsTrue(OkcontentResult.Content.Archived);
            }

            // Remove the doctor from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Doctor dbDoctor = db.Doctors.Find(doctorIDForTest);
                db.Doctors.Remove(dbDoctor);
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
