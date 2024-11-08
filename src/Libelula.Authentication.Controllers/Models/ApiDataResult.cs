namespace Libelula.Authentication.Controllers.Models;

public class ApiDataResult<TResult> : ApiBasicResult
            where TResult : class
{
    public ApiDataResult(TResult data)
        : base(true, ControllerConstants.OkMessage)
    {
        this.Data = data;
    }

    public TResult? Data { get; init; }
}