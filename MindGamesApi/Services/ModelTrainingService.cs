using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MindGamesApi.Helpers;
using MindGamesApi.Hubs;
using MindGamesApi.Models;
using MindGamesApi.Training.Trainer;
using Newtonsoft.Json;

namespace MindGamesApi.Services;

public class ModelTrainingService
{
    public DigitalSignalProcessingHub Hub { get; set; }

    public Task<string> PredictClass(List<ChannelsDataPacketFlattenedLabeled> channelsData, string modelName, TrainingOptions options)
    {
        var dataGroupedByLabel = channelsData.GroupBy(cd => cd.Label).Select(g => g.ToList()).ToArray();

        var predictor = new Predictor(modelName);
        var wrappedBins = DataProcessingHelper.PreProcessEegDataIntoCustomBins(dataGroupedByLabel, options);
        var prediction = predictor.Predict(wrappedBins[0]);

        Debug.WriteLine($"EEG class prediction: {prediction.PredictedLabel}");

        return Task.FromResult(prediction.PredictedLabel);
    }

    public Task<List<MultiClassifierModel>> TrainEegMultiClassifiers(string blobbedChannelsData, string sharedModelId, TrainingOptions options)
    {
        var newlyTrainedMultiClassifierModels = new List<MultiClassifierModel>();

        if (options.HyperParametrize)
        {
            //double nextTimeIntervalInPacketsRootBaseTwo = 5;
            double nextTimeIntervalInPacketsRootBaseTwo = 8;

            //while (nextTimeIntervalInPacketsRootBaseTwo <= 10)
            while (nextTimeIntervalInPacketsRootBaseTwo <= 10)
            {
                var timeIntervalInPackets = (int)Math.Pow(2.0, nextTimeIntervalInPacketsRootBaseTwo);

                var nextOptions = new TrainingOptions
                {
                    TimeIntervalInPackets = timeIntervalInPackets,
                    TransformationType = TransformationType.Fft
                };

                var channelsData = JsonConvert.DeserializeObject<List<ChannelsDataPacketFlattenedLabeled>>(blobbedChannelsData);
                var dataGroupedByLabel = channelsData.GroupBy(cd => cd.Label).Select(g => g.ToList()).ToArray();

                //var dataBinned = this.PreProcessEegDataIntoClassicBins(dataGroupedByLabel, nextOptions);
                var dataBinned = DataProcessingHelper.PreProcessEegDataIntoCustomBins(dataGroupedByLabel, nextOptions);

                var trainers = new List<ITrainerBase>
                {
                    new LbfgsMaximumEntropyTrainer(),
                    new NaiveBayesTrainer(),
                    new OneVersusAllTrainer(),
                    new SdcaMaximumEntropyTrainer()
                };

                newlyTrainedMultiClassifierModels.AddRange(trainers.Select(trainer => this.TrainSingleModel(trainer, sharedModelId, dataBinned, nextOptions)));

                nextTimeIntervalInPacketsRootBaseTwo++;
            }
        }
        else
        {
            var channelsData = JsonConvert.DeserializeObject<List<ChannelsDataPacketFlattenedLabeled>>(blobbedChannelsData);
            var dataGroupedByLabel = channelsData.GroupBy(cd => cd.Label).Select(g => g.ToList()).ToArray();

            //var dataBinned = this.PreProcessEegDataIntoClassicBins(dataGroupedByLabel, nextOptions);
            var dataBinned = DataProcessingHelper.PreProcessEegDataIntoCustomBins(dataGroupedByLabel, options);

            var trainers = new List<ITrainerBase>
            {
                new LbfgsMaximumEntropyTrainer(),
                new NaiveBayesTrainer(),
                new OneVersusAllTrainer(),
                new SdcaMaximumEntropyTrainer()
            };

            newlyTrainedMultiClassifierModels.AddRange(trainers.Select(trainer => this.TrainSingleModel(trainer, sharedModelId, dataBinned, options)));
        }

        this.Hub.DebugMessageClient($"Hyper-parametrized models:{Environment.NewLine}");

        foreach (var newlyTrainedMultiClassifierModel in newlyTrainedMultiClassifierModels)
        {
            this.Hub.DebugMessageClient(
                $"Macro Accuracy: {newlyTrainedMultiClassifierModel.MacroAccuracy} Micro Accuracy: {newlyTrainedMultiClassifierModel.MicroAccuracy} LogLoss: {newlyTrainedMultiClassifierModel.LogLoss} LogLossReduction: {newlyTrainedMultiClassifierModel.LogLossReduction} Name: {newlyTrainedMultiClassifierModel.ModelName}{Environment.NewLine}"
            );
        }

        return Task.FromResult(newlyTrainedMultiClassifierModels);
    }

    private MultiClassifierModel TrainSingleModel(ITrainerBase trainer, string modelId, List<LabeledFlattenedFeatures> inputData, TrainingOptions options)
    {
        this.Hub.DebugMessageClient($"Trainer: {nameof(trainer)}{Environment.NewLine}");
        trainer.Fit(this.Hub, inputData, options);
        this.Hub.DebugMessageClient($"Completed model training. Beginning evaluation.{Environment.NewLine}");
        var modelMetrics = trainer.Evaluate();

        this.Hub.DebugMessageClient(
            $"Macro Accuracy: {modelMetrics.MacroAccuracy:#.##} " +
            $"Micro Accuracy: {modelMetrics.MicroAccuracy:#.##} " +
            $"Log Loss: {modelMetrics.LogLoss:#.##} " +
            $"Log Loss Reduction: {modelMetrics.LogLossReduction:#.##}{Environment.NewLine}{Environment.NewLine}"
        );

        var uniqueModelName =
            $"{modelId}_{nameof(trainer)}_{options.TimeIntervalInPackets}_{options.NumberOfFrequencyBins}_{options.StartOfFrequencyRange}_{options.EndOfFrequencyRange}";
        var modelPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"{uniqueModelName}.mdl");
        trainer.Save(modelPath);

        return new MultiClassifierModel
        {
            ModelName = uniqueModelName,
            FilePath = modelPath,
            MacroAccuracy = modelMetrics.MacroAccuracy,
            MicroAccuracy = modelMetrics.MacroAccuracy,
            LogLoss = modelMetrics.LogLoss,
            LogLossReduction = modelMetrics.LogLossReduction,
            TrainingOptions = options
        };
    }
}
