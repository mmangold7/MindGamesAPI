using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNet.SignalR;
using MindGamesApi.Hubs;
using MindGamesApi.Models;
using MindGamesApi.Training.Trainer;

namespace MindGamesApi.Training.Pipelines;

public class MultiClassifierPipeline : BasePipeline
{
    private readonly List<ChannelsDataPacketFlattenedLabeled> labeledData;
    private DigitalSignalProcessingHub hub;

    public MultiClassifierPipeline(List<ChannelsDataPacketFlattenedLabeled> labeledData, DigitalSignalProcessingHub hub)
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

        trainers.ForEach(t => TrainEvaluatePredict(t, this.labeledData));
    }

    private void TrainEvaluatePredict(ITrainerBase trainer, List<ChannelsDataPacketFlattenedLabeled> labeledData)
    {
        this.hub.DebugMessageClient("*******************************");
        this.hub.DebugMessageClient($"{trainer.Name}");
        this.hub.DebugMessageClient("*******************************");

        trainer.Fit(labeledData);

        var modelMetrics = trainer.Evaluate();

        this.hub.DebugMessageClient(
            $"Macro Accuracy: {modelMetrics.MacroAccuracy:#.##}{Environment.NewLine}" +
            $"Micro Accuracy: {modelMetrics.MicroAccuracy:#.##}{Environment.NewLine}" +
            $"Log Loss: {modelMetrics.LogLoss:#.##}{Environment.NewLine}" +
            $"Log Loss Reduction: {modelMetrics.LogLossReduction:#.##}{Environment.NewLine}"
        );

        trainer.Save();

        var predictor = new Predictor();
        var prediction = predictor.Predict(labeledData.First());
        this.hub.DebugMessageClient("------------------------------");
        this.hub.DebugMessageClient($"Prediction: {prediction.PredictedLabel:#.##}");
        this.hub.DebugMessageClient("------------------------------");
    }
}
