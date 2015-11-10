using AutoMapper;
using MedAgenda.CORE.Domain;
using MedAgenda.CORE.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Web.Http;

namespace MedAgenda.CORE
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //Change from XML to JSON
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            SetupAutoMapper();
        }

        //Initialize AutoMapper
        public static void SetupAutoMapper()
        {
            Mapper.CreateMap<Appointment, AppointmentModel>();
            Mapper.CreateMap<Doctor, DoctorModel>();
            Mapper.CreateMap<DoctorCheck, DoctorCheckModel>();
            Mapper.CreateMap<EmergencyContact, EmergencyContactModel>();
            Mapper.CreateMap<ExamRoom, ExamRoomModel>();
            Mapper.CreateMap<Patient, PatientModel>();
            Mapper.CreateMap<PatientCheck, PatientCheckModel>();
            Mapper.CreateMap<Specialty, SpecialtyModel>();



        }
    }
}

