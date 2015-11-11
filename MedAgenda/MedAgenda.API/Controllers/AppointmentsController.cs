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
    public class AppointmentsController : ApiController
    {
        private MedAgendaDbContext db = new MedAgendaDbContext();

        // GET: api/Appointments
        public IQueryable<Appointment> GetAppointments()
        {
            return db.Appointments;
        }

        // GET: api/Appointments/5
        [ResponseType(typeof(AppointmentModel))]
        public IHttpActionResult GetAppointment(int id)
        {
            Appointment dbAppointment = db.Appointments.Find(id);
            if (dbAppointment == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<AppointmentModel>(dbAppointment));
        }

        // PUT: api/Appointments/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAppointment(int id, Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != appointment.AppointmentID)
            {
                return BadRequest();
            }

            db.Entry(appointment).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(id))
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

        // POST: api/Appointments
        [ResponseType(typeof(AppointmentModel))]
        public IHttpActionResult PostAppointment(AppointmentModel appointment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Appointment dbAppointment = new Appointment();
            dbAppointment.Update(appointment);

            db.Appointments.Add(dbAppointment);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw new Exception ("Unable to add appointment");
            }
            

            appointment.AppointmentID = dbAppointment.AppointmentID;
            return CreatedAtRoute("DefaultApi", new { id = appointment.AppointmentID }, appointment);
        }

        // DELETE: api/Appointments/5
        [ResponseType(typeof(Appointment))]
        public IHttpActionResult DeleteAppointment(int id)
        {
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return NotFound();
            }

            db.Appointments.Remove(appointment);
            db.SaveChanges();

            return Ok(appointment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AppointmentExists(int id)
        {
            return db.Appointments.Count(e => e.AppointmentID == id) > 0;
        }
    }
}