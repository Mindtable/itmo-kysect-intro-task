using System.Text.Json;
using console_task_manager.controller;
using console_task_manager.exception;
using console_task_manager.repository;
using console_task_manager.service;
using console_task_manager.utility;

namespace console_task_manager;

public class Application
{
    private TaskManagerController _controller;
    private TaskUnitRepository _repository;
    private IdGenerator _idGenerator;

    public Application()
    {
        _repository = new TaskUnitRepository("data.json");

        long startId = 1;

        try
        {
            string jsonString = File.ReadAllText("config.json");
            var configuration = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString)!;
            startId = long.Parse(configuration["startId"]);
        }
        catch (Exception ex) when
            (ex is FileNotFoundException or JsonException or KeyNotFoundException or FormatException)
        { }
        
        _idGenerator = new IdGenerator(startId);
        var service = new TaskUnitService(_repository, _idGenerator);
        _controller = new TaskManagerController(service);
    }

    public void Run()
    {
        while (true)
        {
            Console.Write("task-manager#= ");
            try
            {
                string[] input = Console.ReadLine().Split();
                if (_controller.ExecuteCommand(input))
                {
                    return;
                }

                ;
            }
            catch (Exception ex) when
                (ex is WrongInputException or
                    FormatException or
                    UnsupportedTaskUnitTypeException or
                    TaskUnitNotFoundException or
                    FileNotFoundException or
                    TaskUnitLogicException)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public void Persist(string dataFileName = "data.json", string configFileName = "config.json")
    {
        _repository.WriteAllTasksAsJson(dataFileName);

        var lastId = _idGenerator.GetState();

        var config = new Dictionary<string, string>
        {
            ["startId"] = lastId.ToString()
        };

        JsonWriter.SaveAsJson(configFileName, config);
    }
}