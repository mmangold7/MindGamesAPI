using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using MindGamesApi.Hubs;
using MindGamesApi.Models;

namespace MindGamesApi.Training.Trainer;

public abstract class TrainerBase<TParameters> : ITrainerBase
    where TParameters : class
{
    protected readonly MLContext MlContext;

    protected DataOperationsCatalog.TrainTestData DataSplit;
    protected ITrainerEstimator<MulticlassPredictionTransformer<TParameters>, TParameters> Model;
    protected ITransformer TrainedModel;

    public string Name { get; protected set; }

    protected TrainerBase() => this.MlContext = new MLContext(111);

    public void Save(string modelPath)
    {
        this.MlContext.Model.Save(this.TrainedModel, this.DataSplit.TrainSet.Schema, modelPath);
    }

    public MulticlassClassificationMetrics Evaluate()
    {
        var testSetTransform = this.TrainedModel.Transform(this.DataSplit.TestSet);

        return this.MlContext.MulticlassClassification.Evaluate(testSetTransform);
    }

    public bool Fit(DigitalSignalProcessingHub hub, List<LabeledFlattenedFeatures> labeledData, TrainingOptions trainingOptions)
    {
        this.DataSplit = this.LoadAndPrepareInMemoryData(labeledData);

        hub.DebugMessageClient($"Split and featurized data. Beginning model training.{Environment.NewLine}");

        var dataProcessPipeline = this.BuildDataProcessingPipelineForClassicBinnedEeg(trainingOptions);

        var trainingPipeline = dataProcessPipeline
                              .Append(this.Model)
                              .Append(this.MlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        try
        {
            this.TrainedModel = trainingPipeline.Fit(this.DataSplit.TrainSet);

            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    private IEstimator<ITransformer> BuildDataProcessingPipelineForClassicBinnedEeg(TrainingOptions trainingOptions)
    {
        var dataProcessPipeline = this.MlContext
                                      .Transforms.Conversion.MapValueToKey(
                                           inputColumnName: nameof(LabeledFlattenedFeatures.Label),
                                           outputColumnName: "Label"
                                       );

        var togetherChain = dataProcessPipeline
                           .Append(
                                this.MlContext
                                    .Transforms.NormalizeMinMax("Features", "Features")
                            )
                           .AppendCacheCheckpoint(this.MlContext);

        IEstimator<ITransformer> result = togetherChain;

        return result;
    }

    private IEstimator<ITransformer> BuildDataProcessingPipelineForCustomBinnedEeg(TrainingOptions trainingOptions, ExpandoObject labeledData)
    {
        var dataProcessPipeline = this.MlContext
                                      .Transforms.Conversion.MapValueToKey(
                                           inputColumnName: "Label",
                                           outputColumnName: "Label"
                                       );

        var labeledDataList = labeledData.ToList();
        labeledDataList.RemoveAll(d => d.Key == "Label");
        var expandoFeaturePropertiesAsArgs = labeledDataList.Select(d => d.Key).ToArray();

        var togetherChain = dataProcessPipeline
                           .Append(
                                this.MlContext.Transforms.Concatenate(
                                    "Features",
                                    expandoFeaturePropertiesAsArgs
                                )
                            )
                           .Append(
                                this.MlContext
                                    .Transforms.NormalizeMinMax("Features", "Features")
                            )
                           .AppendCacheCheckpoint(this.MlContext);

        IEstimator<ITransformer> result = togetherChain;

        return result;
    }

    private DataOperationsCatalog.TrainTestData LoadAndPrepareInMemoryData(
        List<LabeledFlattenedFeatures> channelsDataPackets
    )
    {
        var trainingDataView2 = this.MlContext.Data.LoadFromEnumerable(channelsDataPackets);

        return this.MlContext.Data.TrainTestSplit(trainingDataView2, 0.3);
    }
}
