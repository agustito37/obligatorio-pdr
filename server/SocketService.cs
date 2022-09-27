using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Shared;
using static Shared.Protocol;

public class SocketService
{
    private string ip;
    private int port;
    private List<Socket> clientSockets = new();
    private Socket? serverSocket = null;

    public SocketService(string ip, int port) {
        this.ip = ip;
        this.port = port;
    }

    public Action<Socket, int, string> RequestHandler { get; set; } = (socket, operation, request) => { };

    public void Start() {
        // create socket
        Console.WriteLine("Creando socket...");
        this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Parse(this.ip), this.port);
        this.serverSocket.Bind(localEndpoint);

        // listen to connections
        this.serverSocket.Listen(1);
        Console.WriteLine("Esperando conexiones...");
        new Thread(() => this.AcceptConnections()).Start();

        // close on command
        CloseServerOnCommand();
    }

    private void CloseServerOnCommand() {
        string input = "";
        while (input != "salir")
        {
            Console.WriteLine("Ingrese 'salir' para cerrar el servidor");
            input = ConsoleHelpers.RequestNonEmptyText("El comando no puede estar vacío");
        }

        // close client socket connections
        foreach (Socket client in this.clientSockets)
        {
            if (client.Connected)
            {
                client.Shutdown(SocketShutdown.Both);
            }
        }

        // close server socket
        this.serverSocket!.Close();
    }

    private void AcceptConnections() {
        try
        {
            while (true)
            {
                Socket clientSocket = this.serverSocket!.Accept();
                Console.WriteLine("Nueva conexión aceptada");
                new Thread(() => this.ManageClient(clientSocket)).Start();
            }
        }
        catch (SocketException)
        {
            Console.WriteLine("Socket cerrado");
        }
    }

    private void ManageClient(Socket clientSocket) {
        try
        {
            this.clientSockets.Add(clientSocket);
            while (clientSocket.Connected)
            {
                // receive header
                byte[] data = NetworkDataHelper.Receive(clientSocket, Protocol.HeaderLen);
                (int operation, int contentLen) header = Protocol.DecodeHeader(data);

                // receive content
                data = NetworkDataHelper.Receive(clientSocket, header.contentLen);

                // process request 
                this.RequestHandler(clientSocket, header.operation, Protocol.DecodeString(data));
            }
        }
        catch (SocketException)
        {
            Console.WriteLine("Cliente desconectado");
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
            Console.WriteLine("Cliente desconectado");
        }
    }

    public string ReceiveFile(Socket clientSocket)
    {
        FileCommsHandler fileCommsHandler = new FileCommsHandler(clientSocket);
        return fileCommsHandler.ReceiveFile();
    }

    public void SendFile(Socket clientSocket, string path)
    {
        FileCommsHandler fileCommsHandler = new FileCommsHandler(clientSocket);
        fileCommsHandler.SendFile(path);
    }
}
