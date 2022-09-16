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
                case (int)Protocol.operations.USER_CREATE:
                    this.CreateUser(client, data);
                    break;
                case (int)Protocol.operations.PROFILE_CREATE:
                    this.CreateProfile(client, data);
                    break;
                case (int)Protocol.operations.PROFILE_UPDATE_PHOTO:
                    this.AddPhoto(client, data);
                    break;
                case (int)Protocol.operations.PROFILE_GET:
                    this.GetProfile(client, data);
                    break;
                case (int)Protocol.operations.PROFILE_GET_LIST:
                    this.GetProfiles(client, data);
                    break;
                case (int)Protocol.operations.MESSAGE_CREATE:
                    this.SendMessage(client, data);
                    break;
                case (int)Protocol.operations.MESSAGE_GET_LIST:
                    this.GetMessages(client, data);
                    break;
            }
        };
    }

    private void CreateUser(Socket client, string data) {

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

        this.socketService.Reponse(client, (int)Protocol.operations.OK, null);
    }

    private void GetMessages(Socket client, string userId) {
        List<Message> messages = Persistence.Instance.GetMessages(Convert.ToInt32(userId));
        byte[] encodedData = Protocol.EncodeList(messages.Cast<object>().ToList(), Message.Encoder);

        this.socketService.Reponse(client, (int)Protocol.operations.OK, encodedData);

        // after sent, mark as seen
        List<int> ids = messages.ConvertAll((m) => m.Id);
        Persistence.Instance.SetSeenMessages(ids);
    }
}
