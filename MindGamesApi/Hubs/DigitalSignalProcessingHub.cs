using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.SignalR;
using MindGamesApi.Helpers;
using MindGamesApi.Models;
using MindGamesApi.Training.Pipelines;
using Newtonsoft.Json;
using Python.Runtime;

namespace MindGamesApi.Hubs;

public class DigitalSignalProcessingHub : Hub
{
    public async Task<PredictionResult> ContinuousWaveletTransformMultiChannel(List<List<ChannelDataPacket>> channelsData, bool returnData)
    {
        //channelsData = new List<List<ChannelDataPacket>>();

        var result = new PredictionResult();

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var channelResults = new List<CwtResult>();

        using (Py.GIL())
        {
            var channelsCwtResult = PythonMathHelper.DoMultiCWT(channelsData);
            var flattenedChannelData = new List<double>();

            foreach (var channelResult in channelsCwtResult)
            {
                var semiFlattenedChannelData = channelResult.SelectMany(d => d);

                flattenedChannelData.AddRange(semiFlattenedChannelData);
            }

            result.EyesClosedPrediction = PythonMathHelper.DoEyesClosedClassificationCWT(flattenedChannelData.ToArray());

            if (returnData)
            {
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

                    channelResults.Add(new CwtResult { TransformedData = channelResult });
                }
            }
        }

        if (returnData)
        {
            result.CwtResults = channelResults;
        }
        else
        {
            result.CwtResults = new List<CwtResult>();
        }

        stopwatch.Stop();
        Debug.WriteLine($"milliseconds for cwt multi: {stopwatch.ElapsedMilliseconds}");

        return result;
    }

    public async Task<CwtResult> ContinuousWaveletTransformSingleChannel(List<ChannelDataPacket> singleChannelData, bool recordData = false, string dataLabel = "")
    {
        double[][] data;
        var alphaPrediction = false;

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

        return new CwtResult
        {
            TransformedData = data,
            MlPrediction = alphaPrediction
        };
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

        var datas = new List<List<ChannelDataPacket>>
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

        var alphaPrediction = false;

        var transformedData = new double[0][];

        foreach (var data in datas)
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

            if (cwtInput.CwtChannelIndex == datas.IndexOf(data))
            {
                //alphaPrediction = DoRandomForestClassification(data);
                //transformedData = data;
            }

            var fileName = $"CWT_DATA_channel_{datas.IndexOf(data)}_{cwtInput.DataLabel}";
            //var fileName = $"CWT_DATA_{cwtInput.DataLabel}";
            var filePath = $"C:\\Users\\mmang\\source\\repos\\MindGamesApi\\MindGamesApi\\EEG Data\\cwt data\\{fileName}.csv";

            var record = new ExpandoObject() as IDictionary<string, object>;

            var flattenedData = data.Select(d => d.Volts).ToArray();

            for (var i = 0; i < flattenedData.Length; i++)
            {
                record.Add(i.ToString(), flattenedData[i]);
            }

            if (File.Exists(filePath))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    // Don't write the header again.
                    HasHeaderRecord = false
                };

                using (var stream = File.Open(filePath, FileMode.Append))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        using (var csv = new CsvWriter(writer, config))
                        {
                            csv.WriteRecord(record);
                            csv.NextRecord();
                        }
                    }
                }
            }
            else
            {
                using (var writer = new StreamWriter(filePath))
                {
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteRecord(record);
                        csv.NextRecord();
                    }
                }
            }
        }

        return new CwtResult
        {
            TransformedData = transformedData,
            MlPrediction = alphaPrediction
        };
    }

    public async Task<double[][]> FastFourierTransform(List<List<ChannelDataPacket>> allChannelData)
    {
        using (Py.GIL())
        {
            var data = PythonMathHelper.DoFFT(allChannelData);

            return data;
        }
    }

    public async Task<PredictionResult> FFTMultiChannel(List<List<ChannelDataPacket>> channelsData)
    {
        //channelsData = new List<List<ChannelDataPacket>>();

        var flattenedChannelsData = new List<double>();

        foreach (var channelData in channelsData)
        {
            flattenedChannelsData.AddRange(channelData.Select(cd => Math.Abs(cd.Volts)));
        }

        var absoluteMaximum = flattenedChannelsData.Max();

        var scaledChannelsData = new List<List<ChannelDataPacket>>
        {
            new(),
            new(),
            new(),
            new(),
            new(),
            new(),
            new(),
            new()
        };

        foreach (var channel in channelsData)
        {
            var channelIndex = channelsData.IndexOf(channel);
            scaledChannelsData[channelIndex] = channel
                                              .Select(
                                                   c =>
                                                       new ChannelDataPacket
                                                       {
                                                           Volts = c.Volts / absoluteMaximum,
                                                           TimeStamp = c.TimeStamp
                                                       }
                                               )
                                              .ToList();
        }

        var result = new PredictionResult();

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        //using (Py.GIL())
        //{
        var channelsCwtResult = PythonMathHelper.DoFFT(scaledChannelsData);
        var flattenedChannelData = new List<double>();

        foreach (var channelResult in channelsCwtResult)
        {
            flattenedChannelData.AddRange(channelResult);
        }

        result.EyesClosedPrediction = PythonMathHelper.DoEyesClosedClassificationFFT(flattenedChannelData.ToArray());
        //}

        stopwatch.Stop();
        Debug.WriteLine(
            $"milliseconds for fft multi: {stopwatch.ElapsedMilliseconds}. result: {result.EyesClosedPrediction.EyesClosedProbability} and {result.EyesClosedPrediction.EyesOpenProbability}"
        );

        return result;
    }

    public void TrainAndEvaluateEegClassifier(string blobbedChannelsData)
    {
        var channelsData = JsonConvert.DeserializeObject<List<ChannelsDataPacketFlattenedLabeled>>(blobbedChannelsData);

        //var pipeline = (MultiClassifierPipeline)Activator.CreateInstance(
        //    Type.GetType(pipelineTypeName),
        //    channelsData
        //);

        var pipeline = new MultiClassifierPipeline(channelsData);

        pipeline.Run();
    }

    public void TrainAndEvaluatePenguinClassifier()
    {
        var test = new PenguinPipeline();
        test.Run();
    }
}
