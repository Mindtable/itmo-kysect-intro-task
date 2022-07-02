// See https://aka.ms/new-console-template for more information

namespace console_task_manager;

class Program
{
    public static void Main(string[] args)
    {
        var application = new Application();
        application.Run();
        application.Persist();
    }
}