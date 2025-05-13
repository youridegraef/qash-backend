namespace Application.Exceptions;

public class BudgetNotFoundException : Exception
{
    public BudgetNotFoundException() : base()
    {
    }

    public BudgetNotFoundException(string message) : base(message)
    {
    }

    public BudgetNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}