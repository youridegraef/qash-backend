namespace Application.Exceptions;

public class TagNotFoundException : Exception
{
    public TagNotFoundException() : base()
    {
    }

    public TagNotFoundException(string message) : base(message)
    {
    }

    public TagNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}