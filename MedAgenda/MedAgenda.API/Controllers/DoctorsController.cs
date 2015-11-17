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

        // GET: api/Doctors || Controller Method [0]
        public IEnumerable<DoctorModel> GetDoctors()
        {
            //Grab non-archieved doctors
            var dbDoctor = db.Doctors.Where(p => !p.Archived);
            return Mapper.Map<IEnumerable<DoctorModel>>(dbDoctor);
        }

        // GET: api/archive/Patients
        // Return all doctors with archived indicator set to true
        [Route("api/archive/doctors")]
        public IEnumerable<DoctorModel> GetArchivedDoctors()
        {
            var dbDoctors = db.Doctors.Where(p => p.Archived);
            return Mapper.Map<IEnumerable<DoctorModel>>(dbDoctors);
        }

        // GET: api/Doctors/5 || Get By ID [1]
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

        // PUT: api/Doctors/5 //Update Doctor [2]
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
                    throw new Exception("Unable to update the doctor in the database");
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Doctors //New Doctor [3]
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

                throw new Exception("Unable to add the doctor to the database");
            }

            doctor.DoctorID = dbDoctor.DoctorID;

            return CreatedAtRoute("DefaultApi", new { id = doctor.DoctorID }, doctor);
        }

     
        [ResponseType(typeof(DoctorModel))] // Delete Doctor [4]
        public IHttpActionResult DeleteDoctor(int id)
        {

            // Get the doctor record corresponding to the doctor ID
            Doctor dbDoctor = db.Doctors.Find(id);
            if (dbDoctor == null)
            {
                return NotFound();
            }

            //   Set the doctor as archived    
            dbDoctor.Archived = true;

            // Set indicator that DB has been modified
            db.Entry(dbDoctor).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw new Exception("Unable to delete the doctor from the database");
            }


            return Ok(Mapper.Map<DoctorModel>(dbDoctor));

        }


        // GET: api/doctors/5/doctorchecks
        // Get doctors check-ins belonging to doctors corresponding to doctor ID
        [Route("api/doctors/{doctorID}/doctorchecks")] // Get all Doctor Checks Method [5]
        public IHttpActionResult GetDoctorChecksForDoctor(int doctorId)
        {
            // Validate request
            if (!DoctorExists(doctorId))
            {
                return BadRequest();
            }

            // Get list of doctor check-ins where the doctor ID
            //  matches the input doctor ID
            var dbDoctorChecks = db.DoctorChecks.Where(dc => dc.DoctorID == doctorId);

            if (dbDoctorChecks.Count() == 0)
            {
                return NotFound();
            }

            // Return the list of PatientCheckModel objects            
            return Ok(Mapper.Map<IEnumerable<DoctorCheckModel>>(dbDoctorChecks));
        }



        // GET: api/doctors/5/appointments
        // Get appointments belonging to doctors corresponding to doctor ID
        [Route("api/doctor/{doctorId}/appointments")] // Get all appointments for DoctorID Method [5]
        public IHttpActionResult GetAppointmentForDoctorId(int doctorId)
        {
            // Validate request
            if (!DoctorExists(doctorId))
            {
                return BadRequest();
            }

            // Get list of appointments that
            //  match the input doctor ID
            var dbDoctorAppointment = db.Appointments.Where(d => d.DoctorID == doctorId);

            if (dbDoctorAppointment.Count() == 0)
            {
                return NotFound();
            }

            // Return the list of appointmentModel objects            
            return Ok(Mapper.Map<IEnumerable<AppointmentModel>>(dbDoctorAppointment));
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