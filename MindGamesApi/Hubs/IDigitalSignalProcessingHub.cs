using System.Collections.Generic;
using System.Threading.Tasks;
using MindGamesApi.Models;

namespace MindGamesApi.Hubs;

public interface IDigitalSignalProcessingHub
{
    Task<PredictionResult> ContinuousWaveletTransformMultiChannel(List<List<ChannelDataPacket>> channelsData, bool returnData);

    Task<CwtResult> ContinuousWaveletTransformSingleChannel(List<ChannelDataPacket> singleChannelData, bool recordData = false, string dataLabel = "");

    Task<CwtResult> CwtCollect(CwtInput cwtInput);

    void DebugMessageClient(string message);

    Task<double[][]> FastFourierTransform(List<List<ChannelDataPacket>> allChannelData);

    Task<PredictionResult> FFTMultiChannel(List<List<ChannelDataPacket>> channelsData);

    void TrainAndEvaluateEegClassifier(string blobbedChannelsData);

    void TrainAndEvaluatePenguinClassifier();
}
