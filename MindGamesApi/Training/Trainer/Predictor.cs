using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;
using MindGamesApi.Models;

namespace MindGamesApi.Training.Trainer;

public class Predictor
{
    private readonly MLContext mlContext;

    private readonly string modelPath;

    private ITransformer model;

    //protected string ModelPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"{this.ModelName}.mdl");

    public Predictor(string modelPath)
    {
        this.modelPath = modelPath;
        this.mlContext = new MLContext(111);
    }

    public MindPrediction Predict(LabeledFlattenedFeatures timePeriodData)
    {
        this.LoadModel();

        var schemaDef = SchemaDefinition.Create(typeof(LabeledFlattenedFeatures));
        schemaDef[nameof(LabeledFlattenedFeatures.Features)].ColumnType
            = new VectorDataViewType(NumberDataViewType.Single, timePeriodData.Features.Length);

        var predictionEngine = this.mlContext.Model.CreatePredictionEngine<LabeledFlattenedFeatures, MindPrediction>(this.model, true, schemaDef);

        return predictionEngine.Predict(timePeriodData);
    }

    private void LoadModel()
    {
        if (!File.Exists(this.modelPath))
        {
            throw new FileNotFoundException($"File {this.modelPath} doesn't exist.");
        }

        using (var stream = new FileStream(this.modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            this.model = this.mlContext.Model.Load(stream, out _);
        }

        if (this.model == null)
        {
            throw new Exception("Failed to load Model");
        }
    }
}
