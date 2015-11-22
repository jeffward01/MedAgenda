using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedAgenda.CORE.Models;

namespace MedAgenda.CORE.Domain
{
    public class ExamRoom
    {
        public ExamRoom()
        {
            Appointments = new HashSet<Appointment>();
            DoctorChecks = new HashSet<DoctorCheck>();
        }

        public int ExamRoomID { get; set; }
        public string ExamRoomName { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<DoctorCheck> DoctorChecks { get; set; }

        public void Update(ExamRoomModel examRoom)
        {           
            ExamRoomName = examRoom.ExamRoomName;
        }
    }
}
