using System.Collections.Generic;
using Microsoft.ML.Data;

namespace MindGamesApi.Models;

public class LabeledFlattenedFeatures
{
    public VBuffer<float> Features { get; set; }

    public string Label { get; set; }

    [NoColumn] public Dictionary<string, float> NamedFeatures { get; set; }
}
