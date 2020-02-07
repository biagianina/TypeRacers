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

        void NameClient(string username);

        void StartReportingGameProgress();

        void RemovePlayer();

        void StartSearchingOpponents();

        void SubscribeToSearchingOpponents(Action<Tuple<List<Tuple<string, Tuple<string, string, int>>>, Dictionary<string, Tuple<bool, int>>>> updateOpponentsList);
        List<Tuple<string, Tuple<string, string, int>>> GetOpponents();

        Dictionary<string, Tuple<bool, int>> GetRanking();
    }
}
