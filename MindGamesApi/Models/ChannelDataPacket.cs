using System.Text.Json.Serialization;

namespace MindGamesApi.Models
{
    public class ChannelDataPacket
    {
        [JsonIgnore]
        public long TimeStamp { get; set; }
        public double Volts { get; set; }
    }
}
