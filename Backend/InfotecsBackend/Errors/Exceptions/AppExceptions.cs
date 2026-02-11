namespace InfotecsBackend.Errors.Exceptions;

public abstract class AppException : Exception
{
    public int StatusCode { get; }
    
    protected AppException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class DeviceNotFoundException : AppException
{
    public DeviceNotFoundException(Guid deviceId) 
        : base($"Device {deviceId} not found", 404) { }
}

public class SessionNotFoundException : AppException
{
    public SessionNotFoundException(Guid sessionId) 
        : base($"Session {sessionId} not found", 404) { }
}

public class ValidationException : AppException
{
    public ValidationException(string message) 
        : base(message, 400) { }
}