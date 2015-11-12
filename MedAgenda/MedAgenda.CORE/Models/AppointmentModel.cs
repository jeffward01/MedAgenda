using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedAgenda.CORE.Models
{
    public class AppointmentModel
    {
        public int AppointmentID { get; set; }
        public int PatientID { get; set; }
        public int DoctorID { get; set; }
        public int ExamRoomID { get; set; }
        public DateTime CheckinDateTime { get; set; }
        public Nullable<DateTime> CheckoutDateTime { get; set; }

        public DoctorModel Doctor { get; set; }
        public PatientModel Patient { get; set; }
        public ExamRoomModel ExamRoom { get; set; }
    }
}
