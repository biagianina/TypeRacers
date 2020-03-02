using Common;
using System;
using System.Collections.Generic;
using TypeRacers.ViewModel;

namespace TypeRacers.Model
{
    public class Model
    {
        private readonly NetworkHandler networkHandler;
        private readonly Player player;
        private readonly IPlayroom gameInfo;

        public Model()
        {
            networkHandler = new NetworkHandler(MainViewModel.Name);
            player = networkHandler.PlayerModel();
            gameInfo = networkHandler.GameModel();
        }

        public Player GetPlayer()
        {
            return player;
        }

        public IPlayroom GetGameInfo()
        {
            return gameInfo;
        }

        public void StartCommunication()
        {
            networkHandler.StartCommunication();
        }

        public string GetGeneratedTextToTypeLocally()
        {
            return LocalGeneratedText.GetText();
        }
    }
}