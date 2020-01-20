using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeRacers
{
    //interface for network handler
    interface INetworkHandler
    { 
        void SendProgressToServer(string progress, bool userIsInGame);

        string GetTextFromServer();

        List<Tuple<string, string, bool>> GetOpponents();

    }
}
