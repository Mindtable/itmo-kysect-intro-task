using System.Text.Json;
using console_task_manager.exception;
using console_task_manager.model;
using console_task_manager.service;
using console_task_manager.utility;

namespace console_task_manager.controller;

public class TaskManagerController
{

    private TaskUnitService _service;

    public TaskManagerController(TaskUnitService service)
    {
        _service = service;
    }

    public bool ExecuteCommand(string[] command)
    {
        if (command.Length == 0)
        {
            throw new WrongInputException();
        }
        
        switch (command[0])
        {
            case "/list-tasks":
            {
                var idList = _service.GetAllTasksIds();
                if (idList.Count == 0)
                {
                    throw new TaskUnitNotFoundException("Task is not found");
                }
                Print(idList);
                break;
            }
            case "/list-groups":
            {
                var groupList = _service.GetAllGroups();
                
                if (groupList.Count == 0)
                {
                    throw new TaskUnitNotFoundException("Group is not found");
                }
                
                foreach (var group in groupList)
                {
                    Print(group.Id);
                }

                break;
            }
            case "/list-today-tasks":
            {
                foreach (var task in _service.GetAllExpiringTasks())
                {
                    Console.WriteLine(task);
                }

                break;
            }
            case "/list-completed-tasks":
            {
                var tasks = _service.GetAllCompletedTaskUnit();
                
                if (tasks.Count == 0)
                {
                    throw new TaskUnitNotFoundException("Task is not found");
                }
                
                foreach (var task in tasks)
                {
                    Print(task.Id);
                }

                break;
            }
            case "/add-task":
            {
                if (command.Length == 1)
                {
                    throw new WrongInputException();
                }
                
                var description = string.Join(" ", command.Skip(1).ToList());
                
                var response = _service.Add(description);

                Console.WriteLine($"Task w/ id = {response} successfully created!");
                break;
            }
            case "/add-subtask":
            {
                if (command.Length is 1 or 2)
                {
                    throw new WrongInputException();
                }
                
                var parentId = long.Parse(command[1]);
                var parentTask = _service.GetById(parentId);

                if (parentTask is null)
                {
                    throw new TaskUnitNotFoundException("Parent not found");
                }
                
                var description = string.Join(" ", command.Skip(2).ToList());
            
                var response = _service.AddSubtask(parentId, description);

                Console.WriteLine($"Subtask w/ id = {response} successfully created!");
                
                MakeTaskUnfinished(parentId);
                
                break;
                }
            case "/add-group":
            {
                if (command.Length == 1)
                {
                    throw new WrongInputException();
                }
                
                var description = string.Join(" ", command.Skip(1).ToList());
                
                var response = _service.AddGroup(description);

                Console.WriteLine($"Group w/ id = {response} successfully created!");
                break;
            }
            case "/get":
            {
                if (command.Length == 1)
                {
                    throw new WrongInputException();
                }

                var id = long.Parse(command[1]);
                var task = _service.GetById(id);
                if (task == null)
                {
                    Console.WriteLine("Task not found!");
                }
                else
                {
                    Print(id);
                }

                break;
            }
            case "/get-by-description":
            {
                if (command.Length == 1)
                {
                    throw new WrongInputException();
                }
                var description = string.Join(" ", command.Skip(1).ToList());

                var taskUnitList = _service.GetAllByDescription(description);

                if (taskUnitList.Count == 0)
                {
                    throw new TaskUnitNotFoundException("Task is not found");
                }

                foreach (var task in taskUnitList)
                {
                    Print(task.Id);
                }

                break;
            }
            case "/finish":
            {
                if (command.Length == 1)
                {
                    throw new WrongInputException();
                }
                
                var id = long.Parse(command[1]);
                var task = _service.GetById(id);

                if (task is null)
                {
                    throw new TaskUnitNotFoundException("Task is not found");
                }

                if (task.Complete)
                {
                    throw new TaskUnitLogicException("Task is already finished");
                }
                
                if (IsAllSubtasksFinished(id))
                {
                    _service.ChangeTaskComplete(id, true);
                    UpdateParentCompleteStatus(task.ParentId);
                }
                else
                {
                    throw new TaskUnitLogicException("You must complete all subtasks before finishing parent task");
                }
                break;

            }
            case "/delete-by-id":
            {
                if (command.Length == 1)
                {
                    throw new WrongInputException();
                }
                
                var id = long.Parse(command[1]);
                
                Delete(id);

                Console.WriteLine($"TaskUnit w/ id = {id} successfully deleted.");
                
                break;
            }
            case "/set-deadline":
            {
                if (command.Length is 1 or 2)
                {
                    throw new WrongInputException();
                }

                var id = long.Parse(command[1]);
                DateOnly.Parse(command[2]);
                var task = _service.GetById(id);

                if (task is null)
                {
                    throw new TaskUnitNotFoundException("Task is not found");
                }

                _service.SetDeadline(id, command[2]);

                break;
            }
            case "/exit":
            {
                return true;
            }
            case "/save":
            {
                if (command.Length == 1)
                {
                    throw new WrongInputException();
                }
                
                _service.SaveAsJson(command[1]);

                Console.WriteLine("All data have been successfully saved.");
                break;
            }
            case "/load":
            {
                if (command.Length == 1)
                {
                    throw new WrongInputException();
                }
                
                _service.LoadFromJson(command[1]);

                Console.WriteLine("All data have been successfully loaded.");
                break;
            }
            case "/help":
            {
                Console.WriteLine("/list-tasks");
                Console.WriteLine("/list-groups");
                Console.WriteLine("/list-today-tasks");
                Console.WriteLine("/list-completed-tasks");
                Console.WriteLine("/add-task {taskInfo}");
                Console.WriteLine("/add-subtask {parentId} {taskInfo}");
                Console.WriteLine("/add-group {groupDescription}");
                Console.WriteLine("/get {taskId}");
                Console.WriteLine("/get-by-description {taskInfo}");
                Console.WriteLine("/finish {id}");
                Console.WriteLine("/delete-by-id {id}");
                Console.WriteLine("/set-deadline {id} {dateOnly}");
                Console.WriteLine("/exit");
                Console.WriteLine("/save {fileName}");
                Console.WriteLine("/load {fileName}" );
                break;
            }
            default:
            {
                throw new WrongInputException();
            }
        }

        return false;
    }

