using System.Net.Sockets;
using System.Text;


namespace TypeRacers.Client
{
    public class TypeRacersClient
    {
        readonly TcpClient client;
        private NetworkStream stream;


        public TypeRacersClient()
        {
           client = new TcpClient("localhost", 80);
           stream = client.GetStream();
        }
         
        public void SendProgressToServer(string progress)
        {
            //writing the progress to stream
            byte[] bytesToSend = Encoding.ASCII.GetBytes(progress);
            stream.Write(bytesToSend, 0, bytesToSend.Length);
        }
    }
}
