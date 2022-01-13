using System.Collections.Generic;
using Microsoft.ML.Data;
using MindGamesApi.Hubs;
using MindGamesApi.Models;

namespace MindGamesApi.Training.Trainer;

public interface ITrainerBase
{
    string Name { get; }

    MulticlassClassificationMetrics Evaluate();

    void Fit(string trainingFileName);

    void Fit(DigitalSignalProcessingHub hub, List<BinnedTransformedDataPacketsResult> labeledData);

    void Save();
}
