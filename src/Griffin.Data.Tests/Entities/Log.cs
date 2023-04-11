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
        Message = "";
    }

    public DateTime CreatedAtUtc { get; }
    public int Id { get; set; }

    public int MainId { get; set; }

    public string Message { get; }
}
