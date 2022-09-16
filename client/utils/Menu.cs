﻿public class Menu
{
    public string? Title { get; set; }
    public List<(string key, Delegate operation)>? Options { get; set; }
    private readonly string EXIT_KEY = "salir";

    private string getOptionsMessage()
    {
        string optionsMessage = "\n";
        int index = 1;
        foreach (var option in Options!)
        {
            optionsMessage += index.ToString() + ") " + option.key + "\n";
            index += 1;
        }
        optionsMessage += EXIT_KEY + "\n";
        return optionsMessage;
    }

    private string ReadOption()
    {
        while (true)
        {
            string input = Console.ReadLine() ?? "";
            if (input == EXIT_KEY)
            {
                return input;
            }

            Delegate option;
            try
            {
                int indexOption = int.Parse(input) - 1;
                option = Options![indexOption].operation;
                Console.WriteLine("### " + Options![indexOption].key + " ###");
            }
            catch (Exception)
            {
                Console.WriteLine("Opción inválida: {0}", input);
                Console.WriteLine("Ingrese la opción nuevamente:", input);
                continue;
            }

            option.DynamicInvoke();
            return input;
        }
    }

    public void Show()
    {
        if (Options == null)
        {
            throw new Exception("\nOptions are not defined");
        }

        string input;
        do
        {
            if (Title != null)
            {
                Console.WriteLine("\n{0}", Title);
            }
            Console.WriteLine(getOptionsMessage());
            input = ReadOption();
        } while (input != EXIT_KEY);
    }
}
