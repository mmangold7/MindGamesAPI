using System;
using System.IO;
using Microsoft.ML;
using MindGamesApi.Models;

namespace MindGamesApi.Training.Trainer;

public class Predictor
{
    private readonly MLContext _mlContext;

    private ITransformer _model;

    protected static string ModelPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "classification.mdl");

    public Predictor() => this._mlContext = new MLContext(111);

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

    public EyesClosedPrediction Predict(ChannelsDataPacketFlattenedLabeled timePeriodData)
    {
        this.LoadModel();

        var predictionEngine = this._mlContext.Model.CreatePredictionEngine<ChannelsDataPacketFlattenedLabeled, EyesClosedPrediction>(this._model);

        return predictionEngine.Predict(timePeriodData);
    }

    private void LoadModel()
    {
        if (!File.Exists(ModelPath))
        {
            throw new FileNotFoundException($"File {ModelPath} doesn't exist.");
        }

        using (var stream = new FileStream(ModelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            this._model = this._mlContext.Model.Load(stream, out _);
        }

        if (this._model == null)
        {
            throw new Exception("Failed to load Model");
        }
    }
}
