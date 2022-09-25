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
        bool flag = false;
        bool abilitisEnd = false;
        List<string> abilities=new List<string>();
        do
        {
            try
            {
                Console.WriteLine("Inserte el Id del usuario ");
                int userId = ConsoleHelpers.RequestInt("No puedes dejar vacío este campo");
                Console.WriteLine("Inserte la descripcion ");
                string description = ConsoleHelpers.RequestNonEmptyText("No puedes dejar vacío este campo");
                while (!abilitisEnd)
                {
                    Console.WriteLine("Inserte una habilidad(Para dejar de insertar inserte: terminar)");
                    string ability = ConsoleHelpers.RequestNonEmptyText("No puedes dejar vacío este campo");
                    if (!ability.Equals("terminar"))
                    {
                        abilities.Add(ability);
                    }
                    else abilitisEnd = true;
                }
                
                int id = controller!.CreateProfile(userId,  description, abilities);
                Console.WriteLine("Perfil insertado con el id: {0}", id);
                flag = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } while (!flag);
    }

    private static void AddPhoto() {
        Console.WriteLine("Inserte id de perfil");
        string id = ConsoleHelpers.RequestNonEmptyText("No puedes dejar vacío este campo");

        Console.WriteLine("Inserte la ruta");
        string path = ConsoleHelpers.RequestNonEmptyText("No puedes dejar vacío este campo");

        try
        {
            controller!.AddPhoto(id, path);

            Console.WriteLine("Foto actualizada");
        }
        catch (Exception e)
        {
            Console.Write(e.Message);
        }
    }

    private static void GetPhoto()
    {
        Console.WriteLine("Inserte id de perfil");
        string id = ConsoleHelpers.RequestNonEmptyText("No puedes dejar vacío este campo");

        try
        {
            string fileName = controller!.GetPhoto(id);

            Console.WriteLine("Nombre foto perfil: " + fileName);
        }
        catch (Exception e)
        {
            Console.Write(e.Message);
        }
    }

    private static void GetProfiles() {
        Console.WriteLine("Buscar perfil  por:");
        Console.WriteLine("1) Descripcion");
        Console.WriteLine("2) Habilidad");
        var userInput = Console.ReadLine();
        int input = int.TryParse(userInput, out input) ? input : 1000;
        string message = "";

        while (input != 1 && input != 2)
        {
            Console.WriteLine("La opcion no existe, ingrese denuevo");
            userInput = Console.ReadLine();
            input = int.TryParse(userInput, out input) ? input : 1000;
        }
        if (input == 1)
        {
            Console.WriteLine("Ingrese la descripcio de buscar");
            message = "byDescription" + "#" + Console.ReadLine();
        }
        else if (input == 2)
        {
            Console.WriteLine("Ingrese la descripcion a buscar");
            message = "by" + "#" + Console.ReadLine();
        }

        try
        {
            List<Profile> profiles = controller!.GetProfiles(message);

            Console.WriteLine("--- Perfiles ---");
            foreach (Profile profile in profiles)
            {
                Console.WriteLine("Id: " + profile.Id);
                Console.WriteLine("User Id: " + profile.UserId);
                Console.WriteLine("Descripcion: " + profile.Description);
                Console.WriteLine("Habilidades:");
                for (int i = 0; i < profile.Abilites.Count; i++)
                {
                    Console.WriteLine(profile.Abilites[i].ToString());
                }
                Console.WriteLine("Imagen: " + profile.ImagePath);
                Console.WriteLine("----------------");
            }
        }
        catch (Exception e)
        {
            Console.Write(e.Message);
        }
    }

    private static void GetProfile() {
        Console.Write("Ingrese el Id del usuario:");
        string userId = Console.ReadLine() ?? "";

        try
        {
            Profile profile = controller!.GetProfile(userId);

            Console.WriteLine("--- Perfil ---");
            Console.WriteLine("Id: " + profile.Id);
            Console.WriteLine("User Id: " + profile.UserId);
            Console.WriteLine("Descripcion: " + profile.Description);
            Console.WriteLine("Habilidades:");
            for (int i = 0; i < profile.Abilites.Count; i++)
            {
                Console.WriteLine(profile.Abilites[i].ToString());
            }
            Console.WriteLine("Imagen: " + profile.ImagePath);
            Console.WriteLine("----------------");
        }
        catch (Exception e)
        {
            Console.Write(e.Message);
        }
    }

    private static void SendMessage() {
        Console.WriteLine("Ingrese los datos del mensaje");

        Console.Write("Id usuario que envia: ");
        int fromUserId = ConsoleHelpers.RequestInt("No puedes dejar vacío este campo");

        Console.Write("Id usuario que recibe: ");
        int toUserId = ConsoleHelpers.RequestInt("No puedes dejar vacío este campo");

        Console.Write("Texto: ");
        string message = Console.ReadLine() ?? "";

        try
        {
            controller!.SendMessage(fromUserId, toUserId, message);
        }
        catch (Exception e)
        {
            Console.Write(e.Message);
        }
        
    }

    private static void GetMessages() {
        Console.Write("Id de usuario que recibe: ");
        string userId = Console.ReadLine() ?? "";

        try
        {
            List<Message> messages = controller!.GetMessages(userId);

            Console.WriteLine("--- Mensajes ---");
            foreach (Message message in messages)
            {
                Console.WriteLine("Id: " + message.Id);
                Console.WriteLine("From: " + message.FromUserId);
                Console.WriteLine("To: " + message.ToUserId);
                Console.WriteLine("Seen: " + message.Seen);
                Console.WriteLine("Text: " + message.Text);
                Console.WriteLine("----------------");
            }
        }
        catch (Exception e)
        {
            Console.Write(e.Message);
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
                ("Obtener foto de perfil de trabajo", GetPhoto),
                ("Consultar perfiles existentes", GetProfiles),
                ("Consultar un perfil específico", GetProfile),
                ("Enviar mensajes", SendMessage),
                ("Consultar mensajes", GetMessages),
            }
        };
        mainMenu.Show();
    }
}
