using System.Collections.Generic;

namespace MindGamesApi.Models;

public class ChannelsDataPacket
{
    public List<double> ChannelsVolts { get; set; } = new(); //todo:consider not initializing this here

    public long TimeStamp { get; set; }
}