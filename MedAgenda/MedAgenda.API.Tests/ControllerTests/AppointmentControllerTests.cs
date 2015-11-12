using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MedAgenda.API.Controllers;
using MedAgenda.CORE.Models;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using MedAgenda.API.Tests.Infrastructure;

namespace MedAgenda.API.Tests.ControllerTests
{
    /// <summary>
    /// Summary description for AccountControllerTests
    /// </summary>
    [TestClass]
    public class AppointmentControllerTests : BaseTest
    {    
        [TestMethod] // Get all appointments | [0]
        public void GetAppointmentsReturnAppointments()
        {
            //Arrange
            using (var apptController = new AppointmentsController())
            {
                //Act
                IEnumerable<AppointmentModel> appointments = apptController.GetAppointments();

                //Assert
                if (appointments.Count() == 0) Assert.Inconclusive("Appointments table is empty");

                Assert.IsTrue(appointments.Count() > 0);
            }
        }

        [TestMethod] // Get an Appointment | [1]
        public void GetAppointmentReturnAppointment()
        {
            //Arrange
            using (var apptController = new AppointmentsController())
            {
                //Act
                IHttpActionResult result = apptController.GetAppointment(1);

                //Assert
                if(result is OkNegotiatedContentResult<AppointmentModel>)
                {
                    OkNegotiatedContentResult<AppointmentModel> contentResult = (OkNegotiatedContentResult<AppointmentModel>)result;

                    Assert.IsTrue(contentResult.Content.AppointmentID == 1);
                }
                else
                {
                    if(result is NotFoundResult)
                    {
                        Assert.Inconclusive();
                    }
                    else
                    {
                        Assert.Fail();
                    }
                }
            }
        }

        [TestMethod] // Create an Appointment | [2]
        public void PostAppointmentCreateAppointment()
        {
            CreatedAtRouteNegotiatedContentResult<AppointmentModel> contentResult;

            //Arrange
            using (var apptController = new AppointmentsController())
            {
                var newAppt = new AppointmentModel
                {
                    DoctorID = 1,
                    CheckinDateTime = DateTime.Now,
                    CheckoutDateTime = DateTime.Now.AddHours(2),
                    ExamRoomID = 1,
                    PatientID = 1
                };

                //Act
                IHttpActionResult result = apptController.PostAppointment(newAppt);

                //Assert
                Assert.IsInstanceOfType(result, typeof(CreatedAtRouteNegotiatedContentResult<AppointmentModel>));

                  contentResult = (CreatedAtRouteNegotiatedContentResult<AppointmentModel>)result;

                Assert.IsTrue(contentResult.Content.AppointmentID != 0);        
            }

            //Delete the Appointment
            using (var SecondapptController = new AppointmentsController())
            {
                //Delete the Test Doctor
              IHttpActionResult  result = SecondapptController.DeleteAppointment(contentResult.Content.AppointmentID);

            }
        }

        [TestMethod] // Update Appointment | [3]
        public void PutAppointmentReturnAppointment()
        {
            //Test Properties
            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<AppointmentModel> contentResult;
            OkNegotiatedContentResult<AppointmentModel> appointmentResult;
            OkNegotiatedContentResult<AppointmentModel> readContentResult;

            using (var apptController = new AppointmentsController())
            {
                //Create Appointment
                var newAppt = new AppointmentModel
                {
                    DoctorID = 1,
                    CheckinDateTime = DateTime.Now,
                    CheckoutDateTime = DateTime.Now.AddHours(2),
                    ExamRoomID = 1,
                    PatientID = 1
                };

                //Insert Appointment Model Object into Database
                //So that I can take it out and test for update
                result = apptController.PostAppointment(newAppt);

                //Cast result as Content Result so that I can gather information from Content Result
                contentResult = (CreatedAtRouteNegotiatedContentResult<AppointmentModel>)result;
            }

            using (var SecondAppointmentController = new AppointmentsController())
            {
                //Result contains the Appoint I have JUST creatd
                result = SecondAppointmentController.GetAppointment(contentResult.Content.AppointmentID);

                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<AppointmentModel>));

                //Get AppointmentModel from 'result'
                appointmentResult = (OkNegotiatedContentResult<AppointmentModel>)result;
            }

            using (var ThirdAppointmentController = new AppointmentsController())
            {
                var modifiedAppointment = appointmentResult.Content;

                modifiedAppointment.ExamRoomID = 2;

                //Act
                //The reuslt of the PUT request
                result = ThirdAppointmentController.PutAppointment(appointmentResult.Content.AppointmentID, modifiedAppointment);

                //Assert
                Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            }

            //Modify Appointment
            using (var FourthAppointmentController = new AppointmentsController())
            {
                IHttpActionResult resultAlteredAppointment = FourthAppointmentController.GetAppointment(appointmentResult.Content.AppointmentID);

                OkNegotiatedContentResult<AppointmentModel> alteredResult = (OkNegotiatedContentResult<AppointmentModel>)resultAlteredAppointment;
                AppointmentModel updatedAppointment = (AppointmentModel)alteredResult.Content;

                //Assert
                Assert.IsInstanceOfType(resultAlteredAppointment, typeof(OkNegotiatedContentResult<AppointmentModel>));

                readContentResult = (OkNegotiatedContentResult<AppointmentModel>)resultAlteredAppointment;

                Assert.IsTrue(readContentResult.Content.ExamRoomID == 2);
            }

            using (var fifthAppointmentController = new AppointmentsController())
            {
                //Delete test Appointment 
                result = fifthAppointmentController.DeleteAppointment(readContentResult.Content.AppointmentID);
            }
        }

        [TestMethod] //Delete Appointment | [4]
        public void DeleteAppointment()
        {
            CreatedAtRouteNegotiatedContentResult<AppointmentModel> contentResult;


            using (var ApptController = new AppointmentsController())
            {
                //Create Doctor
                var newAppt = new AppointmentModel
                {
                    DoctorID = 1,
                    CheckinDateTime = DateTime.Now,
                    CheckoutDateTime = DateTime.Now.AddHours(2),
                    ExamRoomID = 1,
                    PatientID = 1
                };
                //Insert DoctorModelObject into Database so 
                //that I can take it out and test for update.
                var result = ApptController.PostAppointment(newAppt);

                //Cast result as Content Result so that I can gather information from ContentResult
                contentResult = (CreatedAtRouteNegotiatedContentResult<AppointmentModel>)result;
            }

            using (var secondApptController = new AppointmentsController())
            {
                //Delete the Test Doctor
                var result = secondApptController.DeleteAppointment(contentResult.Content.AppointmentID);

                //Assert
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<AppointmentModel>));
            }
            using (var thirdApptController = new AppointmentsController())
            {
                var result = thirdApptController.GetAppointment(contentResult.Content.AppointmentID);

                //Assert
                Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            }

        }

    }
}
