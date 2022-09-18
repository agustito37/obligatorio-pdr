using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Shared;
using static Shared.Protocol;

public class SocketService
{
    private string ip;
    private int port;

    public SocketService(string ip, int port) {
        this.ip = ip;
        this.port = port;
    }

    public Action<Socket, int, string> RequestHandler { get; set; } = (socket, operation, request) => { };

    public void Start() {
        Console.WriteLine("Creando socket...");

        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Parse(this.ip), this.port);
        serverSocket.Bind(localEndpoint);

        serverSocket.Listen(1);
        Console.WriteLine("Esperando conexiones...");

        while (true)
        {
            Socket clientSocket = serverSocket.Accept();
            Console.WriteLine("Nueva conexión aceptada");
            new Thread(() => this.ManageClient(clientSocket)).Start();
        }
    }

    private void ManageClient(Socket clientSocket) {
        try
        {
            while (clientSocket.Connected)
            {
                // receive header
                byte[] data = NetworkDataHelper.Receive(clientSocket, Protocol.headerLen);
                (int operation, int contentLen) header = Protocol.DecodeHeader(data);

                // receive content
                data = NetworkDataHelper.Receive(clientSocket, header.contentLen);

                // process request on controller
                this.RequestHandler(clientSocket, header.operation, Encoding.UTF8.GetString(data));
            }
        }
        catch (SocketException)
        {
            Console.WriteLine("Cliente Desconectado");
        }
    }

    public void Response(Socket clientSocket, int operation, byte[]? responseData)
    {
        try
        {
            byte[] sentData = responseData ?? new byte[0];

            // send response headers
            NetworkDataHelper.Send(clientSocket, Protocol.EncodeHeader(operation, sentData));

            // send response content
            NetworkDataHelper.Send(clientSocket, sentData);
        }
        catch (SocketException)
        {
            Console.WriteLine("Cliente Desconectado");
        }
    }
}
