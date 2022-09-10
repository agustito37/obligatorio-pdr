public class Profile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public List<string> Abilites { get; set; } = new();
    public string Description { get; set; } = "";
    public string? ImagePath { get; set; }
}
