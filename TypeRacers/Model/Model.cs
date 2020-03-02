using Common;
using System;
using System.Collections.Generic;
using TypeRacers.ViewModel;

namespace TypeRacers.Model
{
    public class Model
    {
        private readonly NetworkHandler networkHandler;
        private readonly string textToType = LocalGeneratedText.GetText();
        Player player;
        IPlayroom gameInfo;

        public Model()
        {
            networkHandler = new NetworkHandler(MainViewModel.Name);
            player = networkHandler.PlayerModel();
            gameInfo = networkHandler.GameModel();
        }

        public void StartCommunication()
        {
            networkHandler.StartCommunication();
        }

        public Player GetPlayerModel()
        {
            return player;
        }

        public IPlayroom GetGameModel()
        {
            return gameInfo;
        }
        public List<Player> GetOpponents()
        {
            return gameInfo.Players;
        }

        public void RestartSearch()
        {
            networkHandler.RestartSearch();
        }

        public DateTime GetStartingTime()
        {
            return gameInfo.GameStartingTime;
        }

        public DateTime GetEndingTime()
        {
            return gameInfo.GameEndingTime;
        }

        public DateTime GetWaitingTime()
        {
            return gameInfo.TimeToWaitForOpponents;
        }

        public void SubscribeToSearchingOpponents(Action<List<Player>> updateOpponents)
        {
            networkHandler.SubscribeToSearchingOpponents(updateOpponents);
        }

        public void ReportProgress(int wpmProgress, int completedTextPercentageProgress)
        {
            networkHandler.SendProgressToServer(wpmProgress, completedTextPercentageProgress);
        }

        public string GetGeneratedTextToTypeLocally()
        {
            return textToType;
        }

        public string GetGeneratedTextToTypeFromServer()
        {
            return gameInfo.CompetitionText;
        }

        public void RemovePlayer()
        {
            networkHandler.RemovePlayer();
        }

        internal bool PlayerFinnished()
        {
            return player.Finnished;
        }

        internal string PlayerPlace()
        {
            return player.Place.ToString();
        }
    }
}