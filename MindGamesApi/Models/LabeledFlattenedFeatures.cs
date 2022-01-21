using System.Collections.Generic;

namespace MindGamesApi.Models;

public class LabeledFlattenedFeatures
{
    public string Label { get; set; }

    public Dictionary<string, float> NamedFeatures { get; set; }
}
