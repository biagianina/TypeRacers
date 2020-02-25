using System.Text;
using Common;

namespace TypeRacers.Client
{
    public class PlayerMessage : IMessage
    {
        readonly int wpmProgress;
        readonly int completedTextPercentage;
        readonly string name;
        public PlayerMessage(int wpmProgress, int completedTextPercentage, string name)
        {
            this.wpmProgress = wpmProgress;
            this.completedTextPercentage = completedTextPercentage;
            this.name = name;
        }
        public byte[] ToByteArray()
        {
            return Encoding.ASCII.GetBytes(wpmProgress + "&" + completedTextPercentage + "$" + name + "#");
        }
    }
}
