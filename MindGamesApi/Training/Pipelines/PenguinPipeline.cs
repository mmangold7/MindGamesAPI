using System;
using System.Collections.Generic;
using System.Diagnostics;
using MindGamesApi.Training.Trainer;

namespace MindGamesApi.Training.Pipelines;

public class PenguinPipeline : IPipeline
{
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

        trainers.ForEach(t => TrainEvaluatePredict(t, newSample));
    }

    private static void TrainEvaluatePredict(ITrainerBase trainer, PalmerPenguinsData newSample)
    {
        Debug.WriteLine("*******************************");
        Debug.WriteLine($"{trainer.Name}");
        Debug.WriteLine("*******************************");

        trainer.Fit("Training\\Trainer\\penguins.csv");

        var modelMetrics = trainer.Evaluate();

        Debug.WriteLine(
            $"Macro Accuracy: {modelMetrics.MacroAccuracy:#.##}{Environment.NewLine}" +
            $"Micro Accuracy: {modelMetrics.MicroAccuracy:#.##}{Environment.NewLine}" +
            $"Log Loss: {modelMetrics.LogLoss:#.##}{Environment.NewLine}" +
            $"Log Loss Reduction: {modelMetrics.LogLossReduction:#.##}{Environment.NewLine}"
        );

        trainer.Save();

        var predictor = new Predictor();
        var prediction = predictor.Predict(newSample);
        Debug.WriteLine("------------------------------");
        Debug.WriteLine($"Prediction: {prediction.PredictedLabel:#.##}");
        Debug.WriteLine("------------------------------");
    }
}
