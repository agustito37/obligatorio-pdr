using System;
using System.Net.Sockets;
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
            byte[] encodedMessages = Protocol.EncodeStringList(Errors);
            this.socketService.Response(client, Operations.Error, encodedMessages);

        }
        else
        {
            Console.WriteLine("Insertando usuario: {0}", user.Username);
            int id = Persistence.Instance.AddUser(user);

            byte[] encodedMessage = Protocol.EncodeString(id.ToString());
            this.socketService.Response(client, Operations.Ok, encodedMessage);
        }
    }

    private void CreateProfile(Socket client, string data) {

    }

    private void AddPhoto(Socket client, string data) {

    }

    private void GetProfiles(Socket client, string data) {

    }

    private void GetProfile(Socket client, string data) {

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
