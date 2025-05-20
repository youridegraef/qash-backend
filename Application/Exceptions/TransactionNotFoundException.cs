namespace Application.Exceptions;

public class TransactionNotFoundException : Exception
{
    public TransactionNotFoundException() : base()
    {
    }

    public TransactionNotFoundException(string message) : base(message)
    {
    }

    public TransactionNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}