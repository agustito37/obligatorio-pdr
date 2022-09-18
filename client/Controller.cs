﻿using System;
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

    public void CreateUser() {
        // request to socket here
    }

    public void CreateProfile() {
        // request to socket here
    }

    public void AddPhoto() {
        // request to socket here
    }

    public void GetProfiles() {
        // request to socket here
    }

    public void GetProfile() {
        // request to socket here
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
