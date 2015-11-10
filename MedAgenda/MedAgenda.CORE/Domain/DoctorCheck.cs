using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedAgenda.CORE.Domain
{
    public class DoctorCheck
    {
        public int DoctorCheckID { get; set; }
        public int DoctorID { get; set; }
        public int ExamRoomID { get; set; }
        public DateTime CheckinDateTime { get; set; }
        public Nullable<DateTime> CheckoutDateTime { get; set; }

        public virtual Doctor Doctor { get; set; }
        public virtual ExamRoom ExamRoom { get; set; }
    }
}
