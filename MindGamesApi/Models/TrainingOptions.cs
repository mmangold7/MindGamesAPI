namespace MindGamesApi.Models;

public class TrainingOptions
{
    public double? EndOfFrequencyRange { get; set; }

    public bool HyperParametrize { get; set; }

    public int? NumberOfFrequencyBins { get; set; }

    public double? StartOfFrequencyRange { get; set; }

    public int? TimeIntervalInPackets { get; set; }

    public TransformationType? TransformationType { get; set; }
}
