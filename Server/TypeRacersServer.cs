namespace TypeRacers.Server
{
    public class TypeRacersServer
    {
        private static void Main()
        {
            var server = new ServerSetup();
            server.Setup();
        }
    }
}