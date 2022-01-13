using System;
using System.Collections.Generic;
using System.Linq;
using MindGamesApi.Hubs;
using MindGamesApi.Models;
using MindGamesApi.Training.Trainer;

namespace MindGamesApi.Training.Pipelines;

public class MultiClassifierPipeline : BasePipeline
{
    private readonly DigitalSignalProcessingHub hub;
    private readonly List<ChannelsDataPacketFlattenedLabeled>[] labeledData;

    public MultiClassifierPipeline(DigitalSignalProcessingHub hub, List<ChannelsDataPacketFlattenedLabeled>[] labeledData)
    {
        this.labeledData = labeledData;
        this.hub = hub;
    }

    public override void Run()
    {
        var trainers = new List<ITrainerBase>
        {
            new LbfgsMaximumEntropyTrainer(),
            new NaiveBayesTrainer(),
            new OneVersusAllTrainer(),
            new SdcaMaximumEntropyTrainer(),
            new SdcaNonCalibratedTrainer()
        };

        //var binndedFlattenedLabeledData = ;

        this.hub.DebugMessageClient($"Pre-processing recorded data sets{Environment.NewLine}");

        var binnedTransformedLabeledData = new List<BinnedTransformedDataPacketsResult>();

        foreach (var labelGroup in this.labeledData)
        {
            this.hub.DebugMessageClient($"Processing data set \"{labelGroup.First().Label}\"{Environment.NewLine}");

            var tooLittleForAPacketRemainderLength = labelGroup.Count % 256;
            var clippedLabeledData = labelGroup;
            clippedLabeledData.RemoveRange(labelGroup.Count - tooLittleForAPacketRemainderLength, tooLittleForAPacketRemainderLength);

            var totalTimeIntervals = clippedLabeledData.Count / 256;

            this.hub.DebugMessageClient($"Chunked data into 1 second intervals{Environment.NewLine}");
            this.hub.DebugMessageClient($"Beginning to transform time data{Environment.NewLine}");

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

                        var alphaBinMagnitude = magResult.Skip((int)(8 / frequencyPerBin)).Take((int)(12 / frequencyPerBin)).Sum();
                        var betaBinMagnitude = magResult.Skip((int)(8 / frequencyPerBin)).Take((int)(12 / frequencyPerBin)).Sum();
                        var deltaBinMagnitude = magResult.Skip((int)(8 / frequencyPerBin)).Take((int)(12 / frequencyPerBin)).Sum();
                        var gammaBinMagnitude = magResult.Skip((int)(8 / frequencyPerBin)).Take((int)(12 / frequencyPerBin)).Sum();
                        var thetaBinMagnitude = magResult.Skip((int)(8 / frequencyPerBin)).Take((int)(12 / frequencyPerBin)).Sum();

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
                    this.hub.DebugMessageClient($"{percentComplete}% complete{Environment.NewLine}");
                }
            }
        }

        //split by label!! you have to group by the label and foreach when you add so you can add the rigth label...

        this.hub.DebugMessageClient($"Transformed time data into flattened frequency bins data{Environment.NewLine}");

        trainers.ForEach(t => this.TrainEvaluatePredict(t, binnedTransformedLabeledData));
    }

    private void TrainEvaluatePredict(ITrainerBase trainer, List<BinnedTransformedDataPacketsResult> inputData)
    {
        this.hub.DebugMessageClient($"*******************************{Environment.NewLine}");
        this.hub.DebugMessageClient($"{trainer.Name}{Environment.NewLine}");
        this.hub.DebugMessageClient($"*******************************{Environment.NewLine}");

        trainer.Fit(this.hub, inputData);

        this.hub.DebugMessageClient($"Completed model training. Beginning model evaluation.{Environment.NewLine}");

        var modelMetrics = trainer.Evaluate();

        this.hub.DebugMessageClient(
            $"Macro Accuracy: {modelMetrics.MacroAccuracy:#.##}{Environment.NewLine}" +
            $"Micro Accuracy: {modelMetrics.MicroAccuracy:#.##}{Environment.NewLine}" +
            $"Log Loss: {modelMetrics.LogLoss:#.##}{Environment.NewLine}" +
            $"Log Loss Reduction: {modelMetrics.LogLossReduction:#.##}{Environment.NewLine}"
        );

        trainer.Save();

        //var predictor = new Predictor();
        //var prediction = predictor.Predict(inputData[0].Take(256).ToList());
        //this.hub.DebugMessageClient($"------------------------------{Environment.NewLine}");
        //this.hub.DebugMessageClient($"Prediction: {prediction.PredictedLabel:#.##}{Environment.NewLine}");
        //this.hub.DebugMessageClient($"------------------------------{Environment.NewLine}");
    }
}
