using Microsoft.ML.Data;

namespace MindGamesApi.Training.Trainer;

public class MindPrediction
{
    [ColumnName("PredictedLabel")] public string PredictedLabel { get; set; }
}
