using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Shared;

public class SocketService
{
    string remoteIp;
    int remotePort;
    Socket? socket;

    public SocketService(string remoteIp, int remotePort) {
        this.remoteIp = remoteIp;
        this.remotePort = remotePort;
    }

    public void Connect() {
        if (this.socket == null || !this.socket.Connected) {
            Console.WriteLine("Conectando al Servidor...");
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
            this.socket.Bind(localEndPoint);

            IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Parse(this.remoteIp), this.remotePort);
            this.socket!.Connect(remoteEndpoint);
        }
        else
        {
            Console.WriteLine("Ya hay una conexión establecida");
        }
    }

    public void Disconnect() {
        if (this.socket != null && this.socket.Connected)
        {
            Console.WriteLine("Cerrando la conexion...");
            this.socket.Shutdown(SocketShutdown.Both);
            this.socket.Close();
        }
        else
        {
            Console.WriteLine("No hay una conexión establecida");
        }
    }

    public (int operation, string response) Request(int operation, byte[]? encodedData) {
        if (this.socket != null)
        {
            byte[] sentData = encodedData ?? new byte[0];

            // send headers
            NetworkDataHelper.Send(this.socket, Protocol.EncodeHeader(operation, sentData));

            // send content
            NetworkDataHelper.Send(this.socket, sentData);

            // receive response header
            byte[] responseData = NetworkDataHelper.Receive(this.socket, Protocol.headerLen);
            (int responseOperation, int responseContenLen) header = Protocol.DecodeHeader(responseData);

            // receive response content
            // that could be:
            // OK response, could have content or not
            // ERROR response, description of error in content
            responseData = NetworkDataHelper.Receive(this.socket, header.responseContenLen);

            return (header.responseOperation, Protocol.DecodeBytes(responseData));
        }
        else
        {
            throw new Exception("No hay una conexión establecida");
        }
    }
}
