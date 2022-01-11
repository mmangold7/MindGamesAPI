using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MindGamesApi.Models;
using MindGamesApi.Training.Trainer;

namespace MindGamesApi.Training.Pipelines;

public class EyesClosedClassifierPipeline : BasePipeline
{
    private readonly List<ChannelsDataPacket> eyesClosedData;
    private readonly List<ChannelsDataPacket> eyesOpenData;

    public EyesClosedClassifierPipeline(List<ChannelsDataPacket> eyesClosedData, List<ChannelsDataPacket> eyesOpenData)
    {
        this.eyesClosedData = eyesClosedData;
        this.eyesOpenData = eyesOpenData;
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

        trainers.ForEach(t => TrainEvaluatePredict(t, this.eyesClosedData, this.eyesOpenData));
    }

    private static void TrainEvaluatePredict(ITrainerBase trainer, List<ChannelsDataPacket> eyesOpenData, List<ChannelsDataPacket> eyesClosedData)
    {
        Debug.WriteLine("*******************************");
        Debug.WriteLine($"{trainer.Name}");
        Debug.WriteLine("*******************************");

        trainer.Fit(eyesOpenData, eyesClosedData);

        var modelMetrics = trainer.Evaluate();

        Debug.WriteLine(
            $"Macro Accuracy: {modelMetrics.MacroAccuracy:#.##}{Environment.NewLine}" +
            $"Micro Accuracy: {modelMetrics.MicroAccuracy:#.##}{Environment.NewLine}" +
            $"Log Loss: {modelMetrics.LogLoss:#.##}{Environment.NewLine}" +
            $"Log Loss Reduction: {modelMetrics.LogLossReduction:#.##}{Environment.NewLine}"
        );

        trainer.Save();

        var predictor = new Predictor();
        //var prediction = predictor.Predict(eyesClosedData.Take(250).ToList());
        var prediction = predictor.Predict(
            eyesClosedData.Select(
                               x => new ChannelsDataPacketFlattenedLabeled
                               {
                                   TimeStamp = x.TimeStamp,
                                   Channel1Volts = (float)x.ChannelsVolts[0],
                                   Channel2Volts = (float)x.ChannelsVolts[1],
                                   Channel3Volts = (float)x.ChannelsVolts[2],
                                   Channel4Volts = (float)x.ChannelsVolts[3],
                                   Channel5Volts = (float)x.ChannelsVolts[4],
                                   Channel6Volts = (float)x.ChannelsVolts[5],
                                   Channel7Volts = (float)x.ChannelsVolts[6],
                                   Channel8Volts = (float)x.ChannelsVolts[7],
                                   Condition = 1
                               }
                           )
                          .First()
        );
        Debug.WriteLine("------------------------------");
        Debug.WriteLine($"Prediction: {prediction.PredictedLabel:#.##}");
        Debug.WriteLine("------------------------------");
    }
}