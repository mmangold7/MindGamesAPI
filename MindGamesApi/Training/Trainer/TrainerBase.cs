using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
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

    public void Fit(List<ChannelsDataPacket> conditionTrueData, List<ChannelsDataPacket> conditionFalseData)
    {
        this._dataSplit = this.LoadAndPrepareInMemoryData(conditionTrueData, conditionFalseData);
        var dataProcessPipeline = this.BuildDataProcessingPipelineForEeg();
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
                                           inputColumnName: nameof(ChannelsDataPacketFlattenedLabeled.Condition),
                                           outputColumnName: "Label"
                                       )
                                      .Append(
                                           this.MlContext.Transforms.Concatenate(
                                               "Features",
                                               nameof(ChannelsDataPacketFlattenedLabeled.Channel1Volts),
                                               nameof(ChannelsDataPacketFlattenedLabeled.Channel2Volts),
                                               nameof(ChannelsDataPacketFlattenedLabeled.Channel3Volts),
                                               nameof(ChannelsDataPacketFlattenedLabeled.Channel4Volts),
                                               nameof(ChannelsDataPacketFlattenedLabeled.Channel5Volts),
                                               nameof(ChannelsDataPacketFlattenedLabeled.Channel6Volts),
                                               nameof(ChannelsDataPacketFlattenedLabeled.Channel7Volts),
                                               nameof(ChannelsDataPacketFlattenedLabeled.Channel8Volts)
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
        List<ChannelsDataPacket> channelsDataPacketsTrue,
        List<ChannelsDataPacket> channelsDataPacketsFalse
    )
    {
        var flattnedChannelsDataPackets =
            new List<ChannelsDataPacketFlattenedLabeled>(); //todo:consider savings the data in this format to prevent this overhead processing

        foreach (var channelsDataPacket in channelsDataPacketsTrue)
        {
            flattnedChannelsDataPackets.Add(
                new ChannelsDataPacketFlattenedLabeled
                {
                    TimeStamp = channelsDataPacket.TimeStamp,
                    Channel1Volts = (float)channelsDataPacket.ChannelsVolts[0],
                    Channel2Volts = (float)channelsDataPacket.ChannelsVolts[1],
                    Channel3Volts = (float)channelsDataPacket.ChannelsVolts[2],
                    Channel4Volts = (float)channelsDataPacket.ChannelsVolts[3],
                    Channel5Volts = (float)channelsDataPacket.ChannelsVolts[4],
                    Channel6Volts = (float)channelsDataPacket.ChannelsVolts[5],
                    Channel7Volts = (float)channelsDataPacket.ChannelsVolts[6],
                    Channel8Volts = (float)channelsDataPacket.ChannelsVolts[7],
                    Condition = 1
                }
            );
        }

        foreach (var channelsDataPacket in channelsDataPacketsFalse)
        {
            flattnedChannelsDataPackets.Add(
                new ChannelsDataPacketFlattenedLabeled
                {
                    TimeStamp = channelsDataPacket.TimeStamp,
                    Channel1Volts = (float)channelsDataPacket.ChannelsVolts[0],
                    Channel2Volts = (float)channelsDataPacket.ChannelsVolts[1],
                    Channel3Volts = (float)channelsDataPacket.ChannelsVolts[2],
                    Channel4Volts = (float)channelsDataPacket.ChannelsVolts[3],
                    Channel5Volts = (float)channelsDataPacket.ChannelsVolts[4],
                    Channel6Volts = (float)channelsDataPacket.ChannelsVolts[5],
                    Channel7Volts = (float)channelsDataPacket.ChannelsVolts[6],
                    Channel8Volts = (float)channelsDataPacket.ChannelsVolts[7],
                    Condition = 0
                }
            );
        }

        var trainingDataView2 = this.MlContext.Data.LoadFromEnumerable(flattnedChannelsDataPackets);

        //var trainingDataView = this.MlContext.Data.LoadFromTextFile<PalmerPenguinsData>(trainingFileName, hasHeader: true, separatorChar: ',');

        return this.MlContext.Data.TrainTestSplit(trainingDataView2, 0.3);
    }
}
