namespace Shared;

public class Message
{
    public int Id { get; set; }
    public int FromUserId { get; set; }
    public int ToUserId { get; set; }
    public string Text { get; set; } = "";
    public bool? Seen { get; set; }
}
