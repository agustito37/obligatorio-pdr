using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Shared;

public class TCPController
{
    private TcpService service;

    public TCPController(TcpService tcpService) {
        this.service = tcpService;
        this.service.RequestHandler = async (TcpClient client, int operation, string data) =>
        {
            switch (operation)
            {
                case Operations.UserCreate:
                    await this.CreateUser(client, data);
                    break;
                case Operations.ProfileCreate:
                    await this.CreateProfile(client, data);
                    break;
                case Operations.ProfileGetPhoto:
                    await this.GetPhoto(client, data);
                    break;
                case Operations.ProfileUpdatePhoto:
                    await this.AddPhoto(client, data);
                    break;
                case Operations.ProfileGet:
                    await this.GetProfile(client, data);
                    break;
                case Operations.ProfileGetList:
                    await this.GetProfiles(client, data);
                    break;
                case Operations.MessageCreate:
                    await this.SendMessage(client, data);
                    break;
                case Operations.MessageGetList:
                    await this.GetMessages(client, data);
                    break;
            }
        };
    }

    private async Task CreateUser(TcpClient client, string data) {
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
            await this.service.Response(client, Operations.Error, Protocol.EncodeStringList(Errors));

        }
        else
        {
            Console.WriteLine("Insertando usuario: {0}", user.Username);
            int id = Persistence.Instance.AddUser(user);

            await this.service.Response(client, Operations.Ok, Protocol.EncodeString(id.ToString()));
        }
    }

    private async Task CreateProfile(TcpClient client, string data) {
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
            await this.service.Response(client, Operations.Error, Protocol.EncodeStringList(Errors));
        }
        else
        {
            Console.WriteLine("Insertando perfil: {0}", profile.Description);
            int id = Persistence.Instance.AddProfile(profile);

            await this.service.Response(client, Operations.Ok, Protocol.EncodeString(id.ToString()));
        }
    }

    private async Task AddPhoto(TcpClient client, string idUsuario) {
        Profile? profile = Persistence.Instance.GetProfiles().Find((p) => p.UserId == int.Parse(idUsuario));

        if (profile == null)
        {
            await this.service.Response(client, Operations.Error, Protocol.EncodeString("Perfil no existente"));
            return;
        }

        // send control Ok before streaming
        await this.service.Response(client, Operations.Ok, null);

        string path = "";
        try
        {
            // receive file stream
            path = await this.service.ReceiveFile(client);
        }
        catch (SocketException) {
            Console.WriteLine("Error al enviar archivo");
        }

        Persistence.Instance.SetProfilePhoto(profile.Id, path);
    }

    private async Task GetPhoto(TcpClient client, string idUsuario)
    {
        Profile? profile = Persistence.Instance.GetProfiles().Find((p) => p.UserId == int.Parse(idUsuario));

        if (profile == null) {
           await this.service.Response(client, Operations.Error, Protocol.EncodeString("Perfil no existente"));
            return;
        }

        if (profile.ImagePath == "")
        {
            await this.service.Response(client, Operations.Error, Protocol.EncodeString("El perfil no contiene imagen"));
            return;
        }

        // send control Ok before streaming
        await this.service.Response(client, Operations.Ok, Protocol.EncodeString(profile.ImagePath));

        try
        {
            // send file stream
            await this.service.SendFile(client, profile.ImagePath);
        }
        catch (SocketException)
        {
            Console.WriteLine("Error al enviar archivo");
        }
    }

    private async Task GetProfiles(TcpClient client, string query) {
        (string key, string value) filter = Protocol.getParam(query);
        List<Profile> profiles = Persistence.Instance.GetProfiles(filter.key, filter.value);

        await this.service.Response(client, Operations.Ok, Protocol.EncodeList(profiles, Profile.Encoder));
    }

    private async Task GetProfile(TcpClient client, string userId) {
        Profile? profile = Persistence.Instance.GetProfiles().Find((p) => p.UserId == int.Parse(userId));

        if (profile == null) {
            await this.service.Response(client, Operations.Error, Protocol.EncodeString("Perfil no existente"));
            return;
        }

        await this.service.Response(client, Operations.Ok, Protocol.Encode(profile, Profile.Encoder));
    }

    private async Task SendMessage(TcpClient client, string msg) {
        Message message = Message.Decoder(msg);

        User? userFrom = Persistence.Instance.GetUsers().Find((u) => u.Id == message.FromUserId);
        if (userFrom == null)
        {
            await this.service.Response(client, Operations.Error, Protocol.EncodeString("Usuario origen no existente"));
            return;
        }

        User? userTo = Persistence.Instance.GetUsers().Find((u) => u.Id == message.ToUserId);
        if (userTo == null)
        {
            await this.service.Response(client, Operations.Error, Protocol.EncodeString("Usuario destino no existente"));
            return;
        }

        Persistence.Instance.AddMessage(message);

        await this.service.Response(client, Operations.Ok, null);
    }

    private async Task GetMessages(TcpClient client, string userId) {
        int id = Convert.ToInt32(userId);
        List<Message> messages = Persistence.Instance.GetMessages(id);

        await this.service.Response(client, Operations.Ok, Protocol.EncodeList(messages, Message.Encoder));

        // after sent, mark received messages as seen
        List<int> ids = messages.FindAll((m) => m.ToUserId == id).ConvertAll((m) => m.Id);
        Persistence.Instance.SetSeenMessages(ids);
    }
}
