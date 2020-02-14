using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
namespace Client
{
    public class PlayroomInfo : IPlayroom<PlayerInfo>
    {
        public string CompetitionText => throw new NotImplementedException();

        public List<PlayerInfo> Players { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime GameStartingTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime GameEndingTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime TimeToWaitForOpponents { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Place { get; internal set; }

        public PlayerInfo GetPlayer(string name)
        {
            throw new NotImplementedException();
        }
    }
}
