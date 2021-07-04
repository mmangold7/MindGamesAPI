using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MindGamesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        //static WeatherForecastController() => Runtime.PythonDLL = "C:/Program Files/WindowsApps/PythonSoftwareFoundation.Python.3.8_3.8.2800.0_x64__qbz5n2kfra8p0/python38.dll";

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        //[HttpGet]
        //public void Get()
        //{
        //    //var test = Runtime.PythonDLL;

        //    //await RunCmd();
        //    DoPythonCWT();
        //}

        private async Task RunCmd()
        {
            ProcessStartInfo start = new ProcessStartInfo();

            Process python = new Process();

            python.StartInfo.FileName = "C:/Users/mmang/AppData/Local/Microsoft/WindowsApps/python.exe";
            python.StartInfo.Arguments = "C:/Users/mmang/Desktop/testCWT.py";
            python.StartInfo.UseShellExecute = false;
            python.StartInfo.RedirectStandardOutput = true;
            python.StartInfo.RedirectStandardInput = true;

            python.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (!String.IsNullOrEmpty(e.Data))
                {
                    Debug.WriteLine(e.Data);
                }
            });

            python.Start();

            //using (Process process = Process.Start(start))
            //{


            //python.StandardInput.WriteLine("C:/Users/mmang/Desktop/testCWT.py");
            //var result1 = python.StandardOutput.ReadToEnd();
            //Debug.WriteLine(result1);

            //python.StandardInput.WriteLine("C:/Users/mmang/Desktop/testCWT.py");
            //var result2 = python.StandardOutput.ReadToEnd();
            //Debug.WriteLine(result2);

            //}

            //Process process = Process.Start(start);

            //process.st
        }

        //private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        //{
        //    Debug.WriteLine(e.Data);
        //}

        private void DoPythonCWT()
        {
            using (Py.GIL())
            {
                //dynamic np = Py.Import("numpy");
                //Console.WriteLine(np.cos(np.pi * 2));

                //dynamic sin = np.sin;
                //Console.WriteLine(sin(5));

                //double c = np.cos(5) + sin(5);
                //Console.WriteLine(c);

                //dynamic a = np.array(new List<float> { 1, 2, 3 });
                //Console.WriteLine(a.dtype);

                //dynamic b = np.array(new List<float> { 6, 5, 4 }, dtype: np.int32);
                //Console.WriteLine(b.dtype);

                dynamic a = 1;
                dynamic b = 2;

                //Console.WriteLine(a * b);
                //Console.ReadKey();
            }
        }
    }
}
