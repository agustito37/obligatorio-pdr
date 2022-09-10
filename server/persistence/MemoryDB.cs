public sealed class MemoryDB
{
    // singleton
    public static MemoryDB Instance { get; } = new MemoryDB();

    private int uid = 0;
    private readonly List<User> users = new();
    private readonly List<Profile> profiles = new();
    private readonly List<Message> messages = new();

    public List<User> GetUsers() {
        lock (this.users) {
            return this.users;
        }
    }

    public void addUser(User user)
    {
        lock (this.users)
        {
            // unique Username constraint
            User? foundUser = this.users.Find((u) => u.Username == user.Username);
            if (foundUser == null) { 
                this.uid += 1;
                user.Id = uid;
                this.users.Add(user);
            }
        }
    }

    public List<Profile> getProfiles()
    {
        lock (this.profiles)
        {
            return this.profiles;
        }
    }

    public void AddProfile(Profile profile)
    {
        lock (this.profiles)
        {
            // unique UserId constraint
            Profile? foundProfile = this.profiles.Find((u) => u.UserId == profile.UserId);
            if (foundProfile == null)
            {
                this.uid += 1;
                profile.Id = uid;
                this.profiles.Add(profile);
            }
        }
    }

    public void UpdateProfile(Profile profile)
    {
        lock (this.profiles)
        {
            Profile? foundProfile = this.profiles.Find((p) => p.Id == profile.Id);
            if (foundProfile != null)
            {
                // can only update ImagePath
                foundProfile.ImagePath = profile.ImagePath ?? foundProfile.ImagePath;
            }
        }
    }

    public List<Message> getMessages()
    {
        lock (this.messages)
        {
            return this.messages;
        }
    }

    public void AddMessages(Message message)
    {
        lock (this.messages)
        {
            this.uid += 1;
            message.Id = uid;
            this.messages.Add(message);
        }
    }

    public void UpdateMessage(Message message)
    {
        lock (this.messages)
        {
            Message? foundMessage = this.messages.Find((m) => m.Id == message.Id);
            if (foundMessage != null)
            {
                // can only update Seen
                foundMessage.Seen = message.Seen ?? foundMessage.Seen;
            }
        }
    }

    public void BlockingTransaction(Delegate deleg) {
        lock (this)
        {
            deleg.DynamicInvoke();
        }
    }
}
