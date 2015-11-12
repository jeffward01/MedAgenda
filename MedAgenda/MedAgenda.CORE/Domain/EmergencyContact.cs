using MedAgenda.CORE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedAgenda.CORE.Domain
{
   public class EmergencyContact
    {
        public int EmergencyContactID { get; set; }
        public int PatientID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Relationship { get; set; }

        public virtual Patient Patient { get; set; }

        public void Update(EmergencyContactModel emergencyContact)
        {
            PatientID = emergencyContact.PatientID;
            FirstName = emergencyContact.FirstName;
            LastName = emergencyContact.LastName;
            Telephone = emergencyContact.Telephone;
            Email = emergencyContact.Email;
            Relationship = emergencyContact.Relationship;

        }
    }
}
