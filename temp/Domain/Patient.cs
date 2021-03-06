﻿using System;
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
        public string BloodType { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<EmergencyContact> EmergencyContacts { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }

        public virtual ICollection<PatientCheck> PatientChecks { get; set; }
    }
}
