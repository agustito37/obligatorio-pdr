﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class SocketService
{
    string remoteIp;
    int remotePort;
    Socket? socketClient;

    public SocketService(string remoteIp, int remotePort) {
        this.remoteIp = remoteIp;
        this.remotePort = remotePort;
    }

    public void Start()
    {
        Console.WriteLine("Iniciando Cliente...");
        this.socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
        this.socketClient.Bind(localEndPoint);
    }

    public void Connect() {
        if (socketClient == null)
        {
            throw new Exception("Server not started");
        }

        if (!socketClient.Connected) {
            Console.WriteLine("Conectando al Servidor...");
            var remoteEndpoint = new IPEndPoint(IPAddress.Parse(this.remoteIp), this.remotePort);
            this.socketClient!.Connect(remoteEndpoint);

        } else {
            Console.WriteLine("Ya hay una conexión establecida");
        }
    }

    public void Disconnect() {
        if (socketClient == null)
        {
            throw new Exception("Server not started");
        }

        if (socketClient.Connected)
        {
            Console.WriteLine("Cerrando la conexion...");
            this.socketClient.Shutdown(SocketShutdown.Both);
            this.socketClient.Close();
        }
        else {
            Console.WriteLine("No hay una conexión establecida");
        }
    }

    public void SendMessage(string message)
    {
        if (socketClient == null)
        {
            throw new Exception("Server not started");
        }

        if (socketClient.Connected)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            this.socketClient.Send(data);
        }
        else
        {
            Console.WriteLine("No hay una conexión establecida");
        }
    }
}
