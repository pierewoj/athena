using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Athena.Api.Configuration
{
    public static class EnvironmentVariables
    {
        public static string Hostname => Environment.GetEnvironmentVariable("HOSTNAME");
    }
}
