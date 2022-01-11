using System;

namespace MindGamesApi.Training.Pipelines;

public class BasePipeline : IPipeline
{
    public virtual void Run()
    {
        throw new NotImplementedException();
    }
}