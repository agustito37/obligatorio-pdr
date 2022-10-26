using System;
using System.IO;
using Shared;

public class Controller
{
    private TcpService service;

    public Controller(TcpService service) {
        this.service = service;
    }

    public void Connect() {
        this.service.Connect();
    }

    public void Disconnect() {
        this.service.Disconnect();
    }

    public int CreateUser(string username, string password)
    {
        User user = new User() 
        {
            Username = username,
            Password = password
        };

        (int operation, string content) response = this.service.Request(Operations.UserCreate, Protocol.Encode(user, User.Encoder));

        if (response.operation == Operations.Error)
        {
            throw new Exception(response.content);
        }

        return int.Parse(response.content);
    }
    
    public int CreateProfile(int userId, string description, List<string> abilities)
    {
        Profile profile = new Profile()
        {
            Abilites=abilities,
            Description=description,
            UserId=userId   
        };

        (int operation, string content) response = this.service.Request(Operations.ProfileCreate, Protocol.Encode(profile, Profile.Encoder));

        if (response.operation == Operations.Error)
        {
            throw new Exception(response.content);
        }

        return int.Parse(response.content);
    }

    public void AddPhoto(int id, string path)
    {
        (int operation, string content) response = this.service.SendFile(Operations.ProfileUpdatePhoto, Protocol.EncodeString(id), path);

        if (response.operation == Operations.Error)
        {
            throw new Exception(response.content);
        }
    }

    public string GetPhoto(int id)
    {
        (int operation, string content) response = this.service.GetFile(Operations.ProfileGetPhoto, Protocol.EncodeString(id));

        if (response.operation == Operations.Error)
        {
            throw new Exception(response.content);
        }

        return response.content;
    }

    public List<Profile> GetProfiles(string filter)
    {
        (int operation, string data) response = this.service.Request(Operations.ProfileGetList, Protocol.EncodeString(filter));

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

    public Profile GetProfile(int userId) {
        (int operation, string data) response = this.service.Request(Operations.ProfileGet, Protocol.EncodeString(userId));

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

    public void SendMessage(int fromUserId, int toUserId, string message)
    {
        Message msg = new() {
            FromUserId = fromUserId,
            ToUserId = toUserId,
            Text = message,
        };

        (int operation, string content) response = this.service.Request(Operations.MessageCreate, Protocol.Encode(msg, Message.Encoder));

        if (response.operation == Operations.Error)
        {
            throw new Exception(response.content);
        }
    }

    public List<Message> GetMessages(int userId) {
        (int operation, string data) response = this.service.Request(Operations.MessageGetList, Protocol.EncodeString(userId));

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
