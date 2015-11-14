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
        public int YoungestPatientAge { get; set; }
        public int OldestPatientAge { get; set; }

        public int DoctorsCheckedinCount { get; set; }
        public int PatientsCheckedinCount { get; set; }

        public int NumberOfArchivedDoctors { get; set; }
        public int NumberOfArchivedPatients { get; set; }
       
     

        public IEnumerable<AppointmentModel> CurrentAppointments { get; set; }
        public IEnumerable<DoctorModel> CheckedinDoctors { get; set; }
        public IEnumerable<DoctorModel> CheckedOutDoctors { get; set; }
        public IEnumerable<PatientModel> CheckedinPatients { get; set; }
        public IEnumerable<PatientModel> ArchivedPatients { get; set; }
        public IEnumerable<DoctorModel> ArchivedDoctors { get; set; }
 

    }
}
