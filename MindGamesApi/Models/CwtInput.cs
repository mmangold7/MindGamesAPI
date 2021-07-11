using System.Collections.Generic;

namespace MindGamesApi.Models
{
    public class CwtInput
    {
        public string DataLabel { get; set; }
        public List<ChannelDataPacket> Channel1Data { get; set; }
        public List<ChannelDataPacket> Channel2Data { get; set; }
        public List<ChannelDataPacket> Channel3Data { get; set; }
        public List<ChannelDataPacket> Channel4Data { get; set; }
        public List<ChannelDataPacket> Channel5Data { get; set; }
        public List<ChannelDataPacket> Channel6Data { get; set; }
        public List<ChannelDataPacket> Channel7Data { get; set; }
        public List<ChannelDataPacket> Channel8Data { get; set; }
        public int CwtChannelIndex { get; set; }
    }
}
