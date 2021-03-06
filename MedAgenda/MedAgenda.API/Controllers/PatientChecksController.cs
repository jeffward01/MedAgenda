﻿using System;
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
        public IEnumerable<PatientCheckModel> GetPatientChecks()
        {
            return Mapper.Map<IEnumerable<PatientCheckModel>>(db.PatientChecks.Where(pc => !pc.CheckoutDateTime.HasValue));
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
            PatientCheckModel patientCheck = Mapper.Map<PatientCheckModel>(dbPatientCheck);

            return Ok(patientCheck);
        }
        
        // PUT: api/PatientChecks/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPatientCheck(int id, PatientCheckModel patientCheck)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != patientCheck.PatientCheckID)
            {
                return BadRequest();
            }
            var dbPatientCheck = db.PatientChecks.Find(id);

            dbPatientCheck.Update(patientCheck);

            db.Entry(dbPatientCheck).State = EntityState.Modified;

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
                    throw new Exception("Unable to update the patient check-in");
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/PatientChecks
        [ResponseType(typeof(PatientCheck))]
        public IHttpActionResult PostPatientCheck(PatientCheckModel patientCheck)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var dbPatientCheck = new PatientCheck();

            dbPatientCheck.Update(patientCheck);

            db.PatientChecks.Add(dbPatientCheck);

            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Unable to add the patient Check in", e);
            }

            patientCheck.PatientCheckID = dbPatientCheck.PatientCheckID;

            return CreatedAtRoute("DefaultApi", new { id = patientCheck.PatientCheckID }, patientCheck);
        }

        // DELETE: api/PatientChecks/5
        [ResponseType(typeof(PatientCheckModel))]
        public IHttpActionResult DeletePatientCheck(int id)
        {
            PatientCheck patientCheck = db.PatientChecks.Find(id);
            if (patientCheck == null)
            {
                return NotFound();
            }

            db.PatientChecks.Remove(patientCheck);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw new Exception("Unable to delete the patient check-in");
            }
            

            return Ok(Mapper.Map<PatientCheckModel>(patientCheck));
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