using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using MedAgenda.CORE.Domain;
using MedAgenda.CORE.Models;
using System.Web.Http.Cors;

namespace MedAgenda.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Enable cross origin requests to API
            var cors = new EnableCorsAttribute(
                origins: "*",
                headers: "*",
                methods: "*"
            );
            config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            SetupAutomapper();
        }

        public static void SetupAutomapper()
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
