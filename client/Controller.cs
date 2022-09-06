using System;
public class Controller
{
    private SocketService socketService;

    public Controller(SocketService socketService) {
        this.socketService = socketService;
    }

    public void connect() {
        this.socketService.Connect();
    }

    public void disconnect()
    {
        this.socketService.Disconnect();
    }

    public void createUser()
    {
        // request to socket here
    }

    public void createProfile()
    {
        // request to socket here
    }

    public void addPhoto()
    {
        // request to socket here
    }

    public void getProfiles()
    {
        // request to socket here
    }

    public void getProfile()
    {
        // request to socket here
    }

    public void sendMessage(string message)
    {
        this.socketService.SendMessage(message);
    }

    public void getMessages()
    {
        // request to socket here
    }
}
