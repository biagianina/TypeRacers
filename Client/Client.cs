using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Client
    {
<<<<<<< HEAD
        readonly TcpClient client = new TcpClient(System.Configuration.ConfigurationManager.AppSettings["JuniorMindIP"], 123);
=======
        IPAddress host = IPAddress.Parse("192.168.1.125");

        readonly TcpClient client ;
>>>>>>> b605b724f34f7a11b8b2202c6475da3d99d8d76c
        private NetworkStream stream;

        public Client()
        {
           client = new TcpClient("localhost", 123);
        }
        //returns a string to connect to the MVVM
        public string GetMessageFromServer()
        {
            stream = client.GetStream();
            try
            {
                byte[] inStream = new byte[10025];
                var read = stream.Read(inStream, 0, inStream.Length);
                return Encoding.ASCII.GetString(inStream, 0, read);
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }
    }
}
