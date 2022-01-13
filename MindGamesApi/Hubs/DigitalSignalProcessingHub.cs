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
using MindGamesApi.Training.Trainer;
using Newtonsoft.Json;
using Python.Runtime;

namespace MindGamesApi.Hubs;

public class DigitalSignalProcessingHub : Hub //<IDigitalSignalProcessingHub>
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

    public void DebugMessageClient(string message) //todo:overload this to have a debugwrteline that does the newline for you
    {
        var client = this.Clients.Client(this.Context.ConnectionId).SendAsync("PipelineDebugMessage", message);
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

    public Task<string> PredictEegClassifier(List<ChannelsDataPacketFlattenedLabeled> channelsData, string modelName)
    {
        //var channelsData = JsonConvert.DeserializeObject<List<ChannelsDataPacketFlattenedLabeled>>(blobbedChannelsData);

        var dataGroupedByLabel = channelsData.GroupBy(cd => cd.Label).Select(g => g.ToList()).ToArray();

        var predictor = new Predictor(modelName);
        var wrappedBins = this.PreProcessEegData(dataGroupedByLabel);
        var prediction = predictor.Predict(wrappedBins[0]);

        return Task.FromResult(prediction.PredictedLabel);
    }

    public Task<string> TrainAndEvaluateEegClassifier(string blobbedChannelsData, string modelName)
    {
        var channelsData = JsonConvert.DeserializeObject<List<ChannelsDataPacketFlattenedLabeled>>(blobbedChannelsData);

        var dataGroupedByLabel = channelsData.GroupBy(cd => cd.Label).Select(g => g.ToList()).ToArray();

        var dataBinned = this.PreProcessEegData(dataGroupedByLabel);

        var pipeline = new MultiClassifierPipeline(this, dataBinned, modelName);

        var pipelineRunResult = pipeline.Run();

        return Task.FromResult(pipelineRunResult);
    }

    public void TrainAndEvaluatePenguinClassifier()
    {
        var test = new PenguinPipeline(this);
        test.Run();
    }

    private List<BinnedTransformedDataPacketsResult> PreProcessEegData(List<ChannelsDataPacketFlattenedLabeled>[] labeledData)
    {
        var binnedTransformedLabeledData = new List<BinnedTransformedDataPacketsResult>();

        foreach (var labelGroup in labeledData)
        {
            this.DebugMessageClient($"Processing data set \"{labelGroup.First().Label}\"{Environment.NewLine}");

            var tooLittleForAPacketRemainderLength = labelGroup.Count % 256;
            var clippedLabeledData = labelGroup;
            clippedLabeledData.RemoveRange(labelGroup.Count - tooLittleForAPacketRemainderLength, tooLittleForAPacketRemainderLength);

            var totalTimeIntervals = clippedLabeledData.Count / 256;

            this.DebugMessageClient($"Chunked data into 1 second intervals{Environment.NewLine}");
            this.DebugMessageClient($"Beginning to transform time data{Environment.NewLine}");

            for (var index = 0; index < totalTimeIntervals; index++)
            {
                var timePeriodPackets = clippedLabeledData.Skip(index * 256).Take(256).ToList();

                var channel1DataPackets = timePeriodPackets.Select(
                    p => new ChannelDataPacket
                    {
                        TimeStamp = p.TimeStamp,
                        Volts = p.Channel1Volts
                    }
                );
                var channel2DataPackets = timePeriodPackets.Select(
                    p => new ChannelDataPacket
                    {
                        TimeStamp = p.TimeStamp,
                        Volts = p.Channel2Volts
                    }
                );
                var channel3DataPackets = timePeriodPackets.Select(
                    p => new ChannelDataPacket
                    {
                        TimeStamp = p.TimeStamp,
                        Volts = p.Channel3Volts
                    }
                );
                var channel4DataPackets = timePeriodPackets.Select(
                    p => new ChannelDataPacket
                    {
                        TimeStamp = p.TimeStamp,
                        Volts = p.Channel4Volts
                    }
                );
                var channel5DataPackets = timePeriodPackets.Select(
                    p => new ChannelDataPacket
                    {
                        TimeStamp = p.TimeStamp,
                        Volts = p.Channel5Volts
                    }
                );
                var channel6DataPackets = timePeriodPackets.Select(
                    p => new ChannelDataPacket
                    {
                        TimeStamp = p.TimeStamp,
                        Volts = p.Channel6Volts
                    }
                );
                var channel7DataPackets = timePeriodPackets.Select(
                    p => new ChannelDataPacket
                    {
                        TimeStamp = p.TimeStamp,
                        Volts = p.Channel7Volts
                    }
                );
                var channel8DataPackets = timePeriodPackets.Select(
                    p => new ChannelDataPacket
                    {
                        TimeStamp = p.TimeStamp,
                        Volts = p.Channel8Volts
                    }
                );

                var channelsDataPackets = new List<List<ChannelDataPacket>>
                {
                    channel1DataPackets.ToList(),
                    channel2DataPackets.ToList(),
                    channel3DataPackets.ToList(),
                    channel4DataPackets.ToList(),
                    channel5DataPackets.ToList(),
                    channel6DataPackets.ToList(),
                    channel7DataPackets.ToList(),
                    channel8DataPackets.ToList()
                };

                var timePeriodResult = new BinnedTransformedDataPacketsResult { Label = labelGroup.First().Label };

                foreach (var channelDataPackets in channelsDataPackets)
                {
                    //analyze data

                    var fftData = channelDataPackets.Select(d => d.Volts);
                    var ffttimePeriodPackets = fftData.ToList();

                    double[] fftResults = { };
                    double fftTimeScale = 1;

                    //try to remember to calculate the relative agnitude of each bin with the bins for a given channels compared to each other for normalization, nothe channels compared
                    if (ffttimePeriodPackets.Count() >= 256)
                    {
                        var timeSeries = ffttimePeriodPackets.ToArray();

                        var n = Convert.ToUInt32(ffttimePeriodPackets.Count());
                        var zeros = Convert.ToUInt32(0);
                        var samplingRateHz = Convert.ToDouble(250);

                        var selectedWindowName = "None";
                        var windowToApply = (DSP.Window.Type)Enum.Parse(typeof(DSP.Window.Type), selectedWindowName);

                        // Apply window to the time series data
                        var wc = DSP.Window.Coefficients(windowToApply, n);

                        var windowScaleFactor = DSP.Window.ScaleFactor.Signal(wc);
                        var windowedTimeSeries = DSP.Math.Multiply(timeSeries, wc);

                        var fft = new FFT();
                        fft.Initialize(n, zeros);

                        // Perform a DFT
                        var cpxResult = fft.Execute(windowedTimeSeries);

                        // Convert the complex result to a scalar magnitude 
                        var magResult = DSP.ConvertComplex.ToMagnitude(cpxResult);
                        magResult = DSP.Math.Multiply(magResult, windowScaleFactor);

                        var frequencyPerBin = samplingRateHz / magResult.Length; //what it should be
                        //double frequencyPerBin = 1 / 4.3d; //what works?
                        fftTimeScale = frequencyPerBin;

                        //fftResults = magResult.Take((int)(70 / frequencyPerBin)).ToArray();

                        var alphaBinMagnitude = magResult.Skip((int)(8 / frequencyPerBin)).Take((int)(4 / frequencyPerBin)).Sum();
                        var betaBinMagnitude = magResult.Skip((int)(13 / frequencyPerBin)).Take((int)(17 / frequencyPerBin)).Sum();
                        var deltaBinMagnitude = magResult.Skip((int)(1 / frequencyPerBin)).Take((int)(3 / frequencyPerBin)).Sum();
                        var gammaBinMagnitude = magResult.Skip((int)(30 / frequencyPerBin)).Take((int)(40 / frequencyPerBin)).Sum();
                        var thetaBinMagnitude = magResult.Skip((int)(4 / frequencyPerBin)).Take((int)(4 / frequencyPerBin)).Sum();

                        var binsSum = alphaBinMagnitude + betaBinMagnitude + deltaBinMagnitude + gammaBinMagnitude + thetaBinMagnitude;

                        alphaBinMagnitude /= binsSum;
                        betaBinMagnitude /= binsSum;
                        deltaBinMagnitude /= binsSum;
                        gammaBinMagnitude /= binsSum;
                        thetaBinMagnitude /= binsSum;

                        switch (channelsDataPackets.IndexOf(channelDataPackets))
                        {
                            case 0:

                                timePeriodResult.Channel1AlphaWaveMagnitude = (float)alphaBinMagnitude;
                                timePeriodResult.Channel1BetaWaveMagnitude = (float)betaBinMagnitude;
                                timePeriodResult.Channel1DeltaWaveMagnitude = (float)deltaBinMagnitude;
                                timePeriodResult.Channel1GammaWaveMagnitude = (float)gammaBinMagnitude;
                                timePeriodResult.Channel1ThetaWaveMagnitude = (float)thetaBinMagnitude;

                                break;

                            case 1:

                                timePeriodResult.Channel2AlphaWaveMagnitude = (float)alphaBinMagnitude;
                                timePeriodResult.Channel2BetaWaveMagnitude = (float)betaBinMagnitude;
                                timePeriodResult.Channel2DeltaWaveMagnitude = (float)deltaBinMagnitude;
                                timePeriodResult.Channel2GammaWaveMagnitude = (float)gammaBinMagnitude;
                                timePeriodResult.Channel2ThetaWaveMagnitude = (float)thetaBinMagnitude;

                                break;

                            case 2:

                                timePeriodResult.Channel3AlphaWaveMagnitude = (float)alphaBinMagnitude;
                                timePeriodResult.Channel3BetaWaveMagnitude = (float)betaBinMagnitude;
                                timePeriodResult.Channel3DeltaWaveMagnitude = (float)deltaBinMagnitude;
                                timePeriodResult.Channel3GammaWaveMagnitude = (float)gammaBinMagnitude;
                                timePeriodResult.Channel3ThetaWaveMagnitude = (float)thetaBinMagnitude;

                                break;

                            case 3:

                                timePeriodResult.Channel4AlphaWaveMagnitude = (float)alphaBinMagnitude;
                                timePeriodResult.Channel4BetaWaveMagnitude = (float)betaBinMagnitude;
                                timePeriodResult.Channel4DeltaWaveMagnitude = (float)deltaBinMagnitude;
                                timePeriodResult.Channel4GammaWaveMagnitude = (float)gammaBinMagnitude;
                                timePeriodResult.Channel4ThetaWaveMagnitude = (float)thetaBinMagnitude;

                                break;

                            case 4:

                                timePeriodResult.Channel5AlphaWaveMagnitude = (float)alphaBinMagnitude;
                                timePeriodResult.Channel5BetaWaveMagnitude = (float)betaBinMagnitude;
                                timePeriodResult.Channel5DeltaWaveMagnitude = (float)deltaBinMagnitude;
                                timePeriodResult.Channel5GammaWaveMagnitude = (float)gammaBinMagnitude;
                                timePeriodResult.Channel5ThetaWaveMagnitude = (float)thetaBinMagnitude;

                                break;

                            case 5:

                                timePeriodResult.Channel6AlphaWaveMagnitude = (float)alphaBinMagnitude;
                                timePeriodResult.Channel6BetaWaveMagnitude = (float)betaBinMagnitude;
                                timePeriodResult.Channel6DeltaWaveMagnitude = (float)deltaBinMagnitude;
                                timePeriodResult.Channel6GammaWaveMagnitude = (float)gammaBinMagnitude;
                                timePeriodResult.Channel6ThetaWaveMagnitude = (float)thetaBinMagnitude;

                                break;

                            case 6:

                                timePeriodResult.Channel7AlphaWaveMagnitude = (float)alphaBinMagnitude;
                                timePeriodResult.Channel7BetaWaveMagnitude = (float)betaBinMagnitude;
                                timePeriodResult.Channel7DeltaWaveMagnitude = (float)deltaBinMagnitude;
                                timePeriodResult.Channel7GammaWaveMagnitude = (float)gammaBinMagnitude;
                                timePeriodResult.Channel7ThetaWaveMagnitude = (float)thetaBinMagnitude;

                                break;

                            case 7:

                                timePeriodResult.Channel8AlphaWaveMagnitude = (float)alphaBinMagnitude;
                                timePeriodResult.Channel8BetaWaveMagnitude = (float)betaBinMagnitude;
                                timePeriodResult.Channel8DeltaWaveMagnitude = (float)deltaBinMagnitude;
                                timePeriodResult.Channel8GammaWaveMagnitude = (float)gammaBinMagnitude;
                                timePeriodResult.Channel8ThetaWaveMagnitude = (float)thetaBinMagnitude;

                                break;
                        }
                    }

                    ////scale data according to largest absolute value
                    //double largestYValue = 0;

                    //if (fftResults.Any())
                    //{
                    //    largestYValue = fftResults.ToList().Max(d => Math.Abs(d));
                    //}
                }

                binnedTransformedLabeledData.Add(timePeriodResult);

                var percentComplete = (int)(index / (double)totalTimeIntervals * 100.0);

                if (percentComplete % 10 == 0)
                {
                    this.DebugMessageClient($"{percentComplete}% complete{Environment.NewLine}");
                }
            }
        }

        return binnedTransformedLabeledData;
    }
}
