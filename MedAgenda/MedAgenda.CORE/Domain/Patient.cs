using MedAgenda.CORE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedAgenda.CORE.Domain
{
   public class Patient
    {
        public int PatientID { get; set; }
        public string FirstName { get; set; }
        public string  LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public string Telephone { get; set;}
        public string Email { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string BloodType { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Archived { get; set; }
        public int Age
        {
            get
            {
                TimeSpan age = DateTime.Now - Birthdate;
                double myAge = (age.TotalDays / 365);
                int Age = (int)myAge;

                return (Age);
            }
        }

        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }

        public virtual ICollection<EmergencyContact> EmergencyContacts { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }

        public virtual ICollection<PatientCheck> PatientChecks { get; set; }

        public void Update(PatientModel patient)
        {
            if (patient.PatientID == 0)
            {
                CreatedDate = DateTime.Now;
            }

            FirstName = patient.FirstName;
            LastName = patient.LastName;
            Birthdate = patient.Birthdate;           
            Telephone = patient.Telephone;
            Email = patient.Email;
            Address1 = patient.Address1;
            Address2 = patient.Address2;
            City = patient.City;
            State = patient.State;                      
            Zip = patient.Zip;
            BloodType = patient.BloodType;
            Archived = patient.Archived;
        }
   



    }
}
