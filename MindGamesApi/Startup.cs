using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MindGamesApi.Hubs;

namespace MindGamesApi
{
    public class Startup
    {
        private static readonly string pythonPath1 = @"C:\Users\mmang\AppData\Local\Programs\Python\Python37";

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;

            var pathToPython = pythonPath1;
            var path = pathToPython +
                       ";" +
                       Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
            Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("PYTHONHOME", pathToPython, EnvironmentVariableTarget.Process);

            //var lib = new[]
            //{
            //    @"C:\Users\mmang\<your python code is here>",
            //    Path.Combine(pathToPython, "Lib"),
            //    Path.Combine(pathToPython, "DLLs")

            //};
            //string paths = string.Join("; ", lib);
            //Environment.SetEnvironmentVariable("PYTHONPATH", paths, EnvironmentVariableTarget.Process);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                    //endpoints.MapHub<DigitalSignalProcessingHub>("/dsphub");
                }
            );

            app.UseSignalR(routes => { routes.MapHub<DigitalSignalProcessingHub>("/dsphub"); });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSignalR(
                o =>
                {
                    o.EnableDetailedErrors = true;
                    o.MaximumReceiveMessageSize = 9999999; // bytes
                    //o.s
                }
            );
        }
    }
}
