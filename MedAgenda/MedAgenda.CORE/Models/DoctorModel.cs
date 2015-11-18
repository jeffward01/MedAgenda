using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedAgenda.CORE.Models
{
    public class DoctorModel
    {
        public int DoctorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public DateTime CreatedDate { get; set; }
        public int SpecialtyID { get; set; }
        public bool Archived { get; set; }

        public int UpcomingAppointmentCount { get; set; }

        public DoctorCheckModel DoctorCheck { get; set; }
        public SpecialtyModel Specialty { get; set; }
    }
}
