﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedAgenda.CORE.Domain
{
    public class Specialty
    {
        public int SpecialtyID { get; set; }
        public string SpecialtyName { get; set; }

        public virtual ICollection<Doctor> Doctors { get; set; }
        public virtual ICollection<PatientCheck> PatientChecks { get; set; }

    }
}
