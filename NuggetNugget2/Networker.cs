using Neutrino.Core;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace NuggetNugget2
{
    public class Networker
    {
        const int serverPort = 20000;//51337;
        Player player;
        ChatBox chatBox;
        Node serverNode;
        List<Player> otherPlayers;

        public Networker(Player player, List<Player> otherPlayers)
        {
            this.player = player;
            this.otherPlayers = otherPlayers;

            serverNode = new Node("Sehyo", "127.0.0.1", serverPort, typeof(PlayerMessage).Assembly);
            serverNode.OnPeerConnected += peer => System.Console.WriteLine("Peer connected");
            serverNode.OnPeerDisconnected += peer => System.Console.WriteLine("Peer disconnected");
            serverNode.OnReceived += msg => HandleMessage(msg);
            serverNode.Name = "Client";

            NeutrinoConfig.PeerTimeoutMillis = 10000;

            serverNode.Start();
        }

        public void SetChatBox(ChatBox chatBox)
        {
            this.chatBox = chatBox;
        }

        public void Update()
        {
            serverNode.Update();
            var msg = serverNode.GetMessage<PlayerMessage>();
            msg.positionX = (int)player.GetPosition().X;
            msg.positionY = (int)player.GetPosition().Y;
            msg.PID = 1;
            //System.Console.WriteLine("Sending {0} and {1}", msg.positionX, msg.positionY);
            //serverNode.ConnectedPeers
            serverNode.SendToAll(msg);
            serverNode.Update();
            serverNode.Update();
        }

        public void SendChatMessage(ChatMessage chatMessage)
        {
            var msg = serverNode.GetMessage<ChatMessage>();
            msg.Author = chatMessage.author;
            msg.Message = chatMessage.message;
            msg.Timestamp = chatMessage.timestamp;

            serverNode.SendToAll(msg);
            serverNode.Update();
            serverNode.Update();
        }

        public void HandleMessage(Neutrino.Core.Messages.NetworkMessage msg)
        {
            if(msg is PlayerMessage)
            {
                //System.Console.WriteLine("We received a player message!");
                PlayerMessage pMsg = (PlayerMessage)msg;
                //System.Console.WriteLine("Player {0} is at {1}, {2}", pMsg.PID, pMsg.positionX, pMsg.positionY);
                // Does the current player exist?
                bool exists = false;
                foreach (var currentPlayer in otherPlayers)
                {
                    if(currentPlayer.PID == pMsg.PID)
                    {
                        exists = true;
                        //System.Console.WriteLine("Setting other player pos to {0} and {1}", pMsg.positionX, pMsg.positionY);
                        currentPlayer.SetPosition(pMsg.positionX, pMsg.positionY);
                        break;
                    }
                }

                if(!exists)
                {
                    System.Console.WriteLine("NEW PLAYER!?!?!?!?!?!??");
                    Player newPlayer = new Player();
                    newPlayer.setPlayerTexture(player.GetPlayerTexture());
                    newPlayer.SetPosition(pMsg.positionX, pMsg.positionY);
                    newPlayer.PID = pMsg.PID;
                    otherPlayers.Add(newPlayer);
                }
            }
            else if(msg is ChatMessage)
            {
                ChatMessage cMsg = (ChatMessage)msg;
                ChatMessage receivedChatMessage = new ChatMessage(cMsg.Message, cMsg.Author, cMsg.Timestamp);
                chatBox.HandleForeignMessage(receivedChatMessage);
            }
            else
            {
                System.Console.WriteLine("We received a {0} message.", msg.GetType());
            }
        }
    }
}