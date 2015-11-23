using MedAgenda.CORE.Domain;
using MedAgenda.CORE.Tests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedAgenda.CORE.Tests.MockDbSets
{
    public class TestAppointmentsDbSet : TestDbSet<Appointment>
    {
        public override Appointment Find(params object[] keyValues)
        {
            var id = (int)keyValues.Single();
            return this.SingleOrDefault(b => b.AppointmentID == id);
        }
    }

    public class TestDoctorChecksDbSet : TestDbSet<DoctorCheck>
    {
        public override DoctorCheck Find(params object[] keyValues)
        {
            var id = (int)keyValues.Single();
            return this.SingleOrDefault(b => b.DoctorCheckID == id);
        }
    }

    public class TestDoctorsDbSet : TestDbSet<Doctor>
    {
        public override Doctor Find(params object[] keyValues)
        {
            var id = (int)keyValues.Single();
            return this.SingleOrDefault(b => b.DoctorID == id);
        }
    }

    public class TestEmergencyContactsDbSet : TestDbSet<EmergencyContact>
    {
        public override EmergencyContact Find(params object[] keyValues)
        {
            var id = (int)keyValues.Single();
            return this.SingleOrDefault(b => b.EmergencyContactID == id);
        }
    }

    public class TestExamRoomsDbSet : TestDbSet<ExamRoom>
    {
        public override ExamRoom Find(params object[] keyValues)
        {
            var id = (int)keyValues.Single();
            return this.SingleOrDefault(b => b.ExamRoomID == id);
        }
    }
    public class TestPatientsDbSet : TestDbSet<Patient>
    {
        public override Patient Find(params object[] keyValues)
        {
            var id = (int)keyValues.Single();
            return this.SingleOrDefault(b => b.PatientID == id);
        }
    }

    public class TestPatientChecksDbSet : TestDbSet<PatientCheck>
    {
        public override PatientCheck Find(params object[] keyValues)
        {
            var id = (int)keyValues.Single();
            return this.SingleOrDefault(b => b.PatientCheckID == id);
        }
    }

    public class TestSpecialtiesDbSet : TestDbSet<Specialty>
    {
        public override Specialty Find(params object[] keyValues)
        {
            var id = (int)keyValues.Single();
            return this.SingleOrDefault(b => b.SpecialtyID == id);
        }
    }
}
