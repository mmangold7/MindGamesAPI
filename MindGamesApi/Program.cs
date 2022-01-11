using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Python.Runtime;

namespace MindGamesApi
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        public static void Main(string[] args)
        {
            PythonEngine.Initialize();
            PythonEngine.BeginAllowThreads();
            CreateHostBuilder(args).Build().Run();
        }
    }
}
