namespace Application.Exceptions;

public class SavingGoalNotFoundException : Exception
{
    public SavingGoalNotFoundException() : base()
    {
    }

    public SavingGoalNotFoundException(string message) : base(message)
    {
    }

    public SavingGoalNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}