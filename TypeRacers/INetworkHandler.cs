using System;
using System.Collections.Generic;

namespace TypeRacers
{
    //interface for network handler
    internal interface INetworkHandler
    {
        void SendProgressToServer(string progress);

        string GetTextFromServer();

        DateTime GetStartingTime();

        DateTime GetEndingTime();

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