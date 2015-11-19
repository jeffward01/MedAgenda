using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MedAgenda.CORE.Models;

namespace MedAgenda.CORE.Domain
{
    public class Doctor
    {
        public Doctor()
        {
            Appointments = new HashSet<Appointment>();
            DoctorChecks = new HashSet<DoctorCheck>();
        }

        public int DoctorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public int SpecialtyID { get; set; }
        public bool Archived { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<DoctorCheck> DoctorChecks { get; set; }
        public virtual Specialty Specialty { get; set; }

        public bool IsCheckedIn
        {
            get
            {
                return DoctorChecks.Count(a => !a.CheckoutDateTime.HasValue) > 0;
            }
        }

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
            Address1 = doctor.Address1;
            Address2 = doctor.Address2;
            City = doctor.City;
            State = doctor.State;
            Zip = doctor.Zip;
            SpecialtyID = doctor.SpecialtyID;
            Archived = doctor.Archived;            
            
            if (doctor.DoctorID == 0)
            {
                CreatedDate = DateTime.Now;
            }
        }
    }
}