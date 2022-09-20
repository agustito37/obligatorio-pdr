using Shared;

public class Client
{

    static readonly SettingsManager settingsManager = new SettingsManager();

    private static Controller? controller;

    private static void Connect() {
        controller!.Connect();
    }

    private static void Disconnect() {
        controller!.Disconnect();
    }

    private static void CreateUser() {
        bool flag = false;
        do
        {
            try
            {
                Console.WriteLine("Inserte el nombre de usuario");
                string username = ConsoleHelpers.RequestNonEmptyText("No puedes dejar vacío este campo");
                Console.WriteLine("Inserte la contraseña");
                string password = ConsoleHelpers.RequestNonEmptyText("No puedes dejar vacío este campo");
                int id = controller!.CreateUser(username, password);
                Console.WriteLine("Usuario insertado con el id: {0}", id);
                flag = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }while (!flag);
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


        string ServerIp = settingsManager.ReadSettings(ServerConfig.ServerIPConfigKey);
        int ServerPort = int.Parse(settingsManager.ReadSettings(ServerConfig.ServerPortConfigKey));

        SocketService socketService = new SocketService(ServerIp, ServerPort);
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
