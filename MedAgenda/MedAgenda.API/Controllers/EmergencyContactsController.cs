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
    public class EmergencyContactsController : ApiController
    {
        private MedAgendaDbContext db = new MedAgendaDbContext();

        // GET: api/EmergencyContacts || Controller Method [0]
        public IEnumerable<EmergencyContactModel> GetEmergencyContacts()
        {
            return Mapper.Map<IEnumerable<EmergencyContactModel>>(db.EmergencyContacts);
        }

        // GET: api/EmergencyContacts/5 || Get By ID [1]
        [ResponseType(typeof(EmergencyContactModel))]
        public IHttpActionResult GetEmergencyContact(int id)
        {
            EmergencyContact dbEmergencyContact = db.EmergencyContacts.Find(id);
            if (dbEmergencyContact == null)
            {
                return NotFound();
            }
            EmergencyContactModel emergencyContact = Mapper.Map<EmergencyContactModel>(dbEmergencyContact);

            return Ok(emergencyContact);
        }

        //We need to get the patient and such. 
        [Route("api/emergencycontacts/{id}/patient")]
        public IHttpActionResult GetPatient(int id)
        {
            var patientSearch = db.Patients.Where(p => p.PatientID == id);

            return Ok(patientSearch.Select(p => new PatientModel
            {
                PatientID = p.PatientID,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Birthdate = p.Birthdate,
                Telephone = p.Telephone,
                Email = p.Email,
                BloodType = p.BloodType,
                CreatedDate = p.CreatedDate,
            }));

        }

        // PUT: api/EmergencyContacts/5/Update EmergencyContacts [2]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEmergencyContact(int id, EmergencyContactModel emergencyContact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != emergencyContact.EmergencyContactID)
            {
                return BadRequest();
            }
            var dbEmergencyContact = db.EmergencyContacts.Find(id);

            dbEmergencyContact.Update(emergencyContact);

            db.Entry(dbEmergencyContact).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmergencyContactExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception("Unable to update the Contact in the database");
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/EmergencyContacts //New EmergencyContact [3]
        [ResponseType(typeof(EmergencyContactModel))]
        public IHttpActionResult PostEmergencyContact(EmergencyContactModel emergencyContact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var dbEmergencyContact = new EmergencyContact();

            dbEmergencyContact.Update(emergencyContact);
            db.EmergencyContacts.Add(dbEmergencyContact);
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw new Exception("Unable to add EC to database");
            }

            emergencyContact.EmergencyContactID = dbEmergencyContact.EmergencyContactID;

            return CreatedAtRoute("DefaultApi", new { id = emergencyContact.EmergencyContactID }, emergencyContact);
        }

        // DELETE: api/EmergencyContacts/5 Delete Doctor [4]
        [ResponseType(typeof(EmergencyContactModel))]
        public IHttpActionResult DeleteEmergencyContact(int id)
        {
            EmergencyContact emergencyContact = db.EmergencyContacts.Find(id);
            if (emergencyContact == null)
            {
                return NotFound();
            }

            db.EmergencyContacts.Remove(emergencyContact);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw new Exception("Could not delete Emergency Contact from database");
            }


            return Ok(Mapper.Map<EmergencyContactModel>(emergencyContact));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EmergencyContactExists(int id)
        {
            return db.EmergencyContacts.Count(e => e.EmergencyContactID == id) > 0;
        }
    }
}