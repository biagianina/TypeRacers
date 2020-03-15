using Common;
using TypeRacers.Client;
using TypeRacers.ViewModel;

namespace TypeRacers.Model
{
    public class Model
    {
        private readonly NetworkHandler networkHandler;

        public Model()
        {
            networkHandler = new NetworkHandler(MainViewModel.Name);
        }

        public Player GetPlayer()
        {
            return networkHandler.PlayerModel();
        }

        public GameInfo GetGameInfo()
        {
            return networkHandler.GameModel();
        }

        public bool StartCommunication()
        {
            return networkHandler.StartCommunication();
        }

        public string GetGeneratedTextToTypeLocally()
        {
            var localText = new LocalGeneratedText();
            return localText.GetData();
        }
    }
}