public class Profile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public List<string> Abilites { get; set; } = new();
    public List<string> Keywords { get; set; } = new();
    public string ImagePath { get; set; } = "";
}
