//using IronPython.Hosting;
using Microsoft.AspNetCore.SignalR;
//using Microsoft.Scripting.Hosting;
using MindGamesApi.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Python.Runtime;

namespace MindGamesApi.Hubs
{
    public class DigitalSignalProcessingHub : Hub
    {
        public async Task<List<ChannelDataPacket>> ContinuousWaveletTransform(List<ChannelDataPacket> channelData)
        {
            //RunCmd();
            //RunCWTWithIronPython();
            //DoPythonCWT();
            channelData.ForEach(cd => cd.Volts = -cd.Volts);
            return channelData;
        }

        private void DoPythonCWT()
        {
            using (Py.GIL())
            {
                dynamic np = Py.Import("numpy");
                Console.WriteLine(np.cos(np.pi * 2));

                dynamic sin = np.sin;
                Console.WriteLine(sin(5));

                double c = np.cos(5) + sin(5);
                Console.WriteLine(c);

                dynamic a = np.array(new List<float> { 1, 2, 3 });
                Console.WriteLine(a.dtype);

                dynamic b = np.array(new List<float> { 6, 5, 4 }, dtype: np.int32);
                Console.WriteLine(b.dtype);

                Console.WriteLine(a * b);
                Console.ReadKey();
            }
        }

        private void RunCmd()
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "C:/Users/mmang/AppData/Local/Microsoft/WindowsApps/python.exe";
            start.Arguments = "C:/Users/mmang/Desktop/testCWT.py";
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardInput = true;
            //using (Process process = Process.Start(start))
            //{
            //    using (StreamReader reader = process.StandardOutput)
            //    {
            //        string result = reader.ReadToEnd();
            //        Debug.WriteLine(result);
            //    }
            //}

            Process process = Process.Start(start);

            //process.st
        }

        //works, but can't find scipy and can't install scipy... seems like it's an annoying process that only supports 2011 versions of numpy and scipy, ironpython was dropped by ms in 2012
        //private void RunCWTWithIronPython()
        //{
        //    ScriptEngine engine = Python.CreateEngine();
        //    engine.ExecuteFile(@"C:/Users/mmang/Desktop/testCWT.py");
        //}
    }
}