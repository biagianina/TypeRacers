using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace TypeRacers.Client
{
    public class PlayerMessage : IMessage
    {
        int wpmProgress;
        int completedTextPercentage;
        string name;
        bool restart;
        bool removed;

        public PlayerMessage(int wpmProgress, int completedTextPercentage, string name, bool restart, bool removed)
        {
            this.wpmProgress = wpmProgress;
            this.completedTextPercentage = completedTextPercentage;
            this.name = name;
            this.restart = restart;
            this.removed = removed;
        }
        public byte[] ToByteArray()
        {
            if (restart)
            {
                return Encoding.ASCII.GetBytes(wpmProgress + "&" + completedTextPercentage + "$" + name + "_restart" + "#");
            }
            else if (removed)
            {
                return Encoding.ASCII.GetBytes(wpmProgress + "&" + completedTextPercentage + "$" + name + "_removed" + "#");
            }
            else
            {
                return Encoding.ASCII.GetBytes(wpmProgress + "&" + completedTextPercentage + "$" + name + "#");
            }

        }
    }
}
