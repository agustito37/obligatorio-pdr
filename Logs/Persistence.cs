using Shared;
using Shared.domain;

public sealed class Persistence
{
    // singleton
    public static Persistence Instance { get; } = new Persistence();

    private readonly List<Log> logs = new();

    public Persistence()
    {
    }

    public List<Log> GetLogs()
    {
        lock (this.logs)
        {
            // deep clone to avoid shared read references
            return this.logs.ConvertAll(log => new Log
            {
                Date = log.Date,
                Type = log.Type,
                Message = log.Message
            });
        }
    }

    public void AddLog(Log log)
    {
        lock (this.logs)
        {
            this.logs.Add(log);
        }
    }
}