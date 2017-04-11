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
using ServiceStack.Redis;
using StackExchange.Redis;

namespace Athena.Api.Infrastructure
{
    public interface IRedisProvider
    {
        IRedisClientsManager GetDatabase();
    }

    class RedisProvider : IRedisProvider
    {
        private readonly ILogger<RedisProvider> _logger;
        private readonly AthenaConfiguration _configuration;
        private readonly IRedisClientsManager _clientsManager;
        public RedisProvider(ILogger<RedisProvider> logger, IOptions<AthenaConfiguration> configuration)
        {
            _configuration = configuration.Value;
            _logger = logger;
            _clientsManager = new BasicRedisClientManager(_configuration.RedisHostname);
        }

        public IRedisClientsManager GetDatabase()
        {
            return _clientsManager;
        }
    }
}
