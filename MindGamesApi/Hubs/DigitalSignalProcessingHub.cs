using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.SignalR;
using MindGamesApi.Models;
using MindGamesApi.Helpers;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MindGamesApi.Hubs
{
    public class DigitalSignalProcessingHub : Hub
    {
        public async Task<CwtResult> ContinuousWaveletTransformSingleChannel(List<ChannelDataPacket> singleChannelData, bool recordData = false, string dataLabel = "")
        {
            double[][] data;
            bool alphaPrediction = false;

            using (Py.GIL())
            {
                data = PythonMathHelper.DoSingleCWT(singleChannelData);

                alphaPrediction = PythonMathHelper.DoRandomForestClassification(data);

                var maxValue = PythonMathHelper.GetMaxArrayValue(data);
                var minValue = PythonMathHelper.GetMinArrayValue(data);
                var absoluteMaxValue = Math.Max(maxValue, Math.Abs(minValue));

                var scaleFactor = 255d / absoluteMaxValue;

                for (var i = 0; i < data.Length; i++)
                {
                    for (var j = 0; j < data[0].Length; j++)
                    {
                        data[i][j] *= scaleFactor;
                    }
                }
            }

            return new CwtResult() { TransformedData = data, MlPrediction = alphaPrediction };
        }

        public async Task<List<CwtResult>> ContinuousWaveletTransformMultiChannel(List<List<ChannelDataPacket>> channelsData)
        {
            //channelsData = new List<List<ChannelDataPacket>>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            List<CwtResult> channelResults = new List<CwtResult>();

            using (Py.GIL())
            {
                var channelsCwtResult = PythonMathHelper.DoMultiCWT(channelsData);

                foreach (var channelResult in channelsCwtResult)
                {
                    //var alphaPrediction = PythonMathHelper.DoRandomForestClassification(channelResult);

                    var maxValue = PythonMathHelper.GetMaxArrayValue(channelResult);
                    var minValue = PythonMathHelper.GetMinArrayValue(channelResult);
                    var absoluteMaxValue = Math.Max(maxValue, Math.Abs(minValue));

                    var scaleFactor = 255d / absoluteMaxValue;


                    for (var i = 0; i < channelResult.Length; i++)
                    {
                        for (var j = 0; j < channelResult[0].Length; j++)
                        {
                            channelResult[i][j] *= scaleFactor;
                        }
                    }

                    channelResults.Add(new CwtResult() { TransformedData = channelResult });
                }
            }

            //await Task.Delay(125);

            stopwatch.Stop();
            Debug.WriteLine($"milliseconds for cwt multi: {stopwatch.ElapsedMilliseconds}");

            return channelResults;
        }

        public async Task<CwtResult> CwtCollect(CwtInput cwtInput)
        {
            //var data1 = DoCWT(cwtInput.Channel1Data);
            //var data2 = DoCWT(cwtInput.Channel2Data);
            //var data3 = DoCWT(cwtInput.Channel3Data);
            //var data4 = DoCWT(cwtInput.Channel4Data);
            //var data5 = DoCWT(cwtInput.Channel5Data);
            //var data6 = DoCWT(cwtInput.Channel6Data);
            //var data7 = DoCWT(cwtInput.Channel7Data);
            //var data8 = DoCWT(cwtInput.Channel8Data);

            //using (Py.GIL())
            //{
            //    //List<double[][]> allCwtResult = PythonMathService.DoAllCWT(cwtInput);
            //}

            var datas = new List<List<ChannelDataPacket>>()
            {
                cwtInput.Channel1Data,
                cwtInput.Channel2Data,
                cwtInput.Channel3Data,
                cwtInput.Channel4Data,
                cwtInput.Channel5Data,
                cwtInput.Channel6Data,
                cwtInput.Channel7Data,
                cwtInput.Channel8Data
            };

            bool alphaPrediction = false;

            double[][] transformedData = new double[0][];

            foreach(var data in datas)
            {
                //var maxValue = GetMaxArrayValue(data);
                //var minValue = GetMinArrayValue(data);
                //var absoluteMaxValue = Math.Max(maxValue, Math.Abs(minValue));

                //var scaleFactor = 255d / absoluteMaxValue;

                //for (var i = 0; i < data.Length; i++)
                //{
                //    for (var j = 0; j < data[0].Length; j++)
                //    {
                //        data[i][j] *= scaleFactor;
                //    }
                //}

                if(cwtInput.CwtChannelIndex == datas.IndexOf(data))
                {
                    //alphaPrediction = DoRandomForestClassification(data);
                    //transformedData = data;
                }

                var fileName = $"CWT_DATA_channel_{datas.IndexOf(data)}_{cwtInput.DataLabel}";
                //var fileName = $"CWT_DATA_{cwtInput.DataLabel}";
                var filePath = $"C:\\Users\\mmang\\source\\repos\\MindGamesApi\\MindGamesApi\\EEG Data\\cwt data\\{fileName}.csv";

                var record = new ExpandoObject() as IDictionary<string, Object>;

                var flattenedData = data.Select(d => d.Volts).ToArray();

                for (int i = 0; i < flattenedData.Length; i++)
                {
                    record.Add(i.ToString(), flattenedData[i]);
                }

                if (File.Exists(filePath))
                {
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        // Don't write the header again.
                        HasHeaderRecord = false,
                    };
                    using (var stream = File.Open(filePath, FileMode.Append))
                    using (var writer = new StreamWriter(stream))
                    using (var csv = new CsvWriter(writer, config))
                    {
                        csv.WriteRecord(record);
                        csv.NextRecord();
                    }
                }
                else
                {
                    using (var writer = new StreamWriter(filePath))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteRecord(record);
                        csv.NextRecord();
                    }
                }
            }

            return new CwtResult() { TransformedData = transformedData, MlPrediction = alphaPrediction };
        }

        public async Task<double[][]> FastFourierTransform(double[][] allChannelData)
        {
            using (Py.GIL())
            {
                var data = PythonMathHelper.DoFFT(allChannelData);
                return data;
            }
        }
    }
}
