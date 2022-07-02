using System.Text.Json;

namespace console_task_manager.utility;

public class JsonWriter
{
    public static void SaveAsJson<TValue>(string fileName, TValue value)
    {
        string jsonString = JsonSerializer.Serialize(value);
        File.WriteAllText(fileName, jsonString);
    }
}