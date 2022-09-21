//namespace Shared;

public class Message
{
    public int Id { get; set; }
    public int FromUserId { get; set; }
    public int ToUserId { get; set; }
    public string Text { get; set; } = "";
    public bool Seen { get; set; } = false;

    public static string Encoder(Message message)
    {
        return $"{message.Id}|{message.FromUserId}|{message.ToUserId}|{message.Text}|{message.Seen}";
    }

    public static Message Decoder(string encoded)
    {
        string[] parts = encoded.Split("|");

        return new Message()
        {
            Id = Int32.Parse(parts[0]),
            FromUserId = Int32.Parse(parts[1]),
            ToUserId = Int32.Parse(parts[2]),
            Text = parts[3],
            Seen = Boolean.Parse(parts[4]),
        };
    }
}
