using System;

namespace MindGamesApi.Models;

public class MultiClassifierModel : BaseEntity
{
    public string FilePath { get; set; }

    public double LogLoss { get; set; }

    public double LogLossReduction { get; set; }

    public double MacroAccuracy { get; set; }

    public double MicroAccuracy { get; set; }

    public string ModelName { get; set; }

    public TrainingOptions TrainingOptions { get; set; }

    public MultiClassifierModel(Guid id) : base(id) { }
}
