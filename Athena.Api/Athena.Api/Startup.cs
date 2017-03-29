using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Athena.Api.Configuration;
using Athena.Api.Discovery;
using Athena.Api.Infrastructure.Services;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Athena.Api
{
    public class Startup
    {
        private readonly CancellationTokenSource _consulConfigCancellationTokenSource = new CancellationTokenSource();

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
   
            Configuration = builder.Build();
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.RollingFile("log-{Date}.txt")
                .CreateLogger();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AthenaConfiguration>(Configuration);
            services.AddSingleton<ICrawlablePageService, CrawlablePageService>();
            services.AddSingleton<IShouldCrawlDecider, ShouldCrawlDecider>();
            services.AddMvc();
            services.AddDiscoveryRegistrer();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IDiscoveryRegisterer registrer)
        {
            loggerFactory.AddSerilog();
            app.UseMvc();
            var consulUri = new Uri("http://consul-server:8500");
            registrer.Register(consulUri, cfg =>
            {
                cfg.Address = EnvironmentVariables.Hostname;
                cfg.Port = 80;
                cfg.ServiceName = "Athena-Api";
                cfg.HttpCheck = $"http://{cfg.Address}/health-check";

            });
        }
    }
}
