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
                bool restart;
        bool removed;


        public PlayerMessage(int wpmProgress, int completedTextPercentage, string name, bool firstConection, bool restart, bool removed)
        {
            this.wpmProgress = wpmProgress;
            this.completedTextPercentage = completedTextPercentage;
            this.name = name;
            
            this.firstConection = firstConection;   
            this.restart = restart;
            this.removed = removed;
        }
        public byte[] ToByteArray()
        {
            if (restart)
            {
                return Encoding.ASCII.GetBytes(wpmProgress + "&" + completedTextPercentage +  "&" + firstConection + "$" + name + "_restart" + "#");
            }
            else if (removed)
            {
                return Encoding.ASCII.GetBytes(wpmProgress + "&" + completedTextPercentage + "&" + firstConection + "$" + name + "_removed" + "#");
            }
            else
            {
                return Encoding.ASCII.GetBytes(wpmProgress + "&" + completedTextPercentage + "&" + firstConection + "$" + name + "#");
            }
    }
}
