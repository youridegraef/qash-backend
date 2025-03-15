namespace Application.Exceptions;

public class InvalidEmailFormatException : Exception
{
    public InvalidEmailFormatException(string message) : base(message) { }
}