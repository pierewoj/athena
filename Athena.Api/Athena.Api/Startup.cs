using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Athena.Api.Configuration;
using Athena.Api.Infrastructure;
using Athena.Api.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Athena.Api
{
    public class Startup
    {
        private readonly CancellationTokenSource _consulConfigCancellationTokenSource = new CancellationTokenSource();

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false);
            Configuration = builder.Build();
   
            Configuration = builder.Build();
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AthenaConfiguration>(Configuration.GetSection("AthenaConfiguration"));
            services.AddSingleton<ICrawlablePageService, CrawlablePageService>();
            services.AddSingleton<IShouldCrawlDecider, ShouldCrawlDecider>();
            services.AddSingleton<IRedisProvider, RedisProvider>();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();
            app.UseMvc();
        }
    }
}
