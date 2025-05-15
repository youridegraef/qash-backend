using Microsoft.Extensions.Logging;

namespace TestLayer.MockData;

public class DummyLogger<T> : ILogger<T>
{
    public IDisposable BeginScope<TState>(TState state) => null!;
    public bool IsEnabled(LogLevel logLevel) => false;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
    }
}