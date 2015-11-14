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
                DoctorsOnsitePercentage = (int)(0.5f + ((100f * db.Doctors.Count()/db.DoctorChecks.Count()))),
                ExamRoomsFilledPercentage = (int)(0.5f + ((100f * db.Appointments.Count()/db.ExamRooms.Count()))),

                DoctorsCheckedinCount = db.DoctorChecks.Count(),
                PatientsCheckedinCount = db.PatientChecks.Count(),

                AveragePatientAge = db.Patients.Sum(a => GetAge(DateTime.Now(), a.Birthdate) / db.Patients.Count(),


            TotalMonthlyIncome = db.Leases.Sum(l => l.Rent),
                ExpiringLeases = Mapper.Map<IEnumerable<LeaseModel>>(
                    db.Leases.Where(
                                    l => l.EndDate.HasValue &&
                                         l.EndDate >= DateTime.Now &&
                                         l.EndDate <= sixMonthsFromNow
                                   )
                             .OrderBy(l => l.EndDate)
                             .Take(3)
                )
            };
        }
        public static int GetAge(DateTime reference, DateTime birthday)
        {
            int age = reference.Year - birthday.Year;
            if (reference < birthday.AddYears(age)) age--;

            return age;
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
