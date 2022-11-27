using System;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MindGamesApi.Hubs;
using MindGamesApi.Services;

namespace MindGamesApi;

public class Startup
{
    private static readonly string pythonPath1 = @"C:\Users\mmang\AppData\Local\Programs\Python\Python366";

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
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //if (env.IsDevelopment())
        //{
            app.UseDeveloperExceptionPage();
        //}

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<DigitalSignalProcessingHub>("/dsphub");
        });

        var development = false;

#if (DEBUG)
        development = true;
#endif

        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) =>
        {
            return true;
        };
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSignalR(
            o =>
            {
                o.EnableDetailedErrors = true;
                o.MaximumReceiveMessageSize = long.MaxValue;
            }
        );
        services.AddSingleton<DigitalSignalProcessingHub>();
        services.AddSingleton<ModelTrainingService>();
    }
}
