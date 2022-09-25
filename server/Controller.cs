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
            this.socketService.Response(client, Operations.Error, Protocol.EncodeStringList(Errors));

        }
        else
        {
            Console.WriteLine("Insertando usuario: {0}", user.Username);
            int id = Persistence.Instance.AddUser(user);

            this.socketService.Response(client, Operations.Ok, Protocol.EncodeString(id.ToString()));
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
        if (Persistence.Instance.GetProfiles().Where(x => x.UserId == profile.UserId).ToList().Count > 0)
        {
            Errors.Add("Ya existe un perfil para ese usuario");
        }
        if (Errors.Count > 0)
        {
            this.socketService.Response(client, Operations.Error, Protocol.EncodeStringList(Errors));
        }
        else
        {
            Console.WriteLine("Insertando perfil: {0}", profile.Description);
            int id = Persistence.Instance.AddProfile(profile);

            this.socketService.Response(client, Operations.Ok, Protocol.EncodeString(id.ToString()));
        }
    }

    private void AddPhoto(Socket client, string idUsuario) {
        Profile? profile = Persistence.Instance.GetProfiles().Find((p) => p.UserId == int.Parse(idUsuario));

        if (profile == null)
        {
            this.socketService.Response(client, Operations.Error, Protocol.EncodeString("Perfil no existente"));
            return;
        }

        // send control Ok before streaming
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

        Persistence.Instance.SetProfilePhoto(profile.Id, path);
    }

    private void GetPhoto(Socket client, string idUsuario)
    {
        Profile? profile = Persistence.Instance.GetProfiles().Find((p) => p.UserId == int.Parse(idUsuario));

        if (profile == null) {
            this.socketService.Response(client, Operations.Error, Protocol.EncodeString("Perfil no existente"));
            return;
        }

        if (profile.ImagePath == "")
        {
            this.socketService.Response(client, Operations.Error, Protocol.EncodeString("El perfil no contiene imagen"));
            return;
        }

        // send control Ok before streaming
        this.socketService.Response(client, Operations.Ok, Protocol.EncodeString(profile.ImagePath));

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

    private void GetProfiles(Socket client, string query) {
        (string key, string value) filter = Protocol.getParam(query);
        List<Profile> profiles = Persistence.Instance.GetProfiles(filter.key, filter.value);

        this.socketService.Response(client, Operations.Ok, Protocol.EncodeList(profiles, Profile.Encoder));
    }

    private void GetProfile(Socket client, string userId) {
        Profile? profile = Persistence.Instance.GetProfiles().Find((p) => p.UserId == int.Parse(userId));

        if (profile == null) {
            this.socketService.Response(client, Operations.Error, Protocol.EncodeString("Perfil no existente"));
            return;
        }

        this.socketService.Response(client, Operations.Ok, Protocol.Encode(profile, Profile.Encoder));
    }

    private void SendMessage(Socket client, string msg) {
        Message message = Message.Decoder(msg);

        User? userFrom = Persistence.Instance.GetUsers().Find((u) => u.Id == message.FromUserId);
        if (userFrom == null)
        {
            this.socketService.Response(client, Operations.Error, Protocol.EncodeString("Usuario origen no existente"));
            return;
        }

        User? userTo = Persistence.Instance.GetUsers().Find((u) => u.Id == message.ToUserId);
        if (userTo == null)
        {
            this.socketService.Response(client, Operations.Error, Protocol.EncodeString("Usuario destino no existente"));
            return;
        }

        Persistence.Instance.AddMessage(message);

        this.socketService.Response(client, Operations.Ok, null);
    }

    private void GetMessages(Socket client, string userId) {
        List<Message> messages = Persistence.Instance.GetMessages(Convert.ToInt32(userId));

        this.socketService.Response(client, Operations.Ok, Protocol.EncodeList(messages, Message.Encoder));

        // after sent, mark as seen
        List<int> ids = messages.ConvertAll((m) => m.Id);
        Persistence.Instance.SetSeenMessages(ids);
    }
}
