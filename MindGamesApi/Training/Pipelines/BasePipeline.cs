using System;

namespace MindGamesApi.Training.Pipelines;

public class BasePipeline : IPipeline
{
    public virtual string Run() => throw new NotImplementedException();
}
