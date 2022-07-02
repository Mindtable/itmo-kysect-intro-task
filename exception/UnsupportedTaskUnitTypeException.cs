namespace console_task_manager.exception;

public class UnsupportedTaskUnitTypeException : Exception
{
    public UnsupportedTaskUnitTypeException(string? message) : base(message)
    {
    }
}