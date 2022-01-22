using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML.Data;
using MindGamesApi.Models;
using MindGamesApi.Services.CWT;

namespace MindGamesApi.Helpers;

public static class DataProcessingHelper
{
    //public static List<LabeledFlattenedFeatures> PreProcessEegDataIntoClassicBins(
    //    List<ChannelsDataPacketFlattenedLabeled>[] labeledData,
    //    int timeIntervalInPackets = 256
    //)
    //{
    //    //this.Hub.DebugMessageClient($"Pre-processing recorded data sets{Environment.NewLine}");

    //    var binnedTransformedLabeledData = new List<LabeledFlattenedFeatures>();

    //    foreach (var labelGroup in labeledData)
    //    {
    //        //this.Hub.DebugMessageClient($"Processing data set \"{labelGroup.First().Label}\"{Environment.NewLine}");

    //        var tooLittleForAPacketRemainderLength = labelGroup.Count % timeIntervalInPackets;
    //        var clippedLabeledData = labelGroup.OrderBy(d => d.TimeStamp).ToList();
    //        clippedLabeledData.RemoveRange(labelGroup.Count - tooLittleForAPacketRemainderLength, tooLittleForAPacketRemainderLength);

    //        var totalTimeIntervals = clippedLabeledData.Count / timeIntervalInPackets;

    //        for (var index = 0; index < totalTimeIntervals; index++)
    //        {
    //            var timePeriodPackets = clippedLabeledData.Skip(index * timeIntervalInPackets).Take(timeIntervalInPackets).ToList();

    //            var channel1DataPackets = timePeriodPackets.Select(
    //                p => new ChannelDataPacket
    //                {
    //                    TimeStamp = p.TimeStamp,
    //                    Volts = p.Channel1Volts
    //                }
    //            );
    //            var channel2DataPackets = timePeriodPackets.Select(
    //                p => new ChannelDataPacket
    //                {
    //                    TimeStamp = p.TimeStamp,
    //                    Volts = p.Channel2Volts
    //                }
    //            );
    //            var channel3DataPackets = timePeriodPackets.Select(
    //                p => new ChannelDataPacket
    //                {
    //                    TimeStamp = p.TimeStamp,
    //                    Volts = p.Channel3Volts
    //                }
    //            );
    //            var channel4DataPackets = timePeriodPackets.Select(
    //                p => new ChannelDataPacket
    //                {
    //                    TimeStamp = p.TimeStamp,
    //                    Volts = p.Channel4Volts
    //                }
    //            );
    //            var channel5DataPackets = timePeriodPackets.Select(
    //                p => new ChannelDataPacket
    //                {
    //                    TimeStamp = p.TimeStamp,
    //                    Volts = p.Channel5Volts
    //                }
    //            );
    //            var channel6DataPackets = timePeriodPackets.Select(
    //                p => new ChannelDataPacket
    //                {
    //                    TimeStamp = p.TimeStamp,
    //                    Volts = p.Channel6Volts
    //                }
    //            );
    //            var channel7DataPackets = timePeriodPackets.Select(
    //                p => new ChannelDataPacket
    //                {
    //                    TimeStamp = p.TimeStamp,
    //                    Volts = p.Channel7Volts
    //                }
    //            );
    //            var channel8DataPackets = timePeriodPackets.Select(
    //                p => new ChannelDataPacket
    //                {
    //                    TimeStamp = p.TimeStamp,
    //                    Volts = p.Channel8Volts
    //                }
    //            );

    //            var channelsDataPackets = new List<List<ChannelDataPacket>>
    //            {
    //                channel1DataPackets.ToList(),
    //                channel2DataPackets.ToList(),
    //                channel3DataPackets.ToList(),
    //                channel4DataPackets.ToList(),
    //                channel5DataPackets.ToList(),
    //                channel6DataPackets.ToList(),
    //                channel7DataPackets.ToList(),
    //                channel8DataPackets.ToList()
    //            };

    //            var timePeriodResult = new LabeledFlattenedFeatures { Label = labelGroup.First().Label };

    //            foreach (var channelDataPackets in channelsDataPackets)
    //            {
    //                //analyze data

    //                var fftData = channelDataPackets.Select(d => d.Volts);
    //                var ffttimePeriodPackets = fftData.ToList();

    //                double[] fftResults = { };
    //                double fftTimeScale = 1;

    //                //try to remember to calculate the relative agnitude of each bin with the bins for a given channels compared to each other for normalization, nothe channels compared
    //                if (ffttimePeriodPackets.Count() >= timeIntervalInPackets)
    //                {
    //                    var timeSeries = ffttimePeriodPackets.ToArray();

    //                    var n = Convert.ToUInt32(ffttimePeriodPackets.Count());
    //                    var zeros = Convert.ToUInt32(0);
    //                    var samplingRateHz = Convert.ToDouble(250);

    //                    var selectedWindowName = "None";
    //                    var windowToApply = (DSP.Window.Type)Enum.Parse(typeof(DSP.Window.Type), selectedWindowName);

    //                    // Apply window to the time series data
    //                    var wc = DSP.Window.Coefficients(windowToApply, n);

    //                    var windowScaleFactor = DSP.Window.ScaleFactor.Signal(wc);
    //                    var windowedTimeSeries = DSP.Math.Multiply(timeSeries, wc);

    //                    var fft = new FFT();
    //                    fft.Initialize(n, zeros);

    //                    // Perform a DFT
    //                    var cpxResult = fft.Execute(windowedTimeSeries);

    //                    // Convert the complex result to a scalar magnitude 
    //                    var magResult = DSP.ConvertComplex.ToMagnitude(cpxResult);
    //                    magResult = DSP.Math.Multiply(magResult, windowScaleFactor);

    //                    var frequencyPerBin = samplingRateHz / magResult.Length; //what it should be
    //                    //double frequencyPerBin = 1 / 4.3d; //what works?
    //                    fftTimeScale = frequencyPerBin;

    //                    //fftResults = magResult.Take((int)(70 / frequencyPerBin)).ToArray();

    //                    var alphaBinMagnitude = magResult.Skip((int)(8 / frequencyPerBin)).Take((int)(4 / frequencyPerBin)).Sum();
    //                    var betaBinMagnitude = magResult.Skip((int)(13 / frequencyPerBin)).Take((int)(17 / frequencyPerBin)).Sum();
    //                    var deltaBinMagnitude = magResult.Skip((int)(1 / frequencyPerBin)).Take((int)(3 / frequencyPerBin)).Sum();
    //                    var gammaBinMagnitude = magResult.Skip((int)(30 / frequencyPerBin)).Take((int)(40 / frequencyPerBin)).Sum();
    //                    var thetaBinMagnitude = magResult.Skip((int)(4 / frequencyPerBin)).Take((int)(4 / frequencyPerBin)).Sum();

    //                    var binsSum = alphaBinMagnitude + betaBinMagnitude + deltaBinMagnitude + gammaBinMagnitude + thetaBinMagnitude;

    //                    alphaBinMagnitude /= binsSum;
    //                    betaBinMagnitude /= binsSum;
    //                    deltaBinMagnitude /= binsSum;
    //                    gammaBinMagnitude /= binsSum;
    //                    thetaBinMagnitude /= binsSum;

    //                    timePeriodResult.Features.Add((float)alphaBinMagnitude);
    //                    timePeriodResult.Features.Add((float)betaBinMagnitude);
    //                    timePeriodResult.Features.Add((float)deltaBinMagnitude);
    //                    timePeriodResult.Features.Add((float)gammaBinMagnitude);
    //                    timePeriodResult.Features.Add((float)thetaBinMagnitude);

    //                    timePeriodResult.NamedFeatures.Add(
    //                        $"Channel{channelsDataPackets.IndexOf(channelDataPackets)}{nameof(alphaBinMagnitude)}",
    //                        (float)alphaBinMagnitude
    //                    );
    //                    timePeriodResult.NamedFeatures.Add(
    //                        $"Channel{channelsDataPackets.IndexOf(channelDataPackets)}{nameof(betaBinMagnitude)}",
    //                        (float)betaBinMagnitude
    //                    );
    //                    timePeriodResult.NamedFeatures.Add(
    //                        $"Channel{channelsDataPackets.IndexOf(channelDataPackets)}{nameof(deltaBinMagnitude)}",
    //                        (float)deltaBinMagnitude
    //                    );
    //                    timePeriodResult.NamedFeatures.Add(
    //                        $"Channel{channelsDataPackets.IndexOf(channelDataPackets)}{nameof(gammaBinMagnitude)}",
    //                        (float)gammaBinMagnitude
    //                    );
    //                    timePeriodResult.NamedFeatures.Add(
    //                        $"Channel{channelsDataPackets.IndexOf(channelDataPackets)}{nameof(thetaBinMagnitude)}",
    //                        (float)thetaBinMagnitude
    //                    );
    //                }
    //            }

    //            binnedTransformedLabeledData.Add(timePeriodResult);
    //        }

    //        //this.Hub.DebugMessageClient($"Chunked data set into {timeIntervalInPackets / 250d} second intervals and performed FFT{Environment.NewLine}");
    //    }

    //    //this.Hub.DebugMessageClient(Environment.NewLine);

    //    return binnedTransformedLabeledData;
    //}

    public static List<LabeledFlattenedFeatures> PreProcessEegDataIntoCustomBins(
        List<ChannelsDataPacketFlattenedLabeled>[] labeledData,
        TrainingOptions trainingOptions
    )
    {
        var timeIntervalInPackets = trainingOptions.TimeIntervalInPackets ?? 256; //todo this is probably a stupid idea
        var transformationType = trainingOptions.TransformationType;

        //this.Hub.DebugMessageClient(
        //    $"Pre-processing recorded data sets. Chunking into sets of {timeIntervalInPackets}, FFTing, then binning into {numberOfCustomBins} bins.{Environment.NewLine}"
        //);

        var binnedTransformedLabeledData = new List<LabeledFlattenedFeatures>();

        foreach (var labelGroup in labeledData)
        {
            //this.Hub.DebugMessageClient($"Processing data set \"{labelGroup.First().Label}\"{Environment.NewLine}");

            var tooLittleForAPacketRemainderLength = labelGroup.Count % timeIntervalInPackets;
            var clippedLabeledData = labelGroup.OrderBy(d => d.TimeStamp).ToList();
            clippedLabeledData.RemoveRange(labelGroup.Count - tooLittleForAPacketRemainderLength, tooLittleForAPacketRemainderLength);
            var totalTimeIntervals = clippedLabeledData.Count / timeIntervalInPackets;

            for (var index = 0; index < totalTimeIntervals; index++)
            {
                var timePeriodPackets = clippedLabeledData.Skip(index * timeIntervalInPackets).Take(timeIntervalInPackets).ToList();

                var channelsDataPackets = UnFlattenChannelsData(timePeriodPackets);

                var timePeriodResult = new LabeledFlattenedFeatures { Label = labelGroup.First().Label };
                float[] justValues;

                switch (transformationType)
                {
                    case TransformationType.None:

                        timePeriodResult.NamedFeatures = PrepareTimeData(channelsDataPackets, trainingOptions);
                        justValues = timePeriodResult.NamedFeatures.Select(nf => nf.Value).ToArray();
                        timePeriodResult.Features = new VBuffer<float>(justValues.Length, justValues);

                        break;

                    case TransformationType.Fft:

                        timePeriodResult.NamedFeatures = FftTimePeriod(channelsDataPackets, trainingOptions);
                        justValues = timePeriodResult.NamedFeatures.Select(nf => nf.Value).ToArray();
                        timePeriodResult.Features = new VBuffer<float>(justValues.Length, justValues);

                        break;

                    case TransformationType.Cwt:

                        timePeriodResult.NamedFeatures = CwtTimePeriod(channelsDataPackets, trainingOptions);
                        justValues = timePeriodResult.NamedFeatures.Select(nf => nf.Value).ToArray();
                        timePeriodResult.Features = new VBuffer<float>(justValues.Length, justValues);

                        break;
                }

                binnedTransformedLabeledData.Add(timePeriodResult);
            }

            //this.Hub.DebugMessageClient($"Chunked data set into {timeIntervalInPackets / 250d} second intervals and performed FFT{Environment.NewLine}");
        }

        //this.Hub.DebugMessageClient(Environment.NewLine);

        return binnedTransformedLabeledData;
    }

