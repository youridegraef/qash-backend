namespace Application.Exceptions;

public class UserRetrievalException : Exception
{
    public UserRetrievalException() : base() { }
    public UserRetrievalException(string message) : base(message) { }
    public UserRetrievalException(string message, Exception innerException) : base(message, innerException) { }
}