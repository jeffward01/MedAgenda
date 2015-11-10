using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedAgenda.API.Tests.Infrastructure
{
    public class BaseTest
    {
        public BaseTest()
        {
            WebApiConfig.SetupAutomapper();
        }
    }
}
