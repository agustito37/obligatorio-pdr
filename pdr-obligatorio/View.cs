public class View {
    private static void connect() {
        // get connection data here
        Controller.connect();
    }

    private static void disconnect()
    {
        Controller.disconnect();
    }

    private static void createUser()
    {
        // get user data here
        Controller.createUser();
    }

    private static void createProfile()
    {
        // get profile data here
        Controller.createProfile();
    }

    private static void addPhoto()
    {
        // get photo url
        Controller.addPhoto();
    }

    private static void getProfiles()
    {
        Controller.getProfiles();
        // log profiles here
    }

    private static void getProfile()
    {
        Controller.getProfile();
        // log profile here
    }

    private static void sendMessage()
    {
        // get message data here
        Controller.sendMessage();
    }

    private static void getMessages()
    {
        Controller.getMessages();
        // log messages here
    }

    public static void Main() {
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
