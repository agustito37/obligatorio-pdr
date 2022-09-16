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

    public void AddUser(User user)
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

    public List<Profile> GetProfiles()
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

    public List<Message> GetMessages(int userId)
    {
        lock (this.messages)
        {
            // deep clone to avoid shared read references
            return this.messages.ConvertAll(message => new Message
            {
                Id = message.Id,
                FromUserId = message.FromUserId,
                ToUserId = message.ToUserId,
                Text = message.Text,
                Seen = message.Seen,
            }).FindAll((i) => i.FromUserId == userId);
        }
    }

    public void SetSeenMessages(List<int> seenIds)
    {
        lock (this.messages)
        {
            foreach (Message message in this.messages.FindAll((i) => seenIds.Contains(i.Id))) {
                message.Seen = true;
            }
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
}