    private void Print(long id)
    {
        var listedTasks = new HashSet<long>();
        PrintTasks(id, listedTasks);
    }

    private void Print(List<long> idList)
    {
        var listedTasks = new HashSet<long>();
        foreach (var id in idList)
        {
            PrintTasks(id, listedTasks);
        }
    }

    private void PrintTasks(long id, HashSet<long> listed, string intend = "")
    {
        if (listed.Contains(id))
        {
            return;
        }

        listed.Add(id);
        Console.WriteLine(intend + _service.GetById(id));
        foreach (var child in _service.GetAllChildren(id))
        {
            PrintTasks(child.Id, listed, (intend == "" ? "  - " : "    " + intend));
        }
    }

    private void Delete(long id)
    {
        var children = _service.GetAllChildren(id);
        _service.Delete(id);
        foreach (var child in children)
        {
            Delete(child.Id);
        }
    }

    private bool IsAllSubtasksFinished(long id)
    {
        var children = _service.GetAllChildren(id);
        if (children.All(item => item.Complete))
        {
            foreach (var child in children)
            {
                if (!IsAllSubtasksFinished(child.Id))
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    private void MakeTaskUnfinished(long? id)
    {
        if (id is null)
        {
            return;
        }
        _service.ChangeTaskComplete(id.Value, false);
        var parent = _service.GetById(id.Value);
        if (parent is not null)
        {
            MakeTaskUnfinished(parent.ParentId);
        }
    }
    
    private void UpdateParentCompleteStatus(long? id)
    {
        if (id is null || !IsAllSubtasksFinished(id.Value))
        {
            return;
        }
        _service.ChangeTaskComplete(id.Value, true);
        var parent = _service.GetById(id.Value);
        if (parent is not null)
        {
            MakeTaskUnfinished(parent.ParentId);
        }
    }
}