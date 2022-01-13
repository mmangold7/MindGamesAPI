using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using MindGamesApi.Hubs;
using MindGamesApi.Models;

namespace MindGamesApi.Training.Trainer;

/// <summary>
///     Base class for Trainers.
///     This class exposes methods for training, evaluating and saving ML Models.
///     Classes that inherit this class need to assing concrete model and name; and to implement data pre-processing.
/// </summary>
public abstract class TrainerBase<TParameters> : ITrainerBase
    where TParameters : class
{
    protected readonly MLContext MlContext;

    protected DataOperationsCatalog.TrainTestData _dataSplit;
    protected ITrainerEstimator<MulticlassPredictionTransformer<TParameters>, TParameters> _model;
    protected ITransformer _trainedModel;

    protected static string ModelPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "classification.mdl");

    protected TrainerBase() => this.MlContext = new MLContext(111);

    public string Name { get; protected set; }

    /// <summary>
    ///     Train model on defined data.
    /// </summary>
    /// <param name="trainingFileName"></param>
    public void Fit(string trainingFileName)
    {
        if (!File.Exists(trainingFileName))
        {
            throw new FileNotFoundException($"File {trainingFileName} doesn't exist.");
        }

        this._dataSplit = this.LoadAndPrepareFileData(trainingFileName);
        var dataProcessPipeline = this.BuildDataProcessingPipeline();
        var trainingPipeline = dataProcessPipeline
                              .Append(this._model)
                              .Append(this.MlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        this._trainedModel = trainingPipeline.Fit(this._dataSplit.TrainSet);
    }

    /// <summary>
    ///     Save Model in the file.
    /// </summary>
    public void Save()
    {
        this.MlContext.Model.Save(this._trainedModel, this._dataSplit.TrainSet.Schema, ModelPath);
    }

    /// <summary>
    ///     Evaluate trained model.
    /// </summary>
    /// <returns>RegressionMetrics object which contain information about model performance.</returns>
    public MulticlassClassificationMetrics Evaluate()
    {
        var testSetTransform = this._trainedModel.Transform(this._dataSplit.TestSet);

        return this.MlContext.MulticlassClassification.Evaluate(testSetTransform);
    }

    public void Fit(DigitalSignalProcessingHub hub, List<BinnedTransformedDataPacketsResult> labeledData)
    {
        this._dataSplit = this.LoadAndPrepareInMemoryData(labeledData);

        hub.DebugMessageClient($"Split data into train and test sets{Environment.NewLine}");

        var dataProcessPipeline = this.BuildDataProcessingPipelineForEeg();

        hub.DebugMessageClient($"Featurized frequency data for ML parameters{Environment.NewLine}");

        var trainingPipeline = dataProcessPipeline
                              .Append(this._model)
                              .Append(this.MlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        this._trainedModel = trainingPipeline.Fit(this._dataSplit.TrainSet);
    }

    /// <summary>
    ///     Feature engeneering and data pre-processing.
    /// </summary>
    /// <returns>Data Processing Pipeline.</returns>
    private EstimatorChain<NormalizingTransformer> BuildDataProcessingPipeline()
    {
        var dataProcessPipeline = this.MlContext.Transforms.Conversion.MapValueToKey(inputColumnName: nameof(PalmerPenguinsData.Label), outputColumnName: "Label")
                                      .Append(this.MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Sex", outputColumnName: "SexFeaturized"))
                                      .Append(this.MlContext.Transforms.Text.FeaturizeText(inputColumnName: "Island", outputColumnName: "IslandFeaturized"))
                                      .Append(
                                           this.MlContext.Transforms.Concatenate(
                                               "Features",
                                               "IslandFeaturized",
                                               nameof(PalmerPenguinsData.CulmenLength),
                                               nameof(PalmerPenguinsData.CulmenDepth),
                                               nameof(PalmerPenguinsData.BodyMass),
                                               nameof(PalmerPenguinsData.FliperLength),
                                               "SexFeaturized"
                                           )
                                       )
                                      .Append(this.MlContext.Transforms.NormalizeMinMax("Features", "Features"))
                                      .AppendCacheCheckpoint(this.MlContext);

        return dataProcessPipeline;
    }

    /// <summary>
    ///     Feature engeneering and data pre-processing.
    /// </summary>
    /// <returns>Data Processing Pipeline.</returns>
    private EstimatorChain<NormalizingTransformer> BuildDataProcessingPipelineForEeg()
    {
        var dataProcessPipeline = this.MlContext
                                      .Transforms.Conversion.MapValueToKey(
                                           inputColumnName: nameof(BinnedTransformedDataPacketsResult.Label),
                                           outputColumnName: "Label"
                                       )
                                      .Append(
                                           this.MlContext.Transforms.Concatenate(
                                               "Features", //todo probalby don't concat all these? idk lol
                                               nameof(BinnedTransformedDataPacketsResult.Channel1AlphaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel2AlphaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel3AlphaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel4AlphaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel5AlphaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel6AlphaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel7AlphaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel8AlphaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel1BetaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel2BetaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel3BetaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel4BetaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel5BetaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel6BetaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel7BetaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel8BetaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel1GammaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel2GammaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel3GammaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel4GammaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel5GammaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel6GammaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel7GammaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel8GammaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel1DeltaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel2DeltaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel3DeltaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel4DeltaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel5DeltaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel6DeltaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel7DeltaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel8DeltaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel1ThetaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel2ThetaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel3ThetaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel4ThetaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel5ThetaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel6ThetaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel7ThetaWaveMagnitude),
                                               nameof(BinnedTransformedDataPacketsResult.Channel8ThetaWaveMagnitude)
                                           )
                                       )
                                      .Append(
                                           this.MlContext
                                               .Transforms.NormalizeMinMax("Features", "Features")
                                           //.Append(this.MlContext.Transforms.NormalizeMinMax("Channel2Volts", "Channel2Volts"))
                                           //.Append(this.MlContext.Transforms.NormalizeMinMax("Channel3Volts", "Channel3Volts"))
                                           //.Append(this.MlContext.Transforms.NormalizeMinMax("Channel4Volts", "Channel4Volts"))
                                           //.Append(this.MlContext.Transforms.NormalizeMinMax("Channel5Volts", "Channel5Volts"))
                                           //.Append(this.MlContext.Transforms.NormalizeMinMax("Channel6Volts", "Channel6Volts"))
                                           //.Append(this.MlContext.Transforms.NormalizeMinMax("Channel7Volts", "Channel7Volts"))
                                           //.Append(this.MlContext.Transforms.NormalizeMinMax("Channel8Volts", "Channel8Volts"))
                                       )
                                      .AppendCacheCheckpoint(this.MlContext);

        return dataProcessPipeline;
    }

    private DataOperationsCatalog.TrainTestData LoadAndPrepareFileData(string trainingFileName)
    {
        var trainingDataView = this.MlContext.Data.LoadFromTextFile<PalmerPenguinsData>(trainingFileName, hasHeader: true, separatorChar: ',');

        return this.MlContext.Data.TrainTestSplit(trainingDataView, 0.3);
    }

    private DataOperationsCatalog.TrainTestData LoadAndPrepareInMemoryData(
        List<BinnedTransformedDataPacketsResult> channelsDataPackets
    )
    {
        var trainingDataView2 = this.MlContext.Data.LoadFromEnumerable(channelsDataPackets);

        //var trainingDataView = this.MlContext.Data.LoadFromTextFile<PalmerPenguinsData>(trainingFileName, hasHeader: true, separatorChar: ',');

        return this.MlContext.Data.TrainTestSplit(trainingDataView2, 0.3);
    }
}
