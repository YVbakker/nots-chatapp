using System.Text;

namespace ChatServer;

public class Parser
{
    private StringBuilder sb { get; set; }
    private int _bufferSize { get; set; }

    Parser(int bufferSize)
    {
        sb = new();
        _bufferSize = bufferSize;
    }

    Message Parse(Byte[] bytes, int bufferSize)
    {
        for (var i = 0; i < bytes.Length; i += bufferSize)
        {
            sb.Append(Encoding.ASCII.GetString(bytes, i, bufferSize));
        }

        var rawMessage = sb.ToString();
        Message message = new Message();
        foreach (var c in rawMessage)
        {
            if (c.Equals(message.delimiter))
            {
                //substring
            }
        }

        return message;
    }
}