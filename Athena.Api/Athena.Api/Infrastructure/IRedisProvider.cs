using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Athena.Api.Configuration;
using Athena.Api.Infrastructure.Services;
using Microsoft.AspNetCore.Razor.CodeGenerators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Athena.Api.Infrastructure
{
    public interface IRedisProvider
    {
        IDatabase GetDatabase();
    }

    class RedisProvider : IRedisProvider
    {
        private readonly ILogger<RedisProvider> _logger;
        private readonly AthenaConfiguration _configuration;

        public RedisProvider(ILogger<RedisProvider> logger, IOptions<AthenaConfiguration> configuration)
        {
            _configuration = configuration.Value;
            _logger = logger;
        }

        public IDatabase GetDatabase()
        {
            IPHostEntry ips = Dns.GetHostEntryAsync(_configuration.RedisHostname).Result;
            var ip = ips.AddressList.First().ToString();
            _logger.LogInformation($"Redis server address was resolved to {ip}");
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(ip);
            return redis.GetDatabase();
        }
    }
}
