namespace Application.Exceptions;

public class RegistrationFailedException : Exception
{
    public RegistrationFailedException() : base() { }
    public RegistrationFailedException(string message) : base(message) { }
    public RegistrationFailedException(string message, Exception innerException) : base(message, innerException) { }
}