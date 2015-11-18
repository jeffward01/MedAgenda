using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using MedAgenda.CORE.Domain;
using MedAgenda.CORE.Infrastructure;
using MedAgenda.CORE.Models;
using AutoMapper;
using System;

namespace MedAgenda.API.Controllers
{
    public class DoctorsCheckController : ApiController
    {
        private MedAgendaDbContext db = new MedAgendaDbContext();

        // GET: api/DoctorsCheck
        public IEnumerable<DoctorCheckModel> GetDoctorChecks()
        {
            return Mapper.Map<IEnumerable<DoctorCheckModel>>(db.DoctorChecks.Where(pc => !pc.CheckoutDateTime.HasValue));
        }

        // GET: api/DoctorsCheck/5
        [ResponseType(typeof(DoctorCheckModel))]
        public IHttpActionResult GetDoctorCheck(int id)
        {
            DoctorCheck dbDoctorCheck = db.DoctorChecks.Find(id);
            if (dbDoctorCheck == null)
            {
                return NotFound();
            }
            DoctorCheckModel returnedDoctorCheck = Mapper.Map<DoctorCheckModel>(dbDoctorCheck);
            return Ok(returnedDoctorCheck);
        }

        // PUT: api/DoctorsCheck/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDoctorCheck(int id, DoctorCheckModel doctorCheck)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != doctorCheck.DoctorCheckID)
            {
                return BadRequest();
            }
            var dbDoctorCheck = db.DoctorChecks.Find(id);

            dbDoctorCheck.Update(doctorCheck);
          
            db.Entry(dbDoctorCheck).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorCheckExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception("Unable to update the doctor check-in");
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/DoctorsCheck
        [ResponseType(typeof(DoctorCheckModel))]
        public IHttpActionResult PostDoctorCheck(DoctorCheckModel doctorCheck)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var dbDoctorCheck = new DoctorCheck();

            dbDoctorCheck.Update(doctorCheck);

            db.DoctorChecks.Add(dbDoctorCheck);


            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Unable to add the doctor check-in.", e);
            }

            doctorCheck.DoctorCheckID = dbDoctorCheck.DoctorCheckID;
            return CreatedAtRoute("DefaultApi", new { id = dbDoctorCheck.DoctorCheckID }, doctorCheck);
        }

        // DELETE: api/DoctorsCheck/5
        [ResponseType(typeof(DoctorCheckModel))]
        public IHttpActionResult DeleteDoctorCheck(int id)
        {
            DoctorCheck doctorCheck = db.DoctorChecks.Find(id);
            if (doctorCheck == null)
            {
                return NotFound();
            }

            db.DoctorChecks.Remove(doctorCheck);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw new Exception("Unable to delete the doctor check-in");
            }
           

            return Ok(Mapper.Map<DoctorCheckModel>(doctorCheck));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DoctorCheckExists(int id)
        {
            return db.DoctorChecks.Count(e => e.DoctorCheckID == id) > 0;
        }
    }
}