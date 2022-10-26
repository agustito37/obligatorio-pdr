using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using Shared;

public class TcpService
{
    string localIp;
    string remoteIp;
    int remotePort;
    TcpClient? client;

    public TcpService(string localIp, string remoteIp, int remotePort) {
        this.localIp = localIp;
        this.remoteIp = remoteIp;
        this.remotePort = remotePort;
    }

    public void Connect() {
        if (this.client == null || !this.client.Connected) {
            try
            {
                Console.WriteLine("Conectando al Servidor...");
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(this.localIp), 0);
                this.client = new TcpClient(localEndPoint);

                IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Parse(this.remoteIp), this.remotePort);
                this.client!.Connect(remoteEndpoint);
            }
            catch (SocketException)
            {
                Console.WriteLine("No se puedo conectar con el servidor");
            }
        }
        else
        {
            Console.WriteLine("Ya hay una conexión establecida");
        }
    }

    public void Disconnect() {
        if (this.client != null && this.client.Connected)
        {
            Console.WriteLine("Cerrando la conexion...");
            this.client.GetStream().Close();
            this.client.Close();
        }
        else
        {
            Console.WriteLine("No hay una conexión establecida");
        }
    }

    public (int operation, string response) Request(int operation, byte[]? encodedData) {
        if (this.client != null && this.client.Connected)
        {
            try
            {
                byte[] sentData = encodedData ?? new byte[0];

                // send headers
                NetworkDataHelper.Send(this.client, Protocol.EncodeHeader(operation, sentData));

                // send content
                NetworkDataHelper.Send(this.client, sentData);

                // receive response header
                byte[] responseData = NetworkDataHelper.Receive(this.client, Protocol.HeaderLen);
                (int responseOperation, int responseContenLen) header = Protocol.DecodeHeader(responseData);

                // receive response content
                // that could be:
                // OK response, could have content or not
                // ERROR response, description of error in content
                responseData = NetworkDataHelper.Receive(this.client, header.responseContenLen);

                return (header.responseOperation, Protocol.DecodeString(responseData));
            }
            catch (SocketException) {
                throw new Exception("No se pudo conectar con el servidor");
            }
        }
        else
        {
            throw new Exception("No hay una conexión establecida");
        }
    }

    public (int operation, string response) SendFile(int operation, byte[]? encodedData, string path)
    {
        if (!FileHandler.FileExists(path)) {
            throw new Exception("Archivo no existente");
        }

        // make a control request
        (int responseOperation, string responseData) header = this.Request(operation, encodedData);

        // if response is ok, send file stream
        if (header.responseOperation == Operations.Ok)
        {
            FileCommsHandler fileCommsHandler = new FileCommsHandler(this.client!);
            fileCommsHandler.SendFile(path);
        }

        return (header.responseOperation, header.responseData);
    }

    public (int operation, string fileName) GetFile(int operation, byte[]? encodedData)
    {
        // make a control request
        (int responseOperation, string responseData) header = this.Request(operation, encodedData);

        // if response is ok, receive file stream
        if (header.responseOperation == Operations.Ok) {
            FileCommsHandler fileCommsHandler = new FileCommsHandler(this.client!);
            fileCommsHandler.ReceiveFile();
        }

        return (header.responseOperation, header.responseData);
    }
}
