using System;
using System.IO;
using Microsoft.ML;
using MindGamesApi.Models;

namespace MindGamesApi.Training.Trainer;

public class Predictor
{
    private readonly MLContext _mlContext;

    private ITransformer _model;

    protected string ModelName { get; set; }

    protected string ModelPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"{this.ModelName}.mdl");

    public Predictor(string modelName)
    {
        this.ModelName = modelName;
        this._mlContext = new MLContext(111);
    }

    /// <summary>
    ///     Runs prediction on new data.
    /// </summary>
    /// <param name="newSample">New data sample.</param>
    /// <returns>PalmerPenguinsData object, which contains predictions made by model.</returns>
    public PalmerPenguinsPrediction Predict(PalmerPenguinsData newSample)
    {
        this.LoadModel();

        var predictionEngine = this._mlContext.Model.CreatePredictionEngine<PalmerPenguinsData, PalmerPenguinsPrediction>(this._model);

        return predictionEngine.Predict(newSample);
    }

    public MindPrediction Predict(BinnedTransformedDataPacketsResult timePeriodData)
    {
        this.LoadModel();

        var predictionEngine = this._mlContext.Model.CreatePredictionEngine<BinnedTransformedDataPacketsResult, MindPrediction>(this._model);

        //var transformed = (List<BinnedTransformedDataPacketsResultNoLabel>)timePeriodData.Select(
        //    x => new BinnedTransformedDataPacketsResultNoLabel
        //    {
        //        Channel1AlphaWaveMagnitude = x.Channel1AlphaWaveMagnitude,
        //        Channel2AlphaWaveMagnitude = x.Channel1AlphaWaveMagnitude,
        //        Channel3AlphaWaveMagnitude = x.Channel1AlphaWaveMagnitude,
        //        Channel4AlphaWaveMagnitude = x.Channel1AlphaWaveMagnitude,
        //        Channel5AlphaWaveMagnitude = x.Channel1AlphaWaveMagnitude,
        //        Channel6AlphaWaveMagnitude = x.Channel1AlphaWaveMagnitude,
        //        Channel7AlphaWaveMagnitude = x.Channel1AlphaWaveMagnitude,
        //        Channel8AlphaWaveMagnitude = x.Channel1AlphaWaveMagnitude,
        //        Channel1BetaWaveMagnitude = x.Channel1BetaWaveMagnitude,
        //        Channel2BetaWaveMagnitude = x.Channel1BetaWaveMagnitude,
        //        Channel3BetaWaveMagnitude = x.Channel1BetaWaveMagnitude,
        //        Channel4BetaWaveMagnitude = x.Channel1BetaWaveMagnitude,
        //        Channel5BetaWaveMagnitude = x.Channel1BetaWaveMagnitude,
        //        Channel6BetaWaveMagnitude = x.Channel1BetaWaveMagnitude,
        //        Channel7BetaWaveMagnitude = x.Channel1BetaWaveMagnitude,
        //        Channel8BetaWaveMagnitude = x.Channel1BetaWaveMagnitude,
        //        Channel1GammaWaveMagnitude = x.Channel1GammaWaveMagnitude,
        //        Channel2GammaWaveMagnitude = x.Channel1GammaWaveMagnitude,
        //        Channel3GammaWaveMagnitude = x.Channel1GammaWaveMagnitude,
        //        Channel4GammaWaveMagnitude = x.Channel1GammaWaveMagnitude,
        //        Channel5GammaWaveMagnitude = x.Channel1GammaWaveMagnitude,
        //        Channel6GammaWaveMagnitude = x.Channel1GammaWaveMagnitude,
        //        Channel7GammaWaveMagnitude = x.Channel1GammaWaveMagnitude,
        //        Channel8GammaWaveMagnitude = x.Channel1GammaWaveMagnitude,
        //        Channel1DeltaWaveMagnitude = x.Channel1DeltaWaveMagnitude,
        //        Channel2DeltaWaveMagnitude = x.Channel1DeltaWaveMagnitude,
        //        Channel3DeltaWaveMagnitude = x.Channel1DeltaWaveMagnitude,
        //        Channel4DeltaWaveMagnitude = x.Channel1DeltaWaveMagnitude,
        //        Channel5DeltaWaveMagnitude = x.Channel1DeltaWaveMagnitude,
        //        Channel6DeltaWaveMagnitude = x.Channel1DeltaWaveMagnitude,
        //        Channel7DeltaWaveMagnitude = x.Channel1DeltaWaveMagnitude,
        //        Channel8DeltaWaveMagnitude = x.Channel1DeltaWaveMagnitude,
        //        Channel1ThetaWaveMagnitude = x.Channel1ThetaWaveMagnitude,
        //        Channel2ThetaWaveMagnitude = x.Channel1ThetaWaveMagnitude,
        //        Channel3ThetaWaveMagnitude = x.Channel1ThetaWaveMagnitude,
        //        Channel4ThetaWaveMagnitude = x.Channel1ThetaWaveMagnitude,
        //        Channel5ThetaWaveMagnitude = x.Channel1ThetaWaveMagnitude,
        //        Channel6ThetaWaveMagnitude = x.Channel1ThetaWaveMagnitude,
        //        Channel7ThetaWaveMagnitude = x.Channel1ThetaWaveMagnitude,
        //        Channel8ThetaWaveMagnitude = x.Channel1ThetaWaveMagnitude
        //    }
        //);

        return predictionEngine.Predict(timePeriodData);
    }

    private void LoadModel()
    {
        if (!File.Exists(this.ModelPath))
        {
            throw new FileNotFoundException($"File {this.ModelPath} doesn't exist.");
        }

        using (var stream = new FileStream(this.ModelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            this._model = this._mlContext.Model.Load(stream, out _);
        }

        if (this._model == null)
        {
            throw new Exception("Failed to load Model");
        }
    }
}
