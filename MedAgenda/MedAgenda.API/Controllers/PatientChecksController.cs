using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using MedAgenda.CORE.Domain;
using MedAgenda.CORE.Infrastructure;
using MedAgenda.CORE.Models;
using AutoMapper;

namespace MedAgenda.API.Controllers
{
    public class PatientChecksController : ApiController
    {
        private MedAgendaDbContext db = new MedAgendaDbContext();

        // GET: api/PatientChecks
        public IQueryable<PatientCheck> GetPatientChecks()
        {
            return db.PatientChecks;
        }

        // GET: api/PatientChecks/5
        [ResponseType(typeof(PatientCheckModel))]
        public IHttpActionResult GetPatientCheck(int id)
        {
            PatientCheck dbPatientCheck = db.PatientChecks.Find(id);
            if (dbPatientCheck == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<PatientCheckModel>(dbPatientCheck));
        }

        // PUT: api/PatientChecks/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPatientCheck(int id, PatientCheck patientCheck)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != patientCheck.PatientCheckID)
            {
                return BadRequest();
            }

            db.Entry(patientCheck).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientCheckExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/PatientChecks
        [ResponseType(typeof(PatientCheckModel))]
        public IHttpActionResult PostPatientCheck(PatientCheckModel patientCheck)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PatientCheck dbPatientCheck = new PatientCheck();
            dbPatientCheck.Update(patientCheck);

            db.PatientChecks.Add(dbPatientCheck);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw new Exception("Unable to add the patient check-in to the database.");
            }
            

            patientCheck.PatientCheckID = dbPatientCheck.PatientCheckID;
            return CreatedAtRoute("DefaultApi", new { id = patientCheck.PatientCheckID }, patientCheck);
        }

        // DELETE: api/PatientChecks/5
        [ResponseType(typeof(PatientCheck))]
        public IHttpActionResult DeletePatientCheck(int id)
        {
            PatientCheck patientCheck = db.PatientChecks.Find(id);
            if (patientCheck == null)
            {
                return NotFound();
            }

            db.PatientChecks.Remove(patientCheck);
            db.SaveChanges();

            return Ok(patientCheck);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PatientCheckExists(int id)
        {
            return db.PatientChecks.Count(e => e.PatientCheckID == id) > 0;
        }
    }
}