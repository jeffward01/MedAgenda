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
    public class DoctorsController : ApiController
    {
        private MedAgendaDbContext db = new MedAgendaDbContext();

        // GET: api/Doctors
        public IEnumerable<DoctorModel> GetDoctors()
        {
            return Mapper.Map<IEnumerable<DoctorModel>>(db.Doctors);
        }

        // GET: api/Doctors/5
        [ResponseType(typeof(DoctorModel))]
        public IHttpActionResult GetDoctor(int id)
        {
            Doctor dbDoctor = db.Doctors.Find(id);
            if (dbDoctor == null)
            {
                return NotFound();
            }
            DoctorModel doctor = Mapper.Map<DoctorModel>(dbDoctor);

            return Ok(doctor);
        }

        // PUT: api/Doctors/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDoctor(int id, DoctorModel doctor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != doctor.DoctorID)
            {
                return BadRequest();
            }
            var dbDoctor = db.Doctors.Find(id);

            dbDoctor.Update(doctor);

            db.Entry(dbDoctor).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception("Unable to update the Doctor in the database");
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Doctors
        [ResponseType(typeof(DoctorModel))]
        public IHttpActionResult PostDoctor(DoctorModel doctor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var dbDoctor = new Doctor();

            dbDoctor.Update(doctor);
            db.Doctors.Add(dbDoctor);
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw new Exception("Unable to add Doctor to database");
            }

            doctor.DoctorID = dbDoctor.DoctorID;

            return CreatedAtRoute("DefaultApi", new { id = doctor.DoctorID }, doctor);
        }

        // DELETE: api/Doctors/5
        [ResponseType(typeof(DoctorModel))]
        public IHttpActionResult DeleteDoctor(int id)
        {
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return NotFound();
            }

            db.Doctors.Remove(doctor);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw new Exception("Could not delete Doctor from database");
            }
            

            return Ok(Mapper.Map<DoctorModel>(doctor));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DoctorExists(int id)
        {
            return db.Doctors.Count(e => e.DoctorID == id) > 0;
        }
    }
}