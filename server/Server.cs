using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;

class Server
{
    public static void Main() {
        SocketService socketService = new SocketService("127.0.0.1", 5000);
        Controller controller = new Controller(socketService);
        socketService.Start();
    }
}