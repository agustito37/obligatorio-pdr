public class Client {
    private static void NotImplementedOption() {
        Console.WriteLine("Opción no implementada");
    }

    public static void Main() {
        Menu mainMenu = new()
        {
            Title = "Menu principal",
            Options = new List<Tuple<String, Delegate>> {
                new Tuple<string, Delegate>("Conectarse a servidor", NotImplementedOption),
                new Tuple<string, Delegate>("Desconectarse de servidor", NotImplementedOption),
                new Tuple<string, Delegate>("Alta usuario", NotImplementedOption),
                new Tuple<string, Delegate>("Alta perfil de trabajo", NotImplementedOption),
                new Tuple<string, Delegate>("Asociar foto de perfil de trabajo", NotImplementedOption),
                new Tuple<string, Delegate>("Consultar perfiles existentes", NotImplementedOption),
                new Tuple<string, Delegate>("Consultar un perfil específico", NotImplementedOption),
                new Tuple<string, Delegate>("Enviar mensajes", NotImplementedOption),
                new Tuple<string, Delegate>("Consultar mensajes", NotImplementedOption),
            }
        };
        mainMenu.Show();
    }
}
