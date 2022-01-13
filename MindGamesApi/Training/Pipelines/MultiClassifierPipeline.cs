using System;
using System.Collections.Generic;
using MindGamesApi.Hubs;
using MindGamesApi.Models;
using MindGamesApi.Training.Trainer;

namespace MindGamesApi.Training.Pipelines;

public class MultiClassifierPipeline : BasePipeline
{
    private readonly DigitalSignalProcessingHub hub;
    private readonly List<BinnedTransformedDataPacketsResult> labeledData;
    private readonly string modelName;
    private double runningHighestAccuracy;

    private string runningHighestAccuracyModel;

    public MultiClassifierPipeline(DigitalSignalProcessingHub hub, List<BinnedTransformedDataPacketsResult> labeledData, string modelName)
    {
        this.labeledData = labeledData;
        this.hub = hub;
        this.modelName = modelName;
    }

    public override string Run()
    {
        var trainers = new List<ITrainerBase>
        {
            new LbfgsMaximumEntropyTrainer(this.modelName),
            new NaiveBayesTrainer(this.modelName),
            new OneVersusAllTrainer(this.modelName),
            new SdcaMaximumEntropyTrainer(this.modelName),
            new SdcaNonCalibratedTrainer(this.modelName)
        };

        this.hub.DebugMessageClient($"Pre-processing recorded data sets{Environment.NewLine}");

        this.hub.DebugMessageClient($"Transformed time data into flattened frequency bins data{Environment.NewLine}");

        trainers.ForEach(t => this.TrainEvaluatePredict(t, this.labeledData));

        return $"{this.modelName}_{this.runningHighestAccuracyModel}";
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

        if (modelMetrics.MacroAccuracy > this.runningHighestAccuracy)
        {
            this.runningHighestAccuracy = modelMetrics.MacroAccuracy;
            this.runningHighestAccuracyModel = trainer.GetType().Name;
        }

        //var predictor = new Predictor();
        //var prediction = predictor.Predict(inputData[0].Take(256).ToList());
        //this.hub.DebugMessageClient($"------------------------------{Environment.NewLine}");
        //this.hub.DebugMessageClient($"Prediction: {prediction.PredictedLabel:#.##}{Environment.NewLine}");
        //this.hub.DebugMessageClient($"------------------------------{Environment.NewLine}");
    }
}
