using MsgPack.Serialization;
using Neutrino.Core.Messages;

namespace NuggetNugget2
{
    public class PlayerMessage : NetworkMessage
    {
        [MessagePackMember(0)]
        public int positionX { get; set; }

        [MessagePackMember(1)]
        public int positionY { get; set; }

        [MessagePackMember(2)]
        public int PID { get; set; }
    }
}