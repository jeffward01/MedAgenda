﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedAgenda.CORE.Models;

namespace MedAgenda.CORE.Domain
{
    public class Specialty
    {
        public Specialty()
        {
            Doctors = new HashSet<Doctor>();
            PatientChecks = new HashSet<PatientCheck>();
        }

        public int SpecialtyID { get; set; }
        public string SpecialtyName { get; set; }

        public virtual ICollection<Doctor> Doctors { get; set; }
        public virtual ICollection<PatientCheck> PatientChecks { get; set; }

        public void Update(SpecialtyModel specialty)
        {
            SpecialtyName = specialty.SpecialtyName;
        }
    }
}
