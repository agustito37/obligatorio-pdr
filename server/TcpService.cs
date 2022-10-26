using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Shared;
using static Shared.Protocol;

public class TcpService
{
    private string ip;
    private int port;
    private List<TcpClient> clients = new();
    private TcpListener? server = null;

    public TcpService(string ip, int port) {
        this.ip = ip;
        this.port = port;
    }

    public Action<TcpClient, int, string> RequestHandler { get; set; } = (client, operation, request) => { };

    public void Start() {
        // create listener
        Console.WriteLine("Creando listener...");
        IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Parse(this.ip), this.port);
        this.server = new TcpListener(localEndpoint);

        // listen to connections
        this.server.Start(100);
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

        // close client listener connections
        foreach (TcpClient client in this.clients)
        {
            if (client.Connected)
            {
                client.GetStream().Close();
                client.Close();
            }
        }

        // close server listener
        this.server!.Stop();
    }

    private void AcceptConnections() {
        try
        {
            while (true)
            {
                TcpClient client = this.server!.AcceptTcpClient();
                Console.WriteLine("Nueva conexión aceptada");
                new Thread(() => this.ManageClient(client)).Start();
            }
        }
        catch (SocketException)
        {
            Console.WriteLine("Tcp Listener cerrado");
        }
    }

    private void ManageClient(TcpClient client) {
        try
        {
            this.clients.Add(client);
            while (client.Connected)
            {
                // receive header
                byte[] data = NetworkDataHelper.Receive(client, Protocol.HeaderLen);
                (int operation, int contentLen) header = Protocol.DecodeHeader(data);

                // receive content
                data = NetworkDataHelper.Receive(client, header.contentLen);

                // process request 
                this.RequestHandler(client, header.operation, Protocol.DecodeString(data));
            }
        }
        catch (SocketException)
        {
            Console.WriteLine("Cliente desconectado");
        }
    }

    public void Response(TcpClient client, int operation, byte[]? responseData)
    {
        try
        {
            byte[] sentData = responseData ?? new byte[0];

            // send response headers
            NetworkDataHelper.Send(client, Protocol.EncodeHeader(operation, sentData));

            // send response content
            NetworkDataHelper.Send(client, sentData);
        }
        catch (SocketException)
        {
            Console.WriteLine("Cliente desconectado");
        }
    }

    public string ReceiveFile(TcpClient client)
    {
        FileCommsHandler fileCommsHandler = new FileCommsHandler(client);
        return fileCommsHandler.ReceiveFile();
    }

    public void SendFile(TcpClient client, string path)
    {
        FileCommsHandler fileCommsHandler = new FileCommsHandler(client);
        fileCommsHandler.SendFile(path);
    }
}
