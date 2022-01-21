using System.Collections.Generic;
using System.Threading.Tasks;
using MindGamesApi.Models;

namespace MindGamesApi.Hubs;

public interface IDigitalSignalProcessingHub
{
    void DebugMessageClient(string message);

    Task<double[][]> FastFourierTransform(List<List<ChannelDataPacket>> allChannelData);

    void TrainAndEvaluateEegClassifier(string blobbedChannelsData);

    void TrainAndEvaluatePenguinClassifier();
}
