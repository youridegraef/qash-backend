namespace Application.Exceptions;

public class CalculationException : Exception
{
    public CalculationException() : base() { }
    public CalculationException(string message) : base(message) { }
    public CalculationException(string message, Exception innerException) : base(message, innerException) { }
}