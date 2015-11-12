using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedAgenda.CORE.Models
{
    public class PatientCheckModel
    {
        public int PatientCheckID { get; set; }
        public int PatientID { get; set; }
        public int SpecialtyID { get; set; }
        public DateTime CheckinDateTime { get; set; }
        public Nullable<DateTime> CheckoutDateTime { get; set; }

    }
}
