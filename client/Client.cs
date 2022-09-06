public class Client {
    private static Controller? controller;

    private static void connect() {
        controller!.connect();
    }

    private static void disconnect()
    {
        controller!.disconnect();
    }

    private static void createUser()
    {
        // get user data here
        controller!.createUser();
    }

    private static void createProfile()
    {
        // get profile data here
        controller!.createProfile();
    }

    private static void addPhoto()
    {
        // get photo url
        controller!.addPhoto();
    }

    private static void getProfiles()
    {
        controller!.getProfiles();
        // log profiles here
    }

    private static void getProfile()
    {
        controller!.getProfile();
        // log profile here
    }

    private static void sendMessage()
    {
        Console.WriteLine("Ingrese el mensaje:");
        string message = Console.ReadLine() ?? "";
        controller!.sendMessage(message);
    }

    private static void getMessages()
    {
        controller!.getMessages();
        // log messages here
    }

    public static void Main() {
        SocketService socketService = new SocketService("127.0.0.1", 5000);
        socketService.Start();
        controller = new Controller(socketService);
        
        Menu mainMenu = new()
        {
            Title = "Menu principal",
            Options = new List<Tuple<String, Delegate>> {
                new Tuple<string, Delegate>("Conectarse a servidor", connect),
                new Tuple<string, Delegate>("Desconectarse de servidor", disconnect),
                new Tuple<string, Delegate>("Alta usuario", createUser),
                new Tuple<string, Delegate>("Alta perfil de trabajo", createProfile),
                new Tuple<string, Delegate>("Asociar foto de perfil de trabajo", addPhoto),
                new Tuple<string, Delegate>("Consultar perfiles existentes", getProfiles),
                new Tuple<string, Delegate>("Consultar un perfil específico", getProfile),
                new Tuple<string, Delegate>("Enviar mensajes", sendMessage),
                new Tuple<string, Delegate>("Consultar mensajes", getMessages),
            }
        };
        mainMenu.Show();
    }
}
