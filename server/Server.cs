class Server
{
    public static void Main() {
        Console.WriteLine("Iniciando servidor...");

        SocketService socketService = new SocketService("127.0.0.1", 5000);
        Controller controller = new Controller(socketService);

        socketService.Start();
    }
}
