using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindGamesApi.Models
{
    public class ChannelDataPacket
    {
        public long TimeStamp { get; set; }
        public double Volts { get; set; }
    }
}
