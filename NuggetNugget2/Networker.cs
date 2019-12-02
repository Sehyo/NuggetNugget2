using Neutrino.Core;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace NuggetNugget2
{
    public class Networker
    {
        const int serverPort = 8080;//51337;
        Player player;
        ChatBox chatBox;
        Node serverNode;
        List<Player> otherPlayers;

        bool waitingForLogin = false;
        

        public Networker(Player player, List<Player> otherPlayers)
        {
            this.player = player;
            this.otherPlayers = otherPlayers;
            // 110.35.180.67

            string ip = "110.35.180.67";
            System.Console.WriteLine("What server? (Enter number)");
            System.Console.WriteLine("1: Alex' Server");
            System.Console.WriteLine("2: Localhost");
            if (System.Console.ReadLine().Contains("2")) ip = "127.0.0.1";
            serverNode = new Node("Sehyo", ip, serverPort, typeof(PlayerMessage).Assembly, typeof(ChatMessage).Assembly, typeof(InfoMessage).Assembly);
            serverNode.OnPeerConnected += peer => System.Console.WriteLine("Peer connected");
            serverNode.OnPeerDisconnected += peer => System.Console.WriteLine("Peer disconnected");
            serverNode.OnReceived += msg => HandleMessage(msg);
            serverNode.Name = "Client";

            NeutrinoConfig.PeerTimeoutMillis = 10000;

            serverNode.Start();

            // Temporarily commented out so I can push.

            //string username = "test";
            //var infoMsg = serverNode.GetMessage<InfoMessage>();
            //msg.name = username;
            //serverNode.SendToAll(msg);
            //System.Console.WriteLine("Sending login request.");
            //serverNode.Update();
        }

        public void SetChatBox(ChatBox chatBox)
        {
            this.chatBox = chatBox;
        }

        public void Update()
        {
            if (waitingForLogin) return;
            //serverNode.Update();
            var msg = serverNode.GetMessage<PlayerMessage>();
            msg.positionX = (int)player.GetPosition().X;
            msg.positionY = (int)player.GetPosition().Y;
            msg.PID = 1;
            //System.Console.WriteLine("Sending {0} and {1}", msg.positionX, msg.positionY);
            //serverNode.ConnectedPeers
            System.Console.WriteLine("Sending pos");
            serverNode.SendToAll(msg);
            serverNode.Update();
            //serverNode.Update();
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
                if (waitingForLogin) return;
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
                    newPlayer.objectTexture = player.objectTexture;
                    newPlayer.SetPosition(pMsg.positionX, pMsg.positionY);
                    newPlayer.PID = pMsg.PID;
                    otherPlayers.Add(newPlayer);
                }

            }
            else if(msg is ChatMessage)
            {
                if (waitingForLogin) return;
                ChatMessage cMsg = (ChatMessage)msg;
                ChatMessage receivedChatMessage = new ChatMessage(cMsg.Message, cMsg.Author, cMsg.Timestamp);
                System.Console.WriteLine("Received Chat Message: {0}, {1}", msg.Source, msg.Id);
                chatBox.HandleForeignMessage(receivedChatMessage);
            }
            else if(msg is InfoMessage)
            {
                if(waitingForLogin)
                {

                }
                else
                {
                    // ??
                }
            }
            else
            {
                System.Console.WriteLine("We received a {0} message.", msg.GetType());
            }
        }
    }
}