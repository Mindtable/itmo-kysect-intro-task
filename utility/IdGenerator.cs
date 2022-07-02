using System.Text.Json;

namespace console_task_manager.utility;

public class IdGenerator
{
    private long _id;

    public IdGenerator(long startId)
    {
        _id = startId;
    }

    public long Generate()
    {
        return _id++;
    }

    public long GetState()
    {
        return _id;
    }
}