    private static Dictionary<string, float> CwtTimePeriod(List<List<ChannelDataPacket>> channelsDataPackets, TrainingOptions options)
    {
        var timeIntervalInPackets = options.TimeIntervalInPackets ?? 256;
        var startFrequency = options.StartOfFrequencyRange ?? 0;
        var endFrequency = options.EndOfFrequencyRange ?? int.MaxValue;
        var numberOfCustomBins = options.NumberOfFrequencyBins ?? int.MaxValue;
        var useMaxBins = numberOfCustomBins == int.MaxValue;
        var useAllFrequencies = endFrequency == double.MaxValue;

        var rwt = Array.Empty<double[]>();

        foreach (var channelDataPackets in channelsDataPackets)
        {
            var inputData = channelDataPackets.Select(p => p.Volts).ToArray();
            var dt = 1.0 / 250;

            //var testCwtResults = Cwt.PerformCwt(inputData, dt, Cwt.Wavelet.Morlet, this.CwtParam1, this.CwtParam2, this.CwtParam3, this.CwtParam4);
            var testCwtResults = Cwt.PerformCwt(inputData, dt, Cwt.Wavelet.Morlet, 1, dt, .25, 32);

            var cwtData = (ComplexNumber[][])testCwtResults[0];
            rwt = MatrixOps.getRealPart(cwtData);

            var maxValue = GetMaxArrayValue(rwt);
            var minValue = GetMinArrayValue(rwt);
            var absoluteMaxValue = Math.Max(maxValue, Math.Abs(minValue));

            var scaleFactor = 255d / absoluteMaxValue;

            for (var i = 0; i < rwt.Length; i++)
            {
                for (var j = 0; j < rwt[0].Length; j++)
                {
                    rwt[i][j] *= scaleFactor;
                }
            }
        }

        var hertzPerBin = 250 / rwt.Length; //what it should be//todo check if this is accurate by using analysis view in app

        if (useMaxBins)
        {
            numberOfCustomBins = rwt.Length;
        }

        var binWidthInHertz = useAllFrequencies ? (double)rwt.Length / numberOfCustomBins : (endFrequency - startFrequency) / numberOfCustomBins;
        var arrayWidthOfBins = (int)Math.Ceiling(binWidthInHertz / hertzPerBin);

        var takenItemsCount = 0;
        var k = 0;

        var binnedFrequencyMagnitudes = new List<double[]>();

        while (takenItemsCount < rwt.Length)
        {
            var ithBinGroup =
                rwt.Skip(arrayWidthOfBins * k).Take(arrayWidthOfBins).ToArray(); //add together frequency components...

            var summedMagnitudeForIthFrequencyBin = Enumerable.Range(0, ithBinGroup[0].Length)
                                                              .Select(i => ithBinGroup.Select(nums => nums[i]).Sum())
                                                              .ToArray();

            binnedFrequencyMagnitudes.Add(summedMagnitudeForIthFrequencyBin);
            k++;
            takenItemsCount += arrayWidthOfBins;
        }

        //var rwtList = rwt.ToList();
        var timePeriodFeatures = new Dictionary<string, float>(
            binnedFrequencyMagnitudes.SelectMany(
                x => x.Select(y => new KeyValuePair<string, float>($"Freq{binnedFrequencyMagnitudes.IndexOf(x)}Time{x.ToList().IndexOf(y)}", (float)y))
            )
        );

        return timePeriodFeatures;
    }

    private static Dictionary<string, float> FftTimePeriod(List<List<ChannelDataPacket>> channelsDataPackets, TrainingOptions options)
    {
        var timeIntervalInPackets = options.TimeIntervalInPackets;
        var startFrequency = options.StartOfFrequencyRange ?? 0;
        var endFrequency = options.EndOfFrequencyRange ?? double.MaxValue;
        var numberOfCustomBins = options.NumberOfFrequencyBins ?? int.MaxValue;
        var useMaxBins = numberOfCustomBins == int.MaxValue;
        var useAllFrequencies = endFrequency == double.MaxValue;

        var timePeriodFeatures = new Dictionary<string, float>();

        foreach (var channelDataPackets in channelsDataPackets)
        {
            //analyze data

            var fftData = channelDataPackets.Select(d => d.Volts);
            var ffttimePeriodPackets = fftData.ToList();

            double[] fftResults = { };
            double fftTimeScale = 1;

            //try to remember to calculate the relative agnitude of each bin with the bins for a given channels compared to each other for normalization, nothe channels compared
            if (ffttimePeriodPackets.Count() >= timeIntervalInPackets)
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

                var hertzPerBin = samplingRateHz / magResult.Length; //what it should be//todo check if this is accurate by using analysis view in app
                //double frequencyPerBin = 1 / 4.3d; //what works?
                fftTimeScale = hertzPerBin;

                fftResults = useAllFrequencies
                                 ? magResult
                                 : magResult.Skip((int)(startFrequency / hertzPerBin))
                                            .Take((int)(endFrequency / hertzPerBin))
                                            .ToArray(); //check if magresult needs split in 2

                var tempListOfBinValues = new List<double>();

                if (useMaxBins)
                {
                    numberOfCustomBins = fftResults.Length;
                }

                var binWidthInHertz = useAllFrequencies ? (double)fftResults.Length / numberOfCustomBins : (endFrequency - startFrequency) / numberOfCustomBins;
                //var binWidthInHertz = (endFrequency - startFrequency) / numberOfCustomBins;
                double binsSum = 0;
                var arrayWidthOfBins = (int)Math.Ceiling(binWidthInHertz / hertzPerBin);

                var takenItemsCount = 0;
                var i = 0;

                while (takenItemsCount < fftResults.Length)
                {
                    var ithBinMagnitude =
                        fftResults.Skip(arrayWidthOfBins * i).Take(arrayWidthOfBins).Sum();

                    tempListOfBinValues.Insert(i, ithBinMagnitude);
                    binsSum += ithBinMagnitude;
                    i++;
                    takenItemsCount += arrayWidthOfBins;
                }

                foreach (var value in tempListOfBinValues)
                {
                    timePeriodFeatures.Add(
                        $"Channel{channelsDataPackets.IndexOf(channelDataPackets)}Bin{tempListOfBinValues.IndexOf(value)}Magnitude",
                        (float)(value / binsSum)
                    );
                }
            }
        }

        return timePeriodFeatures;
    }

    private static double GetMaxArrayValue(double[][] entriesArray)
    {
        var max = double.MinValue;

        foreach (var arr in entriesArray)
        {
            foreach (var entry in arr)
            {
                if (entry > max)
                {
                    max = entry;
                }
            }
        }

        return max;
    }

    private static double GetMinArrayValue(double[][] entriesArray)
    {
        var min = double.MaxValue;

        foreach (var arr in entriesArray)
        {
            foreach (var entry in arr)
            {
                if (entry < min)
                {
                    min = entry;
                }
            }
        }

        return min;
    }

    //probably a useless method, only included for some kind of parity with the other transforms...
    private static Dictionary<string, float> PrepareTimeData(List<List<ChannelDataPacket>> channelsDataPackets, TrainingOptions options)
    {
        var timeIntervalInPackets = options.TimeIntervalInPackets;
        var numberOfCustomBins = options.NumberOfFrequencyBins ?? int.MaxValue;
        var useMaxBins = numberOfCustomBins == int.MaxValue;

        var timePeriodFeatures = new Dictionary<string, float>();

        foreach (var channelDataPackets in channelsDataPackets)
        {
            //analyze data

            var data = channelDataPackets.Select(d => d.Volts);
            var timePeriodPackets = data.ToList();

            //try to remember to calculate the relative agnitude of each bin with the bins for a given channels compared to each other for normalization, nothe channels compared
            if (timePeriodPackets.Count() >= timeIntervalInPackets)
            {
                var timeSeries = timePeriodPackets.ToArray();

                var tempListOfBinValues = new List<double>();

                if (useMaxBins)
                {
                    numberOfCustomBins = timeSeries.Length;
                }

                double binsSum = 0;
                var arrayWidthOfBins = (int)Math.Ceiling((decimal)timeSeries.Length / numberOfCustomBins);

                var takenItemsCount = 0;
                var i = 0;

                while (takenItemsCount < timeSeries.Length)
                {
                    var ithBinMagnitude =
                        timeSeries.Skip(arrayWidthOfBins * i).Take(arrayWidthOfBins).Sum();

                    tempListOfBinValues.Insert(i, ithBinMagnitude);
                    binsSum += ithBinMagnitude;
                    i++;
                    takenItemsCount += arrayWidthOfBins;
                }

                foreach (var value in tempListOfBinValues)
                {
                    timePeriodFeatures.Add(
                        $"Channel{channelsDataPackets.IndexOf(channelDataPackets)}Bin{tempListOfBinValues.IndexOf(value)}Magnitude",
                        (float)(value / binsSum)
                    );
                }
            }
        }

        return timePeriodFeatures;
    }

    private static List<List<ChannelDataPacket>> UnFlattenChannelsData(List<ChannelsDataPacketFlattenedLabeled> timePeriodPackets)
    {
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

        return channelsDataPackets;
    }
}
