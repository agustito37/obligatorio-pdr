using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class SocketService
{
    private string ip;
    private int port;

    public SocketService(string ip, int port) {
        this.ip = ip;
        this.port = port;
    }

    public void Start() {
        Console.WriteLine("Creando socket...");

        Socket socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Parse(this.ip), this.port);
        socketServer.Bind(localEndpoint);

        socketServer.Listen(1);

        while (true)
        {
            Socket socketClient = socketServer.Accept();
            Console.WriteLine("Nueva conexión aceptada");
            new Thread(() => ManageClient(socketClient)).Start();
        }
    }

    static void ManageClient(Socket socketCliente) {
        try
        {
            while (socketCliente.Connected)
            {
                byte[] data = new byte[256];
                int received = socketCliente.Receive(data);
                if (received == 0)
                {
                    throw new SocketException();
                }

                string message = $"Cliente dice: {Encoding.UTF8.GetString(data)}";
                Console.WriteLine(message);
            }
        }
        catch (SocketException)
        {
            Console.WriteLine("Cliente Desconectado");
        }
    }
}
