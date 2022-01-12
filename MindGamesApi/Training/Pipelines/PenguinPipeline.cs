using System;
using System.Collections.Generic;
using MindGamesApi.Hubs;
using MindGamesApi.Training.Trainer;

namespace MindGamesApi.Training.Pipelines;

public class PenguinPipeline : IPipeline
{
    private readonly DigitalSignalProcessingHub hub;

    public PenguinPipeline(DigitalSignalProcessingHub hub) => this.hub = hub;

    public void Run()
    {
        var newSample = new PalmerPenguinsData
        {
            Island = "Torgersen",
            CulmenDepth = 18.7f,
            CulmenLength = 39.3f,
            FliperLength = 180,
            BodyMass = 3700,
            Sex = "MALE"
        };

        var trainers = new List<ITrainerBase>
        {
            new LbfgsMaximumEntropyTrainer(),
            new NaiveBayesTrainer(),
            new OneVersusAllTrainer(),
            new SdcaMaximumEntropyTrainer(),
            new SdcaNonCalibratedTrainer()
        };

        trainers.ForEach(t => this.TrainEvaluatePredict(t, newSample));
    }

    private void TrainEvaluatePredict(ITrainerBase trainer, PalmerPenguinsData newSample)
    {
        this.hub.DebugMessageClient("*******************************");
        this.hub.DebugMessageClient($"{trainer.Name}");
        this.hub.DebugMessageClient("*******************************");

        trainer.Fit("Training\\Trainer\\penguins.csv");

        var modelMetrics = trainer.Evaluate();

        this.hub.DebugMessageClient(
            $"Macro Accuracy: {modelMetrics.MacroAccuracy:#.##}{Environment.NewLine}" +
            $"Micro Accuracy: {modelMetrics.MicroAccuracy:#.##}{Environment.NewLine}" +
            $"Log Loss: {modelMetrics.LogLoss:#.##}{Environment.NewLine}" +
            $"Log Loss Reduction: {modelMetrics.LogLossReduction:#.##}{Environment.NewLine}"
        );

        trainer.Save();

        var predictor = new Predictor();
        var prediction = predictor.Predict(newSample);
        this.hub.DebugMessageClient("------------------------------");
        this.hub.DebugMessageClient($"Prediction: {prediction.PredictedLabel:#.##}");
        this.hub.DebugMessageClient("------------------------------");
    }
}
