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
    [TestClass]
    public class PatientCheckControllerTests : BaseTest
    {
        [TestMethod] //Get all PatientChecks [0]
        public void GetPatientChecksReturnPatienChecks()
        {
            using (var patientCheckController = new PatientChecksController())
            {
                IEnumerable<PatientCheckModel> patientChecks = patientCheckController.GetPatientChecks();

                if (patientChecks.Count() == 0) Assert.Inconclusive("Patient checks table is empty");

                Assert.IsTrue(patientChecks.Count() > 0);
            }
        }

        [TestMethod] //Get a PatientCheck [1]
        public void GetPatientCheckReturnPatientChecks()
        {
            using (var patientCheckController = new PatientChecksController())
            {
                IHttpActionResult result = patientCheckController.GetPatientCheck(1);

                if (result is OkNegotiatedContentResult<PatientCheckModel>)
                {
                    OkNegotiatedContentResult<PatientCheckModel> contentResult = (OkNegotiatedContentResult<PatientCheckModel>)result;

                    Assert.IsTrue(contentResult.Content.PatientCheckID == 1);
                }
                else
                {
                    if (result is NotFoundResult)
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

        [TestMethod] //Creat a PatientCheck [2]
        public void PostPatientCheck()
        {
            CreatedAtRouteNegotiatedContentResult<PatientCheckModel> contentResult;

            using (var patientCheckController = new PatientChecksController())
            {
                var newPatientCheck = new PatientCheckModel
                {
                    PatientCheckID = 1,
                    PatientID = 1,
                    SpecialtyID = 1,
                    CheckinDateTime = DateTime.Now,
                    CheckoutDateTime = DateTime.Now.AddHours(2)
                };

                IHttpActionResult result = patientCheckController.PostPatientCheck(newPatientCheck);

                Assert.IsInstanceOfType(result, typeof(CreatedAtRouteNegotiatedContentResult<PatientCheckModel>));

                contentResult = (CreatedAtRouteNegotiatedContentResult<PatientCheckModel>)result;

                Assert.IsTrue(contentResult.Content.PatientCheckID != 0);
            }

            //Delete the PatientCheck
            using (var SecondPatientCheckController = new PatientChecksController())
            {
                IHttpActionResult result = SecondPatientCheckController.DeletePatientCheck(contentResult.Content.PatientCheckID);
            }
        }

        [TestMethod] //Update PatientCheck [3]
        public void PutPatientCheckReturnPatientCheck()
        {
            IHttpActionResult result;
            CreatedAtRouteNegotiatedContentResult<PatientCheckModel> contentResult;
            OkNegotiatedContentResult<PatientCheckModel> patientCheckResult;
            OkNegotiatedContentResult<PatientCheckModel> readContentResult;
            DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            using (var patientCheckController = new PatientChecksController())
            {
                var newPatientCheck = new PatientCheckModel
                {
                    PatientCheckID = 1,
                    PatientID = 1,
                    SpecialtyID = 1,
                    CheckinDateTime = now,
                    CheckoutDateTime = now.AddHours(2)
                };

                result = patientCheckController.PostPatientCheck(newPatientCheck);

                contentResult = (CreatedAtRouteNegotiatedContentResult<PatientCheckModel>)result;

            }
            using (var SecondPatientCheckController = new PatientChecksController())
            {
                result = SecondPatientCheckController.GetPatientCheck(contentResult.Content.PatientCheckID);

                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<PatientCheckModel>));

                patientCheckResult = (OkNegotiatedContentResult<PatientCheckModel>)result;
            }
            using (var ThirdPatientCheckController = new PatientChecksController())
            {
                var modifiedPatientCheck = patientCheckResult.Content;

                modifiedPatientCheck.CheckoutDateTime = now;

                result = ThirdPatientCheckController.PutPatientCheck(patientCheckResult.Content.PatientCheckID, modifiedPatientCheck);

                Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            }

            using (var FourthPatientCheckController = new PatientChecksController())
            {
                IHttpActionResult resultAlteredPatientCheck = FourthPatientCheckController.GetPatientCheck(patientCheckResult.Content.PatientCheckID);

                OkNegotiatedContentResult<PatientCheckModel> alteredResult = (OkNegotiatedContentResult<PatientCheckModel>)resultAlteredPatientCheck;

                PatientCheckModel updatedPatientCheck = (PatientCheckModel)alteredResult.Content;

                Assert.IsInstanceOfType(resultAlteredPatientCheck, typeof(OkNegotiatedContentResult<PatientCheckModel>));

                readContentResult = (OkNegotiatedContentResult<PatientCheckModel>)resultAlteredPatientCheck;

                Assert.IsTrue(readContentResult.Content.CheckoutDateTime == now);
            }
            using (var FifthPatientCheckController = new PatientChecksController())
            {
                result = FifthPatientCheckController.DeletePatientCheck(readContentResult.Content.PatientCheckID);
            }

        }

        [TestMethod] //Delete PatientCheck [4]
        public void DeletePatientCheck()
        {
            CreatedAtRouteNegotiatedContentResult<PatientCheckModel> contentResult;

            using (var patientCheckController = new PatientChecksController())
            {
                var newPatientCheck = new PatientCheckModel
                {
                    PatientCheckID = 1,
                    PatientID = 1,
                    SpecialtyID = 1,
                    CheckinDateTime = DateTime.Now,
                    CheckoutDateTime = DateTime.Now.AddHours(2)
                };
                var result = patientCheckController.PostPatientCheck(newPatientCheck);

                contentResult = (CreatedAtRouteNegotiatedContentResult<PatientCheckModel>)result;
            }
            using (var SecondPatientCheckController = new PatientChecksController())
            {
                var result = SecondPatientCheckController.DeletePatientCheck(contentResult.Content.PatientCheckID);

                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<PatientCheckModel>));
            }
            using (var ThirdPatientCheckController = new PatientChecksController())
            {
                var result = ThirdPatientCheckController.GetPatientCheck(contentResult.Content.PatientCheckID);

                Assert.IsInstanceOfType(result, typeof(NotFoundResult));

            }
        }
    }
}

