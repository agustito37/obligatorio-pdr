using Shared;

public sealed class Persistence
{
    // singleton
    public static Persistence Instance { get; } = new Persistence();

    private int uid = 0;
    private readonly List<User> users = new();
    private readonly List<Profile> profiles = new();
    private readonly List<Message> messages = new();

    public List<User> GetUsers() {
        lock (this.users) {
            // deep clone to avoid shared read references
            return this.users.ConvertAll(user => new User {
                Id = user.Id,
                Username = user.Username,
                Password = user.Password,
            });
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
            // deep clone to avoid shared read references
            return this.profiles.ConvertAll(profile => new Profile
            {
                Id = profile.Id,
                UserId = profile.UserId,
                Description = profile.Description,
                ImagePath = profile.ImagePath,
                Abilites = new List<string>(profile.Abilites),
            });
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
            // deep clone to avoid shared read references
            return this.messages.ConvertAll(message => new Message
            {
                Id = message.Id,
                FromUserId = message.Id,
                ToUserId = message.ToUserId,
                Text = message.Text,
                Seen = message.Seen,
            });
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
