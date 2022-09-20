using Shared;

namespace Server
{
    class Server
    {
        static readonly SettingsManager settingsManager = new SettingsManager();
        public static void Main()
        {
            Console.WriteLine("Iniciando servidor...");

            string ServerIp = settingsManager.ReadSettings(ServerConfig.ServerIPConfigKey);
            int ServerPort = int.Parse(settingsManager.ReadSettings(ServerConfig.ServerPortConfigKey));

            SocketService socketService = new SocketService(ServerIp, ServerPort);
            Controller controller = new Controller(socketService);

            socketService.Start();
        }
    }
}
