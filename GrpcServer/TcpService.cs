using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Shared;
using static Shared.Protocol;
using GrpcServer.Logs;
using Google.Protobuf;

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

    public Func<TcpClient, int, string, Task>? RequestHandler { get; set; }

    public void Start() {
        string message = "";

        // create listener
        message = "Creando listener";
        Logger.Instance.WriteMessage(message);
        Console.WriteLine(message);

        IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Parse(this.ip), this.port);
        this.server = new TcpListener(localEndpoint);

        // listen to connections
        this.server.Start(100);

        message = "Esperando conexiones";
        Logger.Instance.WriteMessage(message);
        Console.WriteLine(message);

        new Task(async () => await this.AcceptConnections()).Start();
    }

    private async Task AcceptConnections() {
        string message = "";
        try
        {
            while (true)
            {
                TcpClient client = await this.server!.AcceptTcpClientAsync();

                message = "Nueva conexión aceptada";
                Logger.Instance.WriteMessage(message);
                Console.WriteLine(message);

                new Task(async () => await this.ManageClient(client)).Start();
            }
        }
        catch (SocketException)
        {
            message = "Tcp Listener cerrado";
            Logger.Instance.WriteError(message);
            Console.WriteLine(message);
        }
    }

    private async Task ManageClient(TcpClient client) {
        string message = "";
        try
        {
            this.clients.Add(client);
            while (client.Connected)
            {
                // receive header
                byte[] data = await NetworkDataHelper.Receive(client, Protocol.HeaderLen);
                (int operation, int contentLen) header = Protocol.DecodeHeader(data);

                // receive content
                data = await NetworkDataHelper.Receive(client, header.contentLen);

                // process request 
                await this.RequestHandler!(client, header.operation, Protocol.DecodeString(data));
            }
        }
        catch (SocketException)
        {
            message = "Cliente desconectado";
            Logger.Instance.WriteError(message);
            Console.WriteLine(message);
        }
    }

    public async Task Response(TcpClient client, int operation, byte[]? responseData)
    {
        string message = "";
        try
        {
            byte[] sentData = responseData ?? new byte[0];

            // send response headers
            await NetworkDataHelper.Send(client, Protocol.EncodeHeader(operation, sentData));

            // send response content
            await NetworkDataHelper.Send(client, sentData);
        }
        catch (SocketException)
        {
            message = "Cliente desconectado";
            Logger.Instance.WriteError(message);
            Console.WriteLine("Cliente desconectado");
        }
    }

    public async Task<string> ReceiveFile(TcpClient client)
    {
        FileCommsHandler fileCommsHandler = new FileCommsHandler(client);
        return await fileCommsHandler.ReceiveFile();
    }

    public async Task SendFile(TcpClient client, string path)
    {
        FileCommsHandler fileCommsHandler = new FileCommsHandler(client);
        await fileCommsHandler.SendFile(path);
    }
}
