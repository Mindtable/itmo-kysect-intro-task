using console_task_manager.model;

namespace console_task_manager.validation;

public class TaskUnitValidator
{
    public static bool ValidateTaskUnitForUpdate(TaskUnit oldTaskUnit, TaskUnit newTaskUnit)
    {
        return oldTaskUnit.Type == newTaskUnit.Type;
    }
}