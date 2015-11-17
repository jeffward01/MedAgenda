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
    public class SpecialtiesController : ApiController
    {
        private MedAgendaDbContext db = new MedAgendaDbContext();

        // GET: api/Specialties || Controller Method [0]
        public IEnumerable<SpecialtyModel> GetSpecialties()
        {
            return Mapper.Map<IEnumerable<SpecialtyModel>>(db.Specialties);
        }

        // GET: api/Specialties/5 || Get By ID [1]
        [ResponseType(typeof(SpecialtyModel))]
        public IHttpActionResult GetSpecialty(int id)
        {
            Specialty dbSpecialty = db.Specialties.Find(id);
            if (dbSpecialty == null)
            {
                return NotFound();
            }
            SpecialtyModel specialty = Mapper.Map<SpecialtyModel>(dbSpecialty);

            return Ok(specialty);
        }

        // PUT: api/Specialties/5 || Update Specialty [2]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSpecialty(int id, SpecialtyModel specialty)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != specialty.SpecialtyID)
            {
                return BadRequest();
            }

            var dbSpecialty = db.Specialties.Find(id);

            dbSpecialty.Update(specialty);

            db.Entry(dbSpecialty).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpecialtyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception("Unable to update the specialty in the database");
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Specialties || New Specialty [3]
        [ResponseType(typeof(SpecialtyModel))]
        public IHttpActionResult PostSpecialty(SpecialtyModel specialty)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var dbSpecialty = new Specialty();

            dbSpecialty.Update(specialty);
            db.Specialties.Add(dbSpecialty);
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw new Exception("Unable to add the specialty to the database");
            }
            specialty.SpecialtyID = dbSpecialty.SpecialtyID;

            return CreatedAtRoute("DefaultApi", new { id = specialty.SpecialtyID }, specialty);
        }

        // DELETE: api/Specialties/5 || Delete Specialty [4]
        [ResponseType(typeof(SpecialtyModel))]
        public IHttpActionResult DeleteSpecialty(int id)
        {
            Specialty specialty = db.Specialties.Find(id);
            if (specialty == null)
            {
                return NotFound();
            }

            db.Specialties.Remove(specialty);

            try
            {
                db.SaveChanges();

            }
            catch (Exception)
            {
                throw new Exception("Unable to delete the specialty from the database");
            }

            return Ok(Mapper.Map<SpecialtyModel>(specialty));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SpecialtyExists(int id)
        {
            return db.Specialties.Count(e => e.SpecialtyID == id) > 0;
        }
    }
}