using MsgPack.Serialization;
using Neutrino.Core.Messages;

namespace NuggetNugget2
{
    public class InfoMessage : NetworkMessage
    {
        [MessagePackMember(0)]
        public string name { get; set; }

        [MessagePackMember(1)]
        public int PID { get; set; }

        [MessagePackMember(2)]
        public int STATUS_CODE { get; set; }

        public InfoMessage()
        {
            IsGuaranteed = true;
        }
    }
}