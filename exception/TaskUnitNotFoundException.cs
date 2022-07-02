namespace console_task_manager.exception;

public class TaskUnitNotFoundException : Exception
{
    public TaskUnitNotFoundException(string? message) : base(message)
    { }
}