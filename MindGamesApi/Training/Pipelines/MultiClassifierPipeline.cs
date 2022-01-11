using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MindGamesApi.Models;
using MindGamesApi.Training.Trainer;

namespace MindGamesApi.Training.Pipelines;

public class MultiClassifierPipeline : BasePipeline
{
    private readonly List<ChannelsDataPacketFlattenedLabeled> labeledData;

    public MultiClassifierPipeline(List<ChannelsDataPacketFlattenedLabeled> labeledData) => this.labeledData = labeledData;

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

        trainers.ForEach(t => TrainEvaluatePredict(t, this.labeledData));
    }

    private static void TrainEvaluatePredict(ITrainerBase trainer, List<ChannelsDataPacketFlattenedLabeled> labeledData)
    {
        Debug.WriteLine("*******************************");
        Debug.WriteLine($"{trainer.Name}");
        Debug.WriteLine("*******************************");

        trainer.Fit(labeledData);

        var modelMetrics = trainer.Evaluate();

        Debug.WriteLine(
            $"Macro Accuracy: {modelMetrics.MacroAccuracy:#.##}{Environment.NewLine}" +
            $"Micro Accuracy: {modelMetrics.MicroAccuracy:#.##}{Environment.NewLine}" +
            $"Log Loss: {modelMetrics.LogLoss:#.##}{Environment.NewLine}" +
            $"Log Loss Reduction: {modelMetrics.LogLossReduction:#.##}{Environment.NewLine}"
        );

        trainer.Save();

        var predictor = new Predictor();
        var prediction = predictor.Predict(labeledData.First());
        Debug.WriteLine("------------------------------");
        Debug.WriteLine($"Prediction: {prediction.PredictedLabel:#.##}");
        Debug.WriteLine("------------------------------");
    }
}
