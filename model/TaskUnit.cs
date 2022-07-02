using console_task_manager.exception;

namespace console_task_manager.model;

public class TaskUnit
{
    public long Id { get; set; }
    public long? ParentId { get; set; }
    public string Description { get; set; }
    public TaskType Type { get; set; }
    public bool Complete { get; set; }
    
    public string? Deadline { get; set; }

    public TaskUnit(long id, long? parentId, string description, TaskType type, bool complete)
    {
        Id = id;
        ParentId = parentId;
        Description = description;
        Type = type;
        Complete = complete;
    }

    public override string ToString()
    {
        if (Type == TaskType.Task)
        {
            return $"[{(Complete ? "x" : " ")}] " +
                   $"{(Deadline is null ? "" : $"{{{DateOnly.Parse(Deadline)}}} ")}" +
                   $"{{{Id}}} " +
                   $"{Description}";
        }
        if (Type == TaskType.Collection)
        {
            return $"[{(Complete ? "x" : " ")}] " +
                   $"{(Deadline is null ? "" : $"{{{DateOnly.Parse(Deadline)}}} ")}" +
                   $"{{{Id}}} " +
                   $"==={Description}===";
        }

        throw new UnsupportedTaskUnitTypeException(Type + " is unsupported");
    }
}