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
    public class PatientsController : ApiController
    {
        private MedAgendaDbContext db = new MedAgendaDbContext();

        // GET: api/Patients
        public IEnumerable<PatientModel> GetPatients()
        {
            return Mapper.Map<IEnumerable<PatientModel>>(db.Patients);
        }

        // GET: api/Patients/5
        [ResponseType(typeof(PatientModel))]
        public IHttpActionResult GetPatient(int id)
        {
            Patient dbPatient = db.Patients.Find(id);
            if (dbPatient == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<PatientModel>(dbPatient));
        }

        // PUT: api/Patients/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPatient(int id, PatientModel patient)
        {
            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != patient.PatientID)
            {
                return BadRequest();
            }

            if (!PatientExists(id))
            {
                return BadRequest();
            }

            // Get the patient record corresponding to the patient ID, 
            //   update its properties to the values in the input PatientModel object,
            //   and then set indicator that the patient record in DB has been modified
            var dbPatient = db.Patients.Find(id);
            dbPatient.Update(patient);
            db.Entry(dbPatient).State = EntityState.Modified;
          
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception ("Unable to update the patient in the database");
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Patients
        [ResponseType(typeof(PatientModel))]
        public IHttpActionResult PostPatient(PatientModel patient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Set up new Patient object,
            //  and populate it with the values from 
            //  the input PatientModel object
            Patient dbPatient = new Patient();
            dbPatient.Update(patient);

            // Add the new Patient object to the DB
            db.Patients.Add(dbPatient);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw new Exception("Unable to add the patient to the database");
            }

            // Update the PatientModel object with the new patient ID
            //  that was placed in the Patient object after the changes
            //  were saved to the DB
            patient.PatientID = dbPatient.PatientID;
            return CreatedAtRoute("DefaultApi", new { id = patient.PatientID }, patient);
        }

        // DELETE: api/Patients/5
        [ResponseType(typeof(PatientModel))]
        public IHttpActionResult DeletePatient(int id)
        {
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return NotFound();
            }            

            try
            {
                // Remove the appointments corresponding to the patient
                var appointments = db.Appointments.Where(a => a.PatientID == patient.PatientID);
                if (appointments != null)
                {
                    foreach (var appointment in appointments)
                    {
                        db.Appointments.Remove(appointment);
                    }
                }

                // Remove the patient check-ins corresponding to the patient
                var patientChecks = db.PatientChecks.Where(pc => pc.PatientID == patient.PatientID);
                if (patientChecks != null)
                {
                    foreach (var patientCheck in patientChecks)
                    {
                        db.PatientChecks.Remove(patientCheck);
                    }
                }

                // Remove the emergency contacts corresponding to the patient
                var emergencyContacts = db.EmergencyContacts.Where(ec => ec.PatientID == patient.PatientID);
                if (emergencyContacts != null)
                {
                    foreach (var emergencyContact in emergencyContacts)
                    {
                        db.EmergencyContacts.Remove(emergencyContact);
                    }
                }

                // Remove the patient
                db.Patients.Remove(patient);
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw new Exception ("Unable to delete the patient from the database");
            }
            

            return Ok(Mapper.Map<PatientModel>(patient));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PatientExists(int id)
        {
            return db.Patients.Count(e => e.PatientID == id) > 0;
        }
    }
}