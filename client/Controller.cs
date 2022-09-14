using System;
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

    public void SendMessage(string message) {
        this.socketService.SendData(message);
    }

    public void GetMessages() {
        // request to socket here
    }
}
