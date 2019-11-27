using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace echo
{
    public class Server
    {
        IPAddress ip = Dns.GetHostEntry("localhost").AddressList[0];
        TcpListener server;
        TcpClient client = default;

        public void ServerSetup()
        {
            server = new TcpListener(ip, 123);
            try
            {
                server.Start();                
            }
            catch (Exception exception)
            {
               throw new Exception(exception.Message);
            }

            while (true)
            {
                client = server.AcceptTcpClient();
                HandleClient cl = new HandleClient(); //server sends a message to a new connected client
                cl.StartClient(client);
            }
        }
    }
}
