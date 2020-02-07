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
        void SendProgressToServer(string progress);

        string GetTextFromServer();

        string GetStartingTime();

        string GetEndingTime();

        void RestartSearch();

        void StartSearchingOpponents();

        List<Tuple<string, Tuple<string, string, int>>> GetOpponents();

        Dictionary<string, Tuple<bool, int>> GetRanking();
    }
}
