using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace TypeRacers.Server
{
    public class TypeRacersServer
    {
        private static bool newClient;
        private static IPAddress ip;
        private static TcpListener server;
        private static Hashtable players;
        private static NetworkStream networkStream;
        private static string currentClient;

        static void Main()
        {
            //starting the server
            ServerSetup();
        }

        //to avoid generating different texts from users in same competition
        public static string CompetitionText { get; } = ServerGeneratedText.GetText();

        public static void ServerSetup()
        {

            ip = Dns.GetHostEntry("localhost").AddressList[0];
            server = new TcpListener(ip, 80);
            players = new Hashtable();

            try
            {
                server.Start();
            }
            catch (Exception)
            {
                throw new Exception("Server setup failed");
            }

            Console.WriteLine("Server started");
            CommunicationSetup();//separated the communication from server starter
        }

        private static void CommunicationSetup()
        {
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();

                try
                {
                    //creates the stream
                    networkStream = client.GetStream();
                    //reads from stream
                    byte[] buffer = new byte[client.ReceiveBufferSize];
                    int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    //solution to get get complete messages
                    while (!dataReceived[bytesRead - 1].Equals('#'))
                    {
                        bytesRead = networkStream.Read(buffer, 0, client.ReceiveBufferSize);
                        dataReceived += Encoding.ASCII.GetString(buffer, dataReceived.Length, bytesRead);
                    }

                    CheckUsername(dataReceived, players);
                    //this bool is set in order to do the text to type sending only once

                    client.Close();
                    Console.WriteLine("info: " + dataReceived);
                    Console.WriteLine("Disconnected client");
                }
                catch (Exception)
                {
                    Console.WriteLine("Lost connection with client");
                }
            }
        }

        private static void CheckNewClient()
        {
            if (newClient)
            {
                byte[] broadcastBytes = Encoding.ASCII.GetBytes(CompetitionText + "#"); //generates random text from text document
                networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);//send the text to connected client
                //SendOpponents();//sending current connected opponents to connected client
            }
            else
            {
                SendOpponents();
            }
        }

        private static void SendOpponents()
        {
            string opponents = string.Empty;
            foreach (DictionaryEntry a in players)
            {
                if (!a.Key.ToString().Equals(currentClient))
                {
                    opponents += a.Key + ": " + a.Value + "/";
                }
            }

            byte[] broadcastBytes = Encoding.ASCII.GetBytes(opponents + "#");
            networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);
        }


        //this method determines if a player is new or is already playing and is just sending progress
        private static void CheckUsername(string dataReceived, Hashtable players)
        {
            string progress = dataReceived.Substring(0, dataReceived.IndexOf('$'));
            string username = dataReceived.Substring(dataReceived.IndexOf('$') + 1);
            currentClient = username.Substring(0, username.Length - 1);
            Console.WriteLine(username + " connected");

            if (players.ContainsKey(currentClient))
            {
                newClient = false;
                players[currentClient] = progress;
            }
            else
            {
                newClient = true;
                players.Add(currentClient, progress);
            }

            CheckNewClient();
        }
    }
}