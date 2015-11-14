using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedAgenda.CORE.Models
{
    public class DashboardModel
    {
        public int DoctorTotalCount { get; set; }
        public int PatientTotalCount { get; set; }
        public int ExamRoomTotalCount { get; set; }

        public int OpenExamRoomsCount { get; set; }

        public int ExamRoomsFilledPercentage { get; set; }
        public int DoctorsOnsitePercentage { get; set; }

        public int AveragePatientAge { get; set; }

        public int DoctorsCheckedinCount { get; set; }
        public int PatientsCheckedinCount { get; set; }

        public IEnumerable<AppointmentModel> CurrentAppointments { get; set; }
        public IEnumerable<AppointmentModel> CheckedinDoctors { get; set; }
        public IEnumerable<AppointmentModel> CheckedOutDoctors { get; set; }
        public IEnumerable<AppointmentModel> CheckedinPatients { get; set; }
 

    }
}
