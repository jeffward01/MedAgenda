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

        // GET: api/EmergencyContacts
        public IEnumerable<EmergencyContactModel> GetEmergencyContacts()
        {           
            return Mapper.Map<IEnumerable<EmergencyContactModel>>(db.EmergencyContacts);
        }

        // GET: api/EmergencyContacts/5
        [ResponseType(typeof(EmergencyContactModel))]
        public IHttpActionResult GetEmergencyContact(int id)
        {
            EmergencyContact dbEmergencyContact = db.EmergencyContacts.Find(id);
            if (dbEmergencyContact == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map< EmergencyContactModel>(dbEmergencyContact));
        }

        // PUT: api/EmergencyContacts/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEmergencyContact(int id, EmergencyContact emergencyContact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != emergencyContact.EmergencyContactID)
            {
                return BadRequest();
            }

            db.Entry(emergencyContact).State = EntityState.Modified;

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
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/EmergencyContacts
        [ResponseType(typeof(EmergencyContactModel))]
        public IHttpActionResult PostEmergencyContact(EmergencyContactModel emergencyContact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            EmergencyContact dbEmergencyContact = new EmergencyContact();
            dbEmergencyContact.Update(emergencyContact);

            db.EmergencyContacts.Add(dbEmergencyContact);
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw new Exception("Unable to add the emergency contact to the database.");
            }

            emergencyContact.EmergencyContactID = dbEmergencyContact.EmergencyContactID;
            return CreatedAtRoute("DefaultApi", new { id = emergencyContact.EmergencyContactID }, emergencyContact);
        }

        // DELETE: api/EmergencyContacts/5
        [ResponseType(typeof(EmergencyContact))]
        public IHttpActionResult DeleteEmergencyContact(int id)
        {
            EmergencyContact emergencyContact = db.EmergencyContacts.Find(id);
            if (emergencyContact == null)
            {
                return NotFound();
            }

            db.EmergencyContacts.Remove(emergencyContact);
            db.SaveChanges();

            return Ok(emergencyContact);
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