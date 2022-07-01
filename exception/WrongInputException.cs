namespace console_task_manager.exception;

public class WrongInputException : Exception
{
    public WrongInputException()
        : base("Incorrect input! Type /help for more info") {}
}