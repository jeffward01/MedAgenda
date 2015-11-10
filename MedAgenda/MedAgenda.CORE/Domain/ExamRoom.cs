using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedAgenda.CORE.Domain
{
    public class ExamRoom
    {
        public int ExamRoomID { get; set; }
        public string ExamRoomName { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<DoctorCheck> DoctorChecks { get; set; }
    }
}
