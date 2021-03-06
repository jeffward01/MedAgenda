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
    public class PatientsController : ApiController
    {
        private MedAgendaDbContext db = new MedAgendaDbContext();

        // GET: api/Patients
        // Return all patients with archived indicator set to false
        public IEnumerable<PatientModel> GetPatients()
        {
            var dbPatients = db.Patients.Where(p => !p.Archived);
            return Mapper.Map<IEnumerable<PatientModel>>(dbPatients);
        }

        [Route("api/patients/checkedin")]
        public IEnumerable<PatientModel> GetCheckedInPatients()
        {
            var dbPatients = db.Patients.Where(p => p.PatientChecks.Any(c => !c.CheckoutDateTime.HasValue));

            return Mapper.Map<IEnumerable<PatientModel>>(dbPatients);
        }

        // GET: api/archive/Patients
        // Return all patients with archived indicator set to true
        [Route("api/archive/patients")]
        public IEnumerable<PatientModel> GetArchivedPatients()
        {
            var dbPatients = db.Patients.Where(p => p.Archived);
            return Mapper.Map<IEnumerable<PatientModel>>(dbPatients);
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

        // GET: api/patients/5/appointments
        // Get appointments belonging to patient corresponding to patient ID
        [Route("api/patients/{patientID}/appointments")]
        public IHttpActionResult GetAppointmentsForPatient(int patientID)
        {
            // Validate request
            if (!PatientExists(patientID))
            {
                return BadRequest();
            }

            // Get list of appointments where the patient ID
            //  matches the input patient ID
            var dbAppointments = db.Appointments.Where(a => a.PatientID == patientID);

            if (dbAppointments.Count() == 0)
            {
                return NotFound();
            }

            // Return the list of AppointmentModel objects            
            return Ok(Mapper.Map<IEnumerable<AppointmentModel>>(dbAppointments));
        }

        // GET: api/patients/5/emergencyContacts
        // Get emergency contact(s) belonging to patient corresponding to patient ID
        [Route("api/patients/{patientID}/emergencyContacts")]
        public IHttpActionResult GetEmergencyContactsForPatient(int patientID)
        {
            // Validate request
            if (!PatientExists(patientID))
            {
                return BadRequest();
            }

            // Get list of emergency contacts where the patient ID
            //  matches the input patient ID
            var dbEmergencyContacts = db.EmergencyContacts.Where(ec => ec.PatientID == patientID);

            if (dbEmergencyContacts.Count() == 0)
            {
                return NotFound();
            }

            // Return the list of EmergencyContactModel objects            
            return Ok(Mapper.Map<IEnumerable<EmergencyContactModel>>(dbEmergencyContacts));
        }

        // GET: api/patients/5/patientchecks
        // Get patient check-ins belonging to patient corresponding to patient ID
        [Route("api/patients/{patientID}/patientchecks")]
        public IHttpActionResult GetPatientChecksForPatient(int patientID)
        {
            // Validate request
            if (!PatientExists(patientID))
            {
                return BadRequest();
            }

            // Get list of patient check-ins where the patient ID
            //  matches the input patient ID
            var dbPatientChecks = db.PatientChecks.Where(pc => pc.PatientID == patientID);

            if (dbPatientChecks.Count() == 0)
            {
                return NotFound();
            }

            // Return the list of PatientCheckModel objects            
            return Ok(Mapper.Map<IEnumerable<PatientCheckModel>>(dbPatientChecks));
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
        // Set the patient as archived, instead of deleting it
        [ResponseType(typeof(PatientModel))]
        public IHttpActionResult DeletePatient(int id)
        {

            // Get the patient record corresponding to the patient ID
            Patient dbPatient = db.Patients.Find(id);
            if (dbPatient == null)
            {
                return NotFound();
            }

            //   Set the patient as archived    
            dbPatient.Archived = true;               
            
            // Set indicator that DB has been modified
           db.Entry(dbPatient).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw new Exception ("Unable to delete the patient from the database");
            }           

            return Ok(Mapper.Map<PatientModel>(dbPatient));
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