using MsgPack.Serialization;
using Neutrino.Core.Messages;
using System;

namespace NuggetNugget2
{
    public class ChatMessage : NetworkMessage
    {
        
        public string author;
        public string message;
        public DateTime timestamp;

        [MessagePackMember(0)]
        public string Author { get; set; }

        [MessagePackMember(1)]
        public string Message { get; set; }

        [MessagePackMember(2)]
        public DateTime Timestamp { get; set; }

        public ChatMessage()
        {
            IsGuaranteed = true;
        }

        public ChatMessage(string text)
        {
            this.message = text;
            this.author = "SERVER";
            this.timestamp = DateTime.Now;
        }

        public ChatMessage(string text, string author)
        {
            this.message = text;
            this.author = author;
            this.timestamp = DateTime.Now;
        }

        public ChatMessage(string text, string author, DateTime timestamp)
        {
            this.message = text;
            this.author = author;
            this.timestamp = DateTime.Now;
        }
    }
}