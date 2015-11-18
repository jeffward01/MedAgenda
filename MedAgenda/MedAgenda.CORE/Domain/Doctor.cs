using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MedAgenda.CORE.Models;

namespace MedAgenda.CORE.Domain
{
    public class Doctor
    {
        public int DoctorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public DateTime CreatedDate { get; set; }
        public int SpecialtyID { get; set; }
        public bool Archived { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<DoctorCheck> DoctorChecks { get; set; }
        public virtual Specialty Specialty { get; set; }

        public int UpcomingAppointmentCount
        {
            get
            {
                return Appointments.Count(a => !a.CheckoutDateTime.HasValue);
            }
        }

        public void Update(DoctorModel doctor)
        {           
            FirstName = doctor.FirstName;
            LastName = doctor.LastName;
            Email = doctor.Email;
            Telephone = doctor.Telephone;
            SpecialtyID = doctor.SpecialtyID;
            
            if (doctor.DoctorID == 0)
            {
                CreatedDate = DateTime.Now;
            }
        }
    }
}