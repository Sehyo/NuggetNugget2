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
            const int serverPort = 20000;
            Node serverNode = new Node(serverPort, typeof(PlayerMessage).Assembly);
            serverNode.OnPeerConnected += peer => System.Console.Out.WriteLine("New Peer Connected: {0}", peer);
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
                return newPeer;
            };

            NeutrinoConfig.PeerTimeoutMillis = 1000000;

            int elapsedMsSinceUpdate = 100;
            int msTreshold = 20;
            DateTime nextUpdate = DateTime.Now;

            while (true)
            {
                
                serverNode.Update();
                if (nextUpdate < DateTime.Now)
                {
                    nextUpdate = DateTime.Now.AddMilliseconds(msTreshold);
                    UpdateClients(serverNode);
                }
                /*var msg = serverNode.GetMessage<PlayerMessage>();
                msg.positionX = 5;
                msg.positionY = 5;
                serverNode.SendToAll(msg);
                System.Console.WriteLine("Sent.");
                */
                //System.Threading.Thread.Sleep(100);
            }
        }

        static void UpdateClients(Node serverNode)
        {
            foreach (var targetClient in clients)
            {
                foreach (var sourceClient  in clients)
                {
                    //System.Console.WriteLine("target == source ? {0}", targetClient == sourceClient);
                    if (targetClient == sourceClient) continue;
                    //System.Console.WriteLine("TARGET IS NOT SOURCE!");
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
            System.Console.WriteLine("Received Message:");
            System.Console.WriteLine("Type is: {0}", msg.GetType());
            if(msg is PlayerMessage)
            {
                int x = ((PlayerMessage)msg).positionX;
                int y = ((PlayerMessage)msg).positionY;

                System.Console.WriteLine("Received {0}, {1}", x, y);

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
        }
    }
}
