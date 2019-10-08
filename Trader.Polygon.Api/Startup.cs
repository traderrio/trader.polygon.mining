using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Trader.Polygon.Api.Config;
using Trader.Polygon.Api.Hubs;
using Trader.Polygon.Api.Infrastructure;

namespace Trader.Polygon.Api
{
    partial class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(IHostingEnvironment env, ILogger<Startup> logger)
        {
            _logger = logger;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAppSettings(Configuration);
           
            services.AddCustomisedCors();
            services.AddCustomisedMvc();

            services.AddDataLayers(Configuration);
            services.AddServices();
            services.AddJobs();

            services.AddCustomisedSignalR();
            services.AddCustomisedAutoMapper();
            services.AddCustomizedHangfire(Configuration);
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("CorsPolicy");
            app.UseMvc();

            app.UseSignalR(routes => { routes.MapHub<StockPricesHub>("/stocks/prices"); });

            app.UseHangfireServer(new BackgroundJobServerOptions{});
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new [] { new HangfireAuthorizationFilter(),  }
            });
            
            Bootstrap(app.ApplicationServices);
        }
    }
}