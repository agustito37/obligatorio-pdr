namespace Shared;

public class Profile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Description { get; set; } = "";
    public string ImagePath { get; set; } = "";
    public List<string> Abilites { get; set; } = new();

    public static string Encoder(Profile profile)
    {
        return $"{profile.Id}|{profile.UserId}|{profile.Description}|{profile.ImagePath}|{String.Join("^", profile.Abilites.ToArray())}";
    }

    public static Profile Decoder(string encoded)
    {
        string[] parts = encoded.Split("|");

        return new Profile()
        {
            Id = Int32.Parse(parts[0]),
            UserId = Int32.Parse(parts[1]),
            Description = parts[2],
            ImagePath = parts[3],
            Abilites = new List<string>(parts[4].Split("^"))
        };
    }
}
