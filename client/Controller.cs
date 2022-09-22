using System;
using Shared;

public class Controller
{
    private SocketService socketService;

    public Controller(SocketService socketService) {
        this.socketService = socketService;
    }

    public void Connect() {
        this.socketService.Connect();
    }

    public void Disconnect() {
        this.socketService.Disconnect();
    }

    public int CreateUser(string username, string password) {
        User user = new User() 
        {
            Username = username,
            Password = password
        };
        byte[] encodedData = Protocol.Encode(user, User.Encoder);
        (int operation, string content) response = this.socketService.Request(Operations.UserCreate, encodedData);

        if (response.operation == Operations.Error)
        {
            throw new Exception(response.content);
        }
        return int.Parse(response.content);
    }
    
    public int CreateProfile(int userId, string description, string imagePath, List<string> abilities) {
        Profile profile = new Profile()
        {
            Abilites=abilities,
            Description=description,
            ImagePath=imagePath,
            UserId=userId   
        };
        byte[] encodedData = Protocol.Encode(profile, Profile.Encoder);
        (int operation, string content) response = this.socketService.Request(Operations.ProfileCreate, encodedData);
        if (response.operation == Operations.Error)
        {
            throw new Exception(response.content);
        }
        return int.Parse(response.content);
    }

    public void AddPhoto(string id, string path) {

        byte[] encodedData = Protocol.EncodeString(id);
        (int operation, string content) response = this.socketService.SendFile(Operations.ProfileUpdatePhoto, encodedData, path);

        if (response.operation == Operations.Error)
        {
            throw new Exception(response.content);
        }
    }

    public void GetPhoto()
    {
        // request to socket here
    }

    public List<Profile> GetProfiles(string ability) {
       
        (int operation, string data) response = this.socketService.Request(Operations.ProfileGetList, Protocol.EncodeString(ability));

        List<Profile> profiles = new List<Profile>();
        if (response.operation != Operations.Error)
        {
            profiles = Protocol.DecodeList(response.data, Profile.Decoder);
        }
        else
        {
            throw new Exception(response.data);
        }

        return profiles;
    }

    public Profile GetProfile(string userId) {
        (int operation, string data) response = this.socketService.Request(Operations.ProfileGet, Protocol.EncodeString(userId));

        Profile profile = new Profile();
        if (response.operation != Operations.Error)
        {
            profile = Protocol.Decode(response.data, Profile.Decoder);
        }
        else
        {
            throw new Exception(response.data);
        }

        return profile;
    }

    public void SendMessage(int fromUserId, int toUserId, string message) {
        Message msg = new() {
            FromUserId = fromUserId,
            ToUserId = toUserId,
            Text = message,
        };
        byte[] encodedData = Protocol.Encode(msg, Message.Encoder);

        (int operation, string content) response = this.socketService.Request(Operations.MessageCreate, encodedData);

        if (response.operation == Operations.Error)
        {
            throw new Exception(response.content);
        }
    }

    public List<Message> GetMessages(string userId) {
        (int operation, string data) response = this.socketService.Request(Operations.MessageGetList, Protocol.EncodeString(userId));

        List<Message> messages = new List<Message>();
        if (response.operation != Operations.Error)
        {
            messages = Protocol.DecodeList(response.data, Message.Decoder);
        }
        else
        {
            throw new Exception(response.data);
        }

        return messages;
    }

}
