using Common;
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

        public IPlayroom GetGameInfo()
        {
            return networkHandler.GameModel();
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