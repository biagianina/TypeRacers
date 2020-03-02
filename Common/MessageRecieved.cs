using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Common
{
    public class MessageRecieved : IMessage
    {
        byte[] message;
        public MessageRecieved(byte[] message)
        {
            this.message = message;
        }

        public byte[] ToByteArray()
        {
            return null;
        }

        public string GetString()
        {
            var dataRecieved = Encoding.ASCII.GetString(message);
            if (!dataRecieved.Contains("#"))
            {
                return string.Empty;
            }

            var completeMessage = dataRecieved.Substring(0, dataRecieved.IndexOf('#'));
            return dataRecieved.Remove(0, completeMessage.Length - 1);
        }
    }
}