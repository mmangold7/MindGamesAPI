using Microsoft.ML.Data;

namespace MindGamesApi.Training.Trainer;

public class PalmerPenguinsPrediction
{
    [ColumnName("PredictedLabel")] public string PredictedLabel { get; set; }
}
