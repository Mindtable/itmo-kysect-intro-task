using System.Collections.Immutable;
using console_task_manager.model;
using console_task_manager.repository;
using console_task_manager.utility;

namespace console_task_manager.service;

public class TaskUnitService
{

    private TaskUnitRepository _repository;
    private IdGenerator _idGenerator;

    public TaskUnitService(TaskUnitRepository repository, IdGenerator idGenerator)
    {
        _repository = repository;
        _idGenerator = idGenerator;
    }

    public long Add(string description)
    {
        var task = new TaskUnit(
            _idGenerator.Generate(),
            null,
            description,
            TaskType.Task,
            false
        );
        
        _repository.Save(task);
        return task.Id;
    }

    public long AddSubtask(long parentId, string description)
    {
        var task = new TaskUnit(
            _idGenerator.Generate(),
            parentId,
            description,
            TaskType.Task,
            false
        );
        
        _repository.Save(task);
        return task.Id;
    }

    public void Delete(long id)
    {
        _repository.DeleteById(id);
    }

    public long AddGroup(string description)
    {
        var task = new TaskUnit(
            _idGenerator.Generate(),
            null,
            description,
            TaskType.Collection,
            false
        );
        
        _repository.Save(task);
        return task.Id;
    }

    public List<TaskUnit> GetAll()
    {
        return _repository.FindAll();
    }

    public List<TaskUnit> GetAllByDescription(string description)
    {
        return _repository.FindAllByDescription(description);
    }

    public List<TaskUnit> GetAllChildren(long id)
    {
        return _repository.FindAllChildren(id);
    }

    public List<TaskUnit> GetAllCompletedTaskUnit()
    {
        return _repository.FindAllCompletedTaskUnit();
    }

    public List<long> GetAllTasksIds()
    {
        return _repository.FindAllTasksIds();
    }

    public List<TaskUnit> GetAllGroups()
    {
        return _repository.FindAllGroups();
    }

    public List<TaskUnit> GetAllExpiringTasks()
    {
        return _repository.FindAllExpiringTasks();
    }

    public void SaveAsJson(string fileName)
    {
        _repository.WriteAllTasksAsJson(fileName);
    }

    public void LoadFromJson(string fileName)
    {
        _repository.ReadAllTasksFromJson(fileName);
    }

    public TaskUnit? GetById(long id)
    {
        return _repository.FindById(id);
    }

    public void ChangeTaskComplete(long id, bool status)
    {
        _repository.FindById(id).Complete = status;
    }

    public void SetDeadline(long id, string deadline)
    {
        _repository.FindById(id).Deadline = deadline;
    }
}