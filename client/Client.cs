﻿using Shared;

public class Client {
    private static Controller? controller;

    private static void Connect() {
        controller!.Connect();
    }

    private static void Disconnect() {
        controller!.Disconnect();
    }

    private static void CreateUser() {
        // get user data here
        controller!.CreateUser();
    }

    private static void CreateProfile() {
        // get profile data here
        controller!.CreateProfile();
    }

    private static void AddPhoto() {
        // get photo url
        controller!.AddPhoto();
    }

    private static void GetProfiles() {
        controller!.GetProfiles();
        // log profiles here
    }

    private static void GetProfile() {
        controller!.GetProfile();
        // log profile here
    }

    private static void SendMessage() {
        Console.WriteLine("Ingrese los datos del mensaje");

        // TODO: agregar validaciones para numericos
        Console.Write("Id usuario que envia: ");
        int fromUserId = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Id usuario que recibe: ");
        int toUserId = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Texto: ");
        string message = Console.ReadLine() ?? "";

        controller!.SendMessage(fromUserId, toUserId, message);
    }

    private static void GetMessages() {
        Console.Write("Id de usuario que recibe: ");
        string userId = Console.ReadLine() ?? "";

        List<Message> messages = controller!.GetMessages(userId);

        Console.WriteLine("--- Mensajes ---");
        foreach (Message message in messages) {
            Console.WriteLine("Id: " + message.Id);
            Console.WriteLine("From: " + message.FromUserId);
            Console.WriteLine("To: " + message.ToUserId);
            Console.WriteLine("Seen: " + message.Seen);
            Console.WriteLine("Text: " + message.Text);
            Console.WriteLine("----------------");
        }
    }

    public static void Main() {
        Console.WriteLine("Iniciando cliente...");

        SocketService socketService = new SocketService("127.0.0.1", 5000);
        controller = new Controller(socketService);

        Menu mainMenu = new()
        {
            Title = "Menu principal",
            Options = new List<(string, Delegate)> {
                ("Conectarse a servidor", Connect),
                ("Desconectarse de servidor", Disconnect),
                ("Alta usuario", CreateUser),
                ("Alta perfil de trabajo", CreateProfile),
                ("Asociar foto de perfil de trabajo", AddPhoto),
                ("Consultar perfiles existentes", GetProfiles),
                ("Consultar un perfil específico", GetProfile),
                ("Enviar mensajes", SendMessage),
                ("Consultar mensajes", GetMessages),
            }
        };
        mainMenu.Show();
    }
}
