namespace Libelula.Authentication.Controllers.Models;

public class ApiBasicResult
{
    public ApiBasicResult(bool isSuccessful, string message)
    {
        this.IsSuccessful = isSuccessful;
        this.Message = message;
    }

    public ApiBasicResult(Exception? exception)
    {
        this.Message = exception is null ? ControllerConstants.NullExceptionMessage : $"{exception.Message}\n\n{exception.StackTrace}";
    }

    public bool IsSuccessful { get; init; }

    public string Message { get; init; } = string.Empty;
}