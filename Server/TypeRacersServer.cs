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
        static void Main()
        {
            //starting the server
            ServerSetup();
        }

        public static void ServerSetup()
        {
            IPAddress ip = Dns.GetHostEntry("localhost").AddressList[0];
            TcpListener server = new TcpListener(ip, 80);
            Hashtable players = new Hashtable();
           
            try
            {
                server.Start();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            Console.WriteLine("Server started");
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("New client accepted");
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
                    client.Close();
                }
                catch (Exception)
                {
                    Console.WriteLine("Client disconnected");
                }
            }
        }

        private static void CheckUsername(string dataReceived, Hashtable players)
        {
            string progress = dataReceived.Substring(0, dataReceived.IndexOf('$'));
            string username = string.Concat(dataReceived.Substring(dataReceived.IndexOf('$') + 1).Except("#"));
            
            if (players.ContainsKey(username))
            {
                players[username] = progress;
            }
            else
            {
                players.Add(username, progress);
            }
        }
    }
}