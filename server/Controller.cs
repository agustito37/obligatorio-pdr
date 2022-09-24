using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Shared;

public class Controller
{
    private SocketService socketService;

    public Controller(SocketService socketService) {
        this.socketService = socketService;
        this.socketService.RequestHandler = (Socket client, int operation, string data) =>
        {
            switch (operation)
            {
                case Operations.UserCreate:
                    this.CreateUser(client, data);
                    break;
                case Operations.ProfileCreate:
                    this.CreateProfile(client, data);
                    break;
                case Operations.ProfileGetPhoto:
                    this.GetPhoto(client, data);
                    break;
                case Operations.ProfileUpdatePhoto:
                    this.AddPhoto(client, data);
                    break;
                case Operations.ProfileGet:
                    this.GetProfile(client, data);
                    break;
                case Operations.ProfileGetList:
                    this.GetProfiles(client, data);
                    break;
                case Operations.MessageCreate:
                    this.SendMessage(client, data);
                    break;
                case Operations.MessageGetList:
                    this.GetMessages(client, data);
                    break;
            }
        };
    }

    private void CreateUser(Socket client, string data) {
        User user = User.Decoder(data);

        List<string> Errors = new List<string>();

        if (user.Username == String.Empty)
        {
            Errors.Add("No puedes dejar vacio el nombre de usuario");
        }
        if (user.Password == String.Empty)
        {
            Errors.Add("No puedes dejar vacia la contraseña");
        }
        if (Persistence.Instance.GetUsers().Where(x => x.Username == user.Username).ToList().Count > 0)
        {
            Errors.Add("Ya existe un usuario con ese nombre, por favor ingrese otro");

        }
        if (Errors.Count > 0)
        {
            byte[] encodedData = Protocol.EncodeStringList(Errors);
            this.socketService.Response(client, Operations.Error, encodedData);

        }
        else
        {
            Console.WriteLine("Insertando usuario: {0}", user.Username);
            int id = Persistence.Instance.AddUser(user);

            byte[] encodedData = Protocol.EncodeString(id.ToString());
            this.socketService.Response(client, Operations.Ok, encodedData);
        }
    }

    private void CreateProfile(Socket client, string data) {
        Profile profile = Profile.Decoder(data);

        List<string> Errors = new List<string>();

        if (profile.Description == String.Empty)
        {
            Errors.Add("No puedes dejar vacia la descripcion del perfil");
        }
        if (profile.Abilites.Count==0)
        {
            Errors.Add("No puedes dejar vacia las habilidades del perfil");
        }
        if (Persistence.Instance.GetProfiles().Where(x => x.Id == profile.Id).ToList().Count > 0)
        {
            Errors.Add("Ya existe un perfil con ese Id, por favor ingrese otro");
        }
        if (Errors.Count > 0)
        {
            byte[] encodedMessages = Protocol.EncodeStringList(Errors);
            this.socketService.Response(client, Operations.Error, encodedMessages);

        }
        else
        {
            Console.WriteLine("Insertando perfil: {0}", profile.Description);
            int id = Persistence.Instance.AddProfile(profile);

            byte[] encodedData = Protocol.EncodeString(id.ToString());
            this.socketService.Response(client, Operations.Ok, encodedData);
        }
    }

    private void AddPhoto(Socket client, string idPerfil) {
        Profile? profile = Persistence.Instance.GetProfiles().Find((p) => p.Id == int.Parse(idPerfil));

        if (profile == null)
        {
            byte[] encodedData = Protocol.EncodeString("Perfil no existente");
            this.socketService.Response(client, Operations.Error, encodedData);
            return;
        }

        // send control Ok before streaming
        byte[] encodedParam = Protocol.EncodeString(profile.ImagePath);
        this.socketService.Response(client, Operations.Ok, null);

        string path = "";
        try
        {
            // receive file stream
            path = this.socketService.ReceiveFile(client);
        }
        catch (Exception) {
            Console.WriteLine("Error al enviar archivo");
        }

        int id = int.Parse(idPerfil);
        Persistence.Instance.SetProfilePhoto(id, path);
    }

    private void GetPhoto(Socket client, string idPerfil)
    {
        Profile? profile = Persistence.Instance.GetProfiles().Find((p) => p.Id == int.Parse(idPerfil));

        if (profile == null) {
            byte[] encodedData = Protocol.EncodeString("Perfil no existente");
            this.socketService.Response(client, Operations.Error, encodedData);
            return;
        }

        if (profile.ImagePath == "")
        {
            byte[] encodedData = Protocol.EncodeString("El perfil no contiene imagen");
            this.socketService.Response(client, Operations.Error, encodedData);
            return;
        }

        // send control Ok before streaming
        byte[] encodedParam = Protocol.EncodeString(profile.ImagePath);
        this.socketService.Response(client, Operations.Ok, encodedParam);

        try
        {
            // send file stream
            this.socketService.SendFile(client, profile.ImagePath);
        }
        catch (Exception)
        {
            Console.WriteLine("Error al enviar archivo");
        }
    }

    private void GetProfiles(Socket client, string ability) {
        List<Profile> profiles = Persistence.Instance.GetProfiles(ability);

        byte[] encodedProfiles = Protocol.EncodeList(profiles, Profile.Encoder);
        this.socketService.Response(client, Operations.Ok, encodedProfiles);
    }

    private void GetProfile(Socket client, string userId) {
        Profile profile=new Profile();
        List<Profile> profiles = Persistence.Instance.GetProfiles();
        for (int i = 0; i < profiles.Count; i++)
        {
            if (Convert.ToInt32(userId)==profiles[i].UserId)
            {
                 profile.Abilites = profiles[i].Abilites;
                 profile.Description = profiles[i].Description;
                 profile.UserId = profiles[i].UserId;
                 profile.ImagePath = profiles[i].ImagePath;
                 profile.Id = profiles[i].Id;
            }
        }
        byte[] encodedMessages = Protocol.Encode(profile,Profile.Encoder);
        this.socketService.Response(client, Operations.Ok, encodedMessages);
    }

    private void SendMessage(Socket client, string msg) {
        Message message = Message.Decoder(msg);

        // TODO: add validations on the message, e.g. non existent user ids
        Persistence.Instance.AddMessages(message);

        this.socketService.Response(client, Operations.Ok, null);
    }

    private void GetMessages(Socket client, string userId) {
        List<Message> messages = Persistence.Instance.GetMessages(Convert.ToInt32(userId));

        byte[] encodedMessages = Protocol.EncodeList(messages, Message.Encoder);
        this.socketService.Response(client, Operations.Ok, encodedMessages);

        // after sent, mark as seen
        List<int> ids = messages.ConvertAll((m) => m.Id);
        Persistence.Instance.SetSeenMessages(ids);
    }
}
