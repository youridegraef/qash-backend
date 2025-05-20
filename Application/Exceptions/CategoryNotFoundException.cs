namespace Application.Exceptions;

public class CategoryNotFoundException : Exception
{
    public CategoryNotFoundException() : base()
    {
    }

    public CategoryNotFoundException(string message) : base(message)
    {
    }

    public CategoryNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}