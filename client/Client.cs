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
        Console.WriteLine("Ingrese el mensaje:");
        string message = Console.ReadLine() ?? "";
        controller!.SendMessage(message);
    }

    private static void GetMessages() {
        controller!.GetMessages();
        // log messages here
    }

    public static void Main() {
        Console.WriteLine("Iniciando cliente...");

        SocketService socketService = new SocketService("127.0.0.1", 5000);
        controller = new Controller(socketService);

        Menu mainMenu = new()
        {
            Title = "Menu principal",
            Options = new List<Tuple<String, Delegate>> {
                new Tuple<string, Delegate>("Conectarse a servidor", Connect),
                new Tuple<string, Delegate>("Desconectarse de servidor", Disconnect),
                new Tuple<string, Delegate>("Alta usuario", CreateUser),
                new Tuple<string, Delegate>("Alta perfil de trabajo", CreateProfile),
                new Tuple<string, Delegate>("Asociar foto de perfil de trabajo", AddPhoto),
                new Tuple<string, Delegate>("Consultar perfiles existentes", GetProfiles),
                new Tuple<string, Delegate>("Consultar un perfil específico", GetProfile),
                new Tuple<string, Delegate>("Enviar mensajes", SendMessage),
                new Tuple<string, Delegate>("Consultar mensajes", GetMessages),
            }
        };
        mainMenu.Show();
    }
}
