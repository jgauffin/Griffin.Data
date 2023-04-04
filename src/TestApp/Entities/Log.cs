namespace TestApp.Entities;

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

    public int Id { get; set; }
    public int MainId { get; set; }

    public DateTime CreatedAtUtc { get; }

    public string Message { get; }
}