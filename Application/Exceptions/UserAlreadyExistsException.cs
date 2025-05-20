namespace Application.Exceptions;

public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException() : base() { }
    public UserAlreadyExistsException(string message) : base(message) { }
    public UserAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
}