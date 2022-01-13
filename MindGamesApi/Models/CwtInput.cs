using System;

namespace MindGamesApi.Models;

public class TrainedModel
{
    public string Description { get; set; }

    public string Name { get; set; }

    public string TrainedModelFileName { get; set; }

    public Guid TrainedModelId { get; set; }
}
