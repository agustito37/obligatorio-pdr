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
        // create listener
        Logger.Instance.WriteMessage("Creando listener");
        IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Parse(this.ip), this.port);
        this.server = new TcpListener(localEndpoint);

        // listen to connections
        this.server.Start(100);

        Logger.Instance.WriteMessage("Esperando conexiones");
        new Task(async () => await this.AcceptConnections()).Start();
    }

    private async Task AcceptConnections() {
        try
        {
            while (true)
            {
                TcpClient client = await this.server!.AcceptTcpClientAsync();
                Logger.Instance.WriteMessage("Nueva conexión aceptada");
                new Task(async () => await this.ManageClient(client)).Start();
            }
        }
        catch (SocketException)
        {
            Logger.Instance.WriteError("Tcp Listener cerrado");
        }
    }

    private async Task ManageClient(TcpClient client) {
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
            Logger.Instance.WriteError("Cliente desconectado");
        }
    }

    public async Task Response(TcpClient client, int operation, byte[]? responseData)
    {
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
            Logger.Instance.WriteError("Cliente desconectado");
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
