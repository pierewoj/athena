using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Athena.Api.Configuration;
using Consul;
using Microsoft.Extensions.Logging;

namespace Athena.Api.Discovery
{
    public class DiscoveryRegisterConfiguration
    {
        public string ServiceName { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public string HttpCheck { get; set; }
    }

    public class DiscoveryRegisterer : IDiscoveryRegisterer
    {
        private readonly ILogger<DiscoveryRegisterer> _logger;

        public DiscoveryRegisterer(ILogger<DiscoveryRegisterer> logger)
        {
            _logger = logger;
        }

        public void Register(Uri consulUri, Action<DiscoveryRegisterConfiguration> configuratior)
        {
            var config = new DiscoveryRegisterConfiguration();
            configuratior(config);
            
            using(_logger.BeginScope("Discovery registration"))
            using (var consul = new ConsulClient(c => c.Address = consulUri))
            {
                _logger.LogInformation($"Registring service to consul. " +
                                       $"Address={config.Address}, " +
                                       $"Port={config.Port}, " +
                                       $"ServiceName={config.ServiceName}, " +
                                       $"Check={config.HttpCheck}");
                var registration = new AgentServiceRegistration()
                {
                    ID = $"{config.ServiceName}:{config.Address}",
                    Address = config.Address,
                    Name = config.ServiceName,
                    Port = config.Port,
                };

                if(config.HttpCheck != null)
                    registration.Check = new AgentCheckRegistration()
                    {
                        HTTP = config.HttpCheck,
                        ID = "Http health-check",
                        Interval = TimeSpan.FromSeconds(5),
                        DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                    };
                try
                {
                    var res = consul.Agent.ServiceRegister(registration, CancellationToken.None).Result;
                }
                catch (Exception e)
                {
                    _logger.LogError(0,e,$"Consul registration failed");
                    throw;
                }
            }
        }
    }
}
