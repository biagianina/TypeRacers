using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;

namespace TypeRacers.Server
{
    public class TypeRacersServer
    {
        private static bool newClient;
        private static IPAddress ip;
        private static TcpListener server;
        private static Hashtable players;

        static void Main()
        {
            //starting the server
            ServerSetup();
        }
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
                throw new Exception("Server disconnected");
            }

            Console.WriteLine("Server started");
            CommunicationSetup();
        }

        private static void CommunicationSetup()
        {
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();

                try
                {
                    NetworkStream networkStream = client.GetStream();

                    byte[] buffer = new byte[client.ReceiveBufferSize];

                    int bytesRead = networkStream.Read(buffer, 0, client.ReceiveBufferSize);
                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    while (!dataReceived[bytesRead - 1].Equals('#'))
                    //---read incoming stream---
                    {
                        bytesRead = networkStream.Read(buffer, 0, client.ReceiveBufferSize);
                        dataReceived += Encoding.ASCII.GetString(buffer, dataReceived.Length, bytesRead);
                    }

                    CheckUsername(dataReceived, players);
                   
                    if (newClient)
                    {
                        byte[] broadcastBytes = Encoding.ASCII.GetBytes(CompetitionText); //generates random text from text document
                        networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);//send the text to connected client
                    }

                    client.Close();

                    Console.WriteLine("Disconnected client");
                }
                catch (Exception)
                {
                    Console.WriteLine("Lost connection with client");
                }
            }
        }

        private static void CheckUsername(string dataReceived, Hashtable players)
        {
            string progress = dataReceived.Substring(0, dataReceived.IndexOf('$'));
            string username = string.Concat(dataReceived.Substring(dataReceived.IndexOf('$') + 1).Except("#"));
          
            Console.WriteLine(username +" connected");
           
            if (players.ContainsKey(username))
            {
                newClient = false;
                players[username] = progress;
            }
            else
            {
                newClient = true;
                players.Add(username, progress);
            }
        }
    }
}