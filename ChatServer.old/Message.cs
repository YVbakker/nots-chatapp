namespace ChatServer;

public class Message
{
    public char delimiter { get; set; } = char.Parse(";");
    public string header { get; set; }
    public string body { get; set; }
}