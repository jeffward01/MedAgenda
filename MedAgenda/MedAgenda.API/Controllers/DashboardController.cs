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
    public class DashboardController : ApiController
    {
        private MedAgendaDbContext db = new MedAgendaDbContext();

        // GET: api/DashBoard || Controller Method[0]
        public DashboardModel GetDashboard()
        {
            var sixMonthsFromNow = DateTime.Now.AddMonths(6);


            return new DashboardModel
            {

                DoctorTotalCount = db.Doctors.Count(),
                PatientTotalCount = db.Patients.Count(),
                ExamRoomTotalCount = db.ExamRooms.Count(),

                OpenExamRoomsCount = db.ExamRooms.Count() - db.Appointments.Count(),
                DoctorsOnsitePercentage = (int)(0.5f + ((100f * db.DoctorChecks.Count() / db.Doctors.Count()))),
                ExamRoomsFilledPercentage = (int)(0.5f + ((100f * db.Appointments.Count() / db.ExamRooms.Count()))),

                DoctorsCheckedinCount = db.DoctorChecks.Count(),
                PatientsCheckedinCount = db.PatientChecks.Count(),


                AveragePatientAge = (int)db.Patients.ToList().Average(p => p.Age),


                YoungestPatientAge = db.Patients.ToList().Min(a => a.Age),
                OldestPatientAge = db.Patients.ToList().Max(a => a.Age),

                NumberOfArchivedDoctors = db.Doctors.Count(a => a.Archived == true),
                NumberOfArchivedPatients = db.Patients.Count(a => a.Archived == true),

                CurrentAppointments = Mapper.Map<IEnumerable<AppointmentModel>>(
                    db.Appointments.Where(a => (a.CheckoutDateTime == null))),

            //    //Logic is wrong here
            //    CheckedOutDoctors = Mapper.Map<IEnumerable<DoctorModel>>(
            //        db.Doctors.Where(d => d.DoctorChecks.Count() == null || d.DoctorChecks.All(c => c.CheckoutDateTime.HasValue))),
            //CheckedinPatients = Mapper.Map<IEnumerable<PatientModel>>(
            //        db.Patients.Where(p => p.PatientChecks.Count() != null && p.PatientChecks.All(c => c.CheckinDateTime > DateTime.Today && c.CheckoutDateTime == null))),
                
            //     CheckedinDoctors = Mapper.Map<IEnumerable<DoctorModel>>(
            //        db.Doctors.Where(d => d.DoctorChecks.Count() != null || d.DoctorChecks.All(c => c.CheckinDateTime >= DateTime.Today && c.CheckoutDateTime == null))),

                 ArchivedDoctors = Mapper.Map<IEnumerable<DoctorModel>>(
                     db.Doctors.Where(a => a.Archived == true)),

                ArchivedPatients = Mapper.Map<IEnumerable<PatientModel>>(
                     db.Patients.Where(a => a.Archived == true)),
                   
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
        }
    }
}



