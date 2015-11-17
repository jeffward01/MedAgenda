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

        // GET: api/Appointments || Controller Method [0]
        public IEnumerable<AppointmentModel> GetAppointments()
        {
            return Mapper.Map<IEnumerable<AppointmentModel>>(db.Appointments);
        }
        // GET: api/appointments/upcoming
        // Return all upcoming appointments
        [Route("api/appointments/upcoming")]
        public IEnumerable<AppointmentModel> GetUpcomingAppointments()
        {
            var dbAppointments = db.Appointments.Where(a => !a.CheckoutDateTime.HasValue);
            return Mapper.Map<IEnumerable<AppointmentModel>>(dbAppointments);
        }

        // GET: api/appointments/past
        // Return all past appointments
        [Route("api/appointments/past")]
        public IEnumerable<AppointmentModel> GetPastAppointments()
        {
            var dbAppointments = db.Appointments.Where(a => a.CheckoutDateTime.HasValue);
            return Mapper.Map<IEnumerable<AppointmentModel>>(dbAppointments);
        }

        // GET: api/Appointments/5 || Controller Method [1]
        [ResponseType(typeof(AppointmentModel))]
        public IHttpActionResult GetAppointment(int id)
        {
            Appointment dbAppt = db.Appointments.Find(id);
            if(dbAppt == null)
            {
                return NotFound();
            }
            AppointmentModel apptModel = Mapper.Map<AppointmentModel>(dbAppt);

            return Ok(apptModel);
        }

        // PUT: api/Appointments/5 || Controller Method [2]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAppointment(int id, AppointmentModel appointment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != appointment.AppointmentID)
            {
                return BadRequest();
            }

            var dbAppt = db.Appointments.Find(id);

            dbAppt.Update(appointment);

            db.Entry(dbAppt).State = EntityState.Modified;

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
                    throw new Exception("Unable to update the Appointment in the database");
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Appointments || Controller Method [3]
        [ResponseType(typeof(AppointmentModel))]
        public IHttpActionResult PostAppointment(AppointmentModel appointment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var dbAppt = new Appointment();

            dbAppt.Update(appointment);
            db.Appointments.Add(dbAppt);
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw new Exception("Unable to add Appointment to database");
            }

            appointment.AppointmentID = dbAppt.AppointmentID;

            return CreatedAtRoute("DefaultApi", new { id = appointment.AppointmentID }, appointment);
        }

        // DELETE: api/Appointments/5 || Controller Method [4]
        [ResponseType(typeof(AppointmentModel))]
        public IHttpActionResult DeleteAppointment(int id)
        {
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return NotFound();
            }

            db.Appointments.Remove(appointment);
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw new Exception("Could not delete Appointment from database");
            }


            return Ok(Mapper.Map<AppointmentModel>(appointment));
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