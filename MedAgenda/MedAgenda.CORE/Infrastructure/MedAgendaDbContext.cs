using MedAgenda.CORE.Domain;
using MedAgenda.CORE.Infrastructure;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedAgenda.CORE.Infrastructure
{
    public class MedAgendaDbContext : IdentityDbContext
    {
        public MedAgendaDbContext () : base("MedAgenda")
        {

        }

        public IDbSet<Appointment> Appointments { get; set; }
        public IDbSet<Doctor> Doctors { get; set; }
        public IDbSet<DoctorCheck> DoctorChecks { get; set; }
        public IDbSet<EmergencyContact> EmergencyContacts { get; set; }
        public IDbSet<ExamRoom> ExamRooms { get; set; }
        public IDbSet<Patient> Patients { get; set; }
        public IDbSet<PatientCheck> PatientChecks { get; set; }
        public IDbSet<Specialty> Specialties { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>().HasKey(a => a.AppointmentID);
            modelBuilder.Entity<Appointment>().HasRequired(a => a.ExamRoom)
                                            .WithMany()
                                            .HasForeignKey(a => a.ExamRoomID);

            modelBuilder.Entity<Doctor>().HasKey(d => d.DoctorID);
            modelBuilder.Entity<Doctor>().HasMany(d => d.Appointments)
                                            .WithRequired(a => a.Doctor)
                                            .HasForeignKey(a => a.DoctorID);
            modelBuilder.Entity<Doctor>().HasRequired(d => d.Specialty)
                                            .WithMany()
                                            .HasForeignKey(d => d.SpecialtyID);

            modelBuilder.Entity<DoctorCheck>().HasKey(d => d.DoctorCheckID);
            modelBuilder.Entity<DoctorCheck>().HasRequired(d => d.Doctor)
                                    .WithMany()
                                    .HasForeignKey(d => d.DoctorID);
            modelBuilder.Entity<DoctorCheck>().HasRequired(d => d.ExamRoom)
                                                .WithMany()
                                                .HasForeignKey(d => d.ExamRoomID);

            modelBuilder.Entity<EmergencyContact>().HasKey(e => e.EmergencyContactID);
            modelBuilder.Entity<EmergencyContact>().HasRequired(e => e.Patient)
                                                    .WithMany()
                                                    .HasForeignKey(e => e.PatientID);

            modelBuilder.Entity<ExamRoom>().HasKey(e => e.ExamRoomID);

            modelBuilder.Entity<Patient>().HasKey(p => p.PatientID);
            modelBuilder.Entity<Patient>().HasMany(d => d.Appointments)
                                            .WithRequired(a => a.Patient)
                                            .HasForeignKey(a => a.PatientID);



            modelBuilder.Entity<PatientCheck>().HasKey(p => p.PatientCheckID);
            modelBuilder.Entity<PatientCheck>().HasRequired(p => p.Patient)
                                                .WithMany()
                                                .HasForeignKey(p => p.PatientID);
            modelBuilder.Entity<PatientCheck>().HasRequired(p => p.Specialty)
                                                .WithMany()
                                                .HasForeignKey(p => p.SpecialtyID);


            modelBuilder.Entity<Specialty>().HasKey(s => s.SpecialtyID);

            base.OnModelCreating(modelBuilder);
        }

    }
}
