using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Google.Protobuf;
using Shared;
using Shared.domain;
using GrpcServer.Logs;

public class TcpController
{
    private TcpService service;

    public TcpController(TcpService tcpService) {
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
        string resultMessage = "";
        User user = User.Decoder(data);

        List<string> Errors = new List<string>();

        if (user.Username == String.Empty)
        {
            resultMessage = "No puedes dejar vacio el nombre de usuario";
            Logger.Instance.WriteWarning(resultMessage);
            Errors.Add(resultMessage);
        }
        if (user.Password == String.Empty)
        {
            resultMessage = "No puedes dejar vacia la contraseña";
            Logger.Instance.WriteWarning(resultMessage);
            Errors.Add(resultMessage);
        }
        if (Persistence.Instance.GetUsers().Where(x => x.Username == user.Username).ToList().Count > 0)
        {
            resultMessage = "Ya existe un usuario con ese nombre, por favor ingrese otro";
            Logger.Instance.WriteWarning(resultMessage);
            Errors.Add(resultMessage);
        }
        if (Errors.Count > 0)
        {
            await this.service.Response(client, Operations.Error, Protocol.EncodeStringList(Errors));

        }
        else
        {
            resultMessage = "Insertando usuario: " + user.Username;
            Logger.Instance.WriteInfo(resultMessage);
            int id = Persistence.Instance.AddUser(user);

            await this.service.Response(client, Operations.Ok, Protocol.EncodeString(id.ToString()));
        }
    }

    private async Task CreateProfile(TcpClient client, string data) {
        string resultMessage = "";
        Profile profile = Profile.Decoder(data);

        List<string> Errors = new List<string>();

        if (profile.Description == String.Empty)
        {
            resultMessage = "No puedes dejar vacia la descripcion del perfil";
            Logger.Instance.WriteWarning(resultMessage);
            Errors.Add(resultMessage);
        }
        if (profile.Abilites.Count==0)
        {
            resultMessage = "No puedes dejar vacia las habilidades del perfil";
            Logger.Instance.WriteWarning(resultMessage);
            Errors.Add(resultMessage);
        }
        if (Persistence.Instance.GetProfiles().Where(x => x.UserId == profile.UserId).ToList().Count > 0)
        {
            resultMessage = "Ya existe un perfil para ese usuario";
            Logger.Instance.WriteWarning(resultMessage);
            Errors.Add(resultMessage);
        }
        if (Errors.Count > 0)
        {
            await this.service.Response(client, Operations.Error, Protocol.EncodeStringList(Errors));
        }
        else
        {
            Logger.Instance.WriteInfo("Insertando perfil: " + profile.Description);
            int id = Persistence.Instance.AddProfile(profile);
            await this.service.Response(client, Operations.Ok, Protocol.EncodeString(id.ToString()));
        }
    }

    private async Task AddPhoto(TcpClient client, string idUsuario) {
        string resultMessage = "";
        Profile? profile = Persistence.Instance.GetProfiles().Find((p) => p.UserId == int.Parse(idUsuario));

        if (profile == null)
        {
            resultMessage = "Perfil no existente";
            Logger.Instance.WriteWarning(resultMessage);
            await this.service.Response(client, Operations.Error, Protocol.EncodeString(resultMessage));
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
            Logger.Instance.WriteError("Error al enviar archivo");
        }
        catch (Exception ex)
        {
            Logger.Instance.WriteError(ex.Message);
        }

        Persistence.Instance.SetProfilePhoto(profile.Id, path);
    }

    private async Task GetPhoto(TcpClient client, string idUsuario)
    {
        string resultMessage = "";
        Profile? profile = Persistence.Instance.GetProfiles().Find((p) => p.UserId == int.Parse(idUsuario));

        if (profile == null) {
            resultMessage = "Perfil no existente";
            Logger.Instance.WriteWarning(resultMessage);
            await this.service.Response(client, Operations.Error, Protocol.EncodeString(resultMessage));
            return;
        }

        if (profile.ImagePath == "")
        {
            resultMessage = "El perfil no contiene imagen";
            Logger.Instance.WriteWarning(resultMessage);
            await this.service.Response(client, Operations.Error, Protocol.EncodeString(resultMessage));
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
            Logger.Instance.WriteError("Error al enviar archivo");
        }
        catch (Exception ex)
        {
            Logger.Instance.WriteError(ex.Message);
        }
    }

    private async Task GetProfiles(TcpClient client, string query) {
        (string key, string value) filter = Protocol.getParam(query);
        List<Profile> profiles = Persistence.Instance.GetProfiles(filter.key, filter.value);

        await this.service.Response(client, Operations.Ok, Protocol.EncodeList(profiles, Profile.Encoder));
    }

    private async Task GetProfile(TcpClient client, string userId) {
        string resultMessage = "";
        Profile? profile = Persistence.Instance.GetProfiles().Find((p) => p.UserId == int.Parse(userId));

        if (profile == null) {
            resultMessage = "Perfil no existente";
            Logger.Instance.WriteWarning(resultMessage);
            await this.service.Response(client, Operations.Error, Protocol.EncodeString(resultMessage));
            return;
        }

        await this.service.Response(client, Operations.Ok, Protocol.Encode(profile, Profile.Encoder));
    }

    private async Task SendMessage(TcpClient client, string msg) {
        string resultMessage = "";
        Message message = Message.Decoder(msg);

        User? userFrom = Persistence.Instance.GetUsers().Find((u) => u.Id == message.FromUserId);
        if (userFrom == null)
        {
            resultMessage = "Usuario origen no existente";
            Logger.Instance.WriteWarning(resultMessage);
            await this.service.Response(client, Operations.Error, Protocol.EncodeString(resultMessage));
            return;
        }

        User? userTo = Persistence.Instance.GetUsers().Find((u) => u.Id == message.ToUserId);
        if (userTo == null)
        {
            resultMessage = "Usuario destino no existente";
            Logger.Instance.WriteWarning(resultMessage);
            await this.service.Response(client, Operations.Error, Protocol.EncodeString(resultMessage));
            return;
        }

        Persistence.Instance.AddMessage(message);

        Logger.Instance.WriteInfo("Mensaje creado");
        await this.service.Response(client, Operations.Ok, null);
    }

    private async Task GetMessages(TcpClient client, string userId) {
        int id = Convert.ToInt32(userId);
        List<Message> messages = Persistence.Instance.GetMessages(id);

        Logger.Instance.WriteInfo("Mensajes obtenidos");
        await this.service.Response(client, Operations.Ok, Protocol.EncodeList(messages, Message.Encoder));

        // after sent, mark received messages as seen
        Logger.Instance.WriteInfo("Mensajes leídos");
        List<int> ids = messages.FindAll((m) => m.ToUserId == id).ConvertAll((m) => m.Id);
        Persistence.Instance.SetSeenMessages(ids);
    }
}
