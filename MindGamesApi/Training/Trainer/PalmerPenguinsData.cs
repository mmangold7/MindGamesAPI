﻿using Microsoft.ML.Data;

namespace MindGamesApi.Training.Trainer;

public class PalmerPenguinsData
{
    [LoadColumn(5)] public float BodyMass { get; set; }

    [LoadColumn(3)] public float CulmenDepth { get; set; }

    [LoadColumn(2)] public float CulmenLength { get; set; }

    [LoadColumn(4)] public float FliperLength { get; set; }

    [LoadColumn(1)] public string Island { get; set; }

    [LoadColumn(0)] public string Label { get; set; }

    [LoadColumn(6)] public string Sex { get; set; }
}
