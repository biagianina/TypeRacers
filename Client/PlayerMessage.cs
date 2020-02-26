using System.Text;
using Common;

namespace TypeRacers.Client
{
    public class PlayerMessage : IMessage
    {
        readonly int wpmProgress;
        readonly int completedTextPercentage;
        readonly string name;
        readonly bool firstConection;
        public PlayerMessage(int wpmProgress, int completedTextPercentage, string name, bool firstConection)
        {
            this.wpmProgress = wpmProgress;
            this.completedTextPercentage = completedTextPercentage;
            this.name = name;
            this.firstConection = firstConection;
        }
        public byte[] ToByteArray()
        { 
            return Encoding.ASCII.GetBytes(wpmProgress + "&" + completedTextPercentage + "&" + firstConection + "$" + name + "#");
        }
    }
}
