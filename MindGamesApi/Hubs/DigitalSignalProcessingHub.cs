using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MindGamesApi.Models;
using MindGamesApi.Services;

namespace MindGamesApi.Hubs;

public class DigitalSignalProcessingHub : Hub
{
    private readonly ModelTrainingService modelTrainingService;

    public DigitalSignalProcessingHub(ModelTrainingService modelTrainingService)
    {
        modelTrainingService.Hub = this;
        this.modelTrainingService = modelTrainingService;
    }

    public void DebugMessageClient(string message) //todo:overload this to have a debug writeline that does the newline for you
    {
        var client = this.Clients.Client(this.Context.ConnectionId).SendAsync("PipelineDebugMessage", message);
        Debug.Write(message);
    }

    public Task<string> PredictEegClassUsingMultiClassifier(List<ChannelsDataPacketFlattenedLabeled> channelsData, string modelPath, TrainingOptions options) =>
        this.modelTrainingService.PredictClass(channelsData, modelPath, options);

    public Task<List<MultiClassifierModel>> TrainEegMultiClassifiers(string blobbedChannelsData, string modelId, TrainingOptions options) =>
        this.modelTrainingService.TrainEegMultiClassifiers(blobbedChannelsData, modelId, options);
}
