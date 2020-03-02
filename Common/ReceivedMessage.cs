using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Common
{
    public class ReceivedMessage : IMessage
    {

        private string Data { get; set; }

        public byte[] ToByteArray()
        {
            return default;
        }

        public void DecodeMessage(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            Data = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            while (!Data.Contains("#"))
            {
                bytesRead = stream.Read(buffer, 0, 1024);
                Data += Encoding.ASCII.GetString(buffer, Data.Length, bytesRead);
            }

            Data = Data.Substring(0, Data.IndexOf('#'));
        }

        public string GetData()
        {
            return string.IsNullOrEmpty(Data) ? string.Empty : Data;
        }
    }
}
