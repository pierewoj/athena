using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Athena.Api.Controllers
{
    [Route("/")]
    public class HealthController : Controller
    {
        [Route("/health-check")]
        public string Health()
        {
            return "Athena.Api is alive.";
        }
    }
}
