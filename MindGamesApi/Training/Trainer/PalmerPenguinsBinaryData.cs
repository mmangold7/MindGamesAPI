using Microsoft.ML.Data;

namespace MindGamesApi.Training.Trainer;

/// <summary>
///     Models Palmer Penguins Binary Data.
/// </summary>
public class PalmerPenguinsBinaryData
{
    [LoadColumn(2)] public float CulmenDepth { get; set; }

    [LoadColumn(1)] public float CulmenLength { get; set; }

    [LoadColumn(0)] public bool Label { get; set; }
}
