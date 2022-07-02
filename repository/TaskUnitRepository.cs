using System.Collections.Immutable;
using System.Globalization;
using System.Text.Json;
using console_task_manager.model;
using console_task_manager.utility;
using console_task_manager.validation;

namespace console_task_manager.repository;

public class TaskUnitRepository
{
    private List<TaskUnit> _mData;

    public TaskUnitRepository(string? fileName = null)
    {
        try
        {
            string jsonString = File.ReadAllText(fileName);
            _mData = JsonSerializer.Deserialize<List<TaskUnit>>(jsonString)!;
        }
        catch (Exception ex) when 
            (ex is FileNotFoundException or ArgumentNullException)
        {
            _mData = new List<TaskUnit>();
        }
    }

    public TaskUnit? FindById(long id)
    {
        return _mData.Find(task => task.Id == id);
    }

    public void Save(TaskUnit task)
    {
        var existingTaskUnit = FindById(task.Id);

        if (existingTaskUnit is null)
        {
            _mData.Add(task);
        }
        else if (TaskUnitValidator.ValidateTaskUnitForUpdate(existingTaskUnit, task))
        {
            UpdateExistingTaskUnit(existingTaskUnit, task);
        }
    }

    public List<TaskUnit> FindAll()
    {
        return _mData;
    }

    public List<TaskUnit> FindAllCompletedTaskUnit()
    {
        return _mData
            .FindAll(item => item.Complete)
            .FindAll(item => item.ParentId is null)
            .FindAll(item => item.Type is TaskType.Task)
            .ToList();
    }

    public List<TaskUnit> FindAllByDescription(string description)
    {
        return _mData.FindAll(item => item.Description == description).ToList();
    }

    public List<TaskUnit> FindAllExpiringTasks()
    {
        var today = DateOnly.FromDateTime(DateTime.Today).ToString(new CultureInfo("ru-RU"));
        return _mData.FindAll(item => item.Deadline == today).ToList();
    }

    public void DeleteById(long id)
    {
        _mData.RemoveAll(item => item.Id == id);
    }

    public List<TaskUnit> FindAllChildren(long parentId)
    {
        return _mData.FindAll(item => item.ParentId == parentId).ToList();
    }

    public List<long> FindAllTasksIds()
    {
        return _mData.FindAll(item => item.Type == TaskType.Task).Select(item => item.Id).ToList();
    }

    public List<TaskUnit> FindAllGroups()
    {
        return _mData.FindAll(item => item.Type == TaskType.Collection).ToList();
    }
    
    public void WriteAllTasksAsJson(string fileName)
    {
        JsonWriter.SaveAsJson(fileName, _mData);
    }

    public void ReadAllTasksFromJson(string fileName)
    {
        string jsonString = File.ReadAllText(fileName);
        _mData = JsonSerializer.Deserialize<List<TaskUnit>>(jsonString)!;
    }

    private static void UpdateExistingTaskUnit(TaskUnit existingTaskUnit, TaskUnit newTaskUnit)
    {
        existingTaskUnit.Description = newTaskUnit.Description;
        existingTaskUnit.Complete = newTaskUnit.Complete;
        existingTaskUnit.ParentId = newTaskUnit.ParentId;
    }
    
}