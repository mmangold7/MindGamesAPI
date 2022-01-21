using System.Collections.Generic;
using Microsoft.ML.Data;
using MindGamesApi.Hubs;
using MindGamesApi.Models;

namespace MindGamesApi.Training.Trainer;

public interface ITrainerBase
{
    MulticlassClassificationMetrics Evaluate();

    bool Fit(DigitalSignalProcessingHub hub, List<LabeledFlattenedFeatures> labeledData, TrainingOptions trainingOptions);

    void Save(string filePath);
}
