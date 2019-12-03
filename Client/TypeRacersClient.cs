using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TypeRacers;

namespace TypeRacers.Client
{
    public class TypeRacersClient
    {
        readonly TcpClient client ;
        private NetworkStream stream;

        public TypeRacersClient()
        {
           client = new TcpClient("localhost", 80);
            
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
