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
                if (doctors.Count() == 0) Assert.Inconclusive();
                
                Assert.IsTrue(doctors.Count() > 0);

                Assert.IsTrue(doctors.Any(d => !d.Archived));
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
            CreatedAtRouteNegotiatedContentResult<DoctorModel> contentResult;

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

                 contentResult = (CreatedAtRouteNegotiatedContentResult<DoctorModel>)result;

                Assert.IsTrue(contentResult.Content.DoctorID != 0);                
            }

            using (var SecondDoctorsController = new DoctorsController())
            {
                var deleteAppt = contentResult.Content;

                //Delete the Test Doctor
               IHttpActionResult result = SecondDoctorsController.DeleteDoctor(contentResult.Content.DoctorID);
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

                readContentResult = (OkNegotiatedContentResult<DoctorModel>)resultAlteredDoctor;

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
            int doctorIDForTest;

            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<DoctorModel> createdContentResult;
            OkNegotiatedContentResult<DoctorModel> OkcontentResult;

            // Create a new test patient, and get its patient ID
            using (var DoctorController = new DoctorsController())
            {
                var newDoctor = new DoctorModel
                {
                    FirstName = "Alex",
                    LastName = "Smith",
                    Email = "example@example.com",
                    SpecialtyID = 2,
                    Telephone = "111-111-1111",
                    CreatedDate = DateTime.Today
                };
                result = DoctorController.PostDoctor(newDoctor);
                createdContentResult =
                    (CreatedAtRouteNegotiatedContentResult<DoctorModel>)result;
                doctorIDForTest = createdContentResult.Content.DoctorID;
            }

            //Call the procedure to delete the patient, which sets its archived indicator to true
            using (var docConrtoller = new DoctorsController())
            {
                result = docConrtoller.DeleteDoctor(doctorIDForTest);

                // Verify that HTTP result is OK
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<DoctorModel>));
                // Verify that the returned PatientModel object has archived indicator set to true
                OkcontentResult =
                    (OkNegotiatedContentResult<DoctorModel>)result;
                Assert.IsTrue(OkcontentResult.Content.Archived);
            }

            // Get the patient and verify that the patient has archived indicator set to true
            using (var DocController = new DoctorsController())
            {
                result = DocController.GetDoctor(doctorIDForTest);
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<DoctorModel>));
                OkcontentResult =
                    (OkNegotiatedContentResult<DoctorModel>)result;
                Assert.IsTrue(OkcontentResult.Content.Archived);
            }

            // Remove the patient from the database with actual deletion, not archiving
            using (MedAgendaDbContext db = new MedAgendaDbContext())
            {
                Doctor dbDoctor = db.Doctors.Find(doctorIDForTest);
                db.Doctors.Remove(dbDoctor);
                db.SaveChanges();
            }
        }
    }
}
