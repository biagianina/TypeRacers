using System.Net.Sockets;
using System.Text;


namespace TypeRacers.Client
{
    public class TypeRacersClient
    {
        TcpClient client;
        NetworkStream stream;

        public string Name { get; set; }
   
        public void SendProgressToServer(string progress)
        {
            //connecting to server
            client = new TcpClient("localhost", 80);
            stream = client.GetStream();
            //writing the progress to stream
            byte[] bytesToSend = Encoding.ASCII.GetBytes(progress + "$" + Name + "#");
            stream.Write(bytesToSend, 0, bytesToSend.Length);
            stream.Flush();
        }
    }
}
