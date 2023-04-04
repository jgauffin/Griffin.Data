namespace Griffin.Data.Tests.Entities;

public class Log
{
    public Log(string message)
    {
        Message = message;
        CreatedAtUtc = DateTime.UtcNow;
    }

    protected Log()
    {
    }

    public int MainId { get; set; }

    public DateTime CreatedAtUtc { get; }

    public string Message { get; }
}