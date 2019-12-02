using Neutrino.Core;
using NuggetNugget2;
using System.Collections.Generic;
using System;


namespace NuggetNugget2Server
{
    class Program
    {

        static List<Tuple<NetworkPeer, ClientData>> clients;

        static void Main(string[] args)
        {
            System.Console.WriteLine("Starting Server");
            const int serverPort = 8080;
            int clientsConnected = 0;
            int totalConnections = 0;
            Node serverNode = new Node(serverPort, typeof(PlayerMessage).Assembly, typeof(ChatMessage).Assembly);
            serverNode.OnPeerConnected += peer =>
            {
                ++clientsConnected;
                System.Console.WriteLine("We currently have {0} connected clients.", clientsConnected);
            };

            serverNode.OnPeerDisconnected += peer =>
            {
                --clientsConnected;
                System.Console.WriteLine("We now have {0} connected clients.", clientsConnected);
            };

            serverNode.OnReceived += msg => HandleMessage(msg, serverNode);
            serverNode.Name = "Server";
            serverNode.Start();
            
            clients = new List<Tuple<NetworkPeer, ClientData>>();


            NeutrinoConfig.CreatePeer = () =>
            {
                System.Console.WriteLine("Creating Peer.");
                var newPeer = new NetworkPeer();
                var clientTuple = new Tuple<NetworkPeer, ClientData>(newPeer, new ClientData());
                
                clients.Add(clientTuple);
                int i = 0;
                foreach (var item in clients)
                {
                    System.Console.WriteLine("PID of client {0} is: {1}", i, item.Item1.GetHashCode());
                    i++;
                }
                totalConnections++;
                return newPeer;
            };


            NeutrinoConfig.PeerTimeoutMillis = 1000000;

            int elapsedMsSinceUpdate = 100;
            int msTreshold = 20;
            DateTime nextUpdate = DateTime.Now;

            while (true)
            {
                if (nextUpdate < DateTime.Now)
                {
                    
                    nextUpdate = DateTime.Now.AddMilliseconds(msTreshold);

                    UpdateClients(serverNode);
                    try
                    {
                        serverNode.SendToAll(serverNode.GetMessage<Neutrino.Core.Messages.AckMessage>());
                        serverNode.Update();
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("We crashed {0}", e.Message);
                        System.Console.WriteLine(e.StackTrace);
                        System.Console.WriteLine(e.Source);
                        System.Console.WriteLine(e.InnerException);
                    }
                }
            }
        }

        static void UpdateClients(Node serverNode)
        {
            //Neutrino.Core.Messages.AckMessage a;
            //serverNode.SendToAll(serverNode.GetMessage<Neutrino.Core.Messages.Ackmessage>());
            foreach (var targetClient in clients)
            {
                foreach (var sourceClient  in clients)
                {
                    if (targetClient == sourceClient) continue;
                    
                    var msg = serverNode.GetMessage<PlayerMessage>();

                    msg.positionX = sourceClient.Item2.positionX;
                    msg.positionY = sourceClient.Item2.positionY;
                    msg.PID = sourceClient.Item1.GetHashCode();

                    targetClient.Item1.SendNetworkMessage(msg);
                }
            }
        }

        static void HandleMessage(Neutrino.Core.Messages.NetworkMessage msg, Node serverNode)
        {
            //System.Console.WriteLine("Received Message:");
            //System.Console.WriteLine("Type is: {0}", msg.GetType());
            System.Console.WriteLine("Received {0}", msg.ToString());
            if (msg is PlayerMessage)
            {
                int x = ((PlayerMessage)msg).positionX;
                int y = ((PlayerMessage)msg).positionY;

                //System.Console.WriteLine("Received {0}, {1}", x, y);

                foreach (var tuple in clients)
                {
                    if(tuple.Item1 == msg.Source)
                    {
                        tuple.Item2.positionX = x;
                        tuple.Item2.positionY = y;
                    }
                }
                //System.Console.WriteLine("Player Position received from client is: {0}, {1}", x, y);
            }
            else if(msg is ChatMessage)
            {
                System.Console.WriteLine("Received chat message: {0}, {1}", msg.Source, msg.Id);
                ChatMessage chatMessage = (ChatMessage)msg;
                string author = chatMessage.Author;
                string message = chatMessage.Message;
                DateTime timestamp = chatMessage.Timestamp;

                var outMessage = serverNode.GetMessage<ChatMessage>();
                outMessage.Author = author;
                outMessage.Message = message;
                outMessage.Timestamp = timestamp;

                foreach (var tuple in clients)
                {
                    if (tuple.Item1 == msg.Source) continue; // Skip the sender!
                    tuple.Item1.SendNetworkMessage(outMessage);
                }
            }
            else
            {
                System.Console.WriteLine("Received other kind of message {0}", msg.ToString());
            }
        }
    }
}