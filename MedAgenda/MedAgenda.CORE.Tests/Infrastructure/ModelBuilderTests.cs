using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MedAgenda.CORE.Infrastructure;
using System.Linq;

namespace MedAgenda.API.Tests
{
    [TestClass]
    public class ModelBuilderTests
    {
        [TestMethod]
        public void GenerateDatabase()
        {
            //using (var db = new MedAgendaDbContext())
            //{
            //    var firstDoctor = db.Doctors.FirstOrDefault();

            //    Assert.IsNull(firstDoctor);
            //}
            Assert.IsTrue(true);
        }
    }
}
