using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<DoctorCheck> DoctorChecks { get; set; }
        public virtual Specialty Specialty { get; set; }
    }
}