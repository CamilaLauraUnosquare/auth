namespace Libelula.Authentication.Controllers.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class ApiErrorResult : ApiBasicResult
{
    public ApiErrorResult(IEnumerable<Error> errors, string message)
       : base(false, message) => this.Errors = errors;

    public ApiErrorResult(ModelStateDictionary modelState, string bodyString)
   : base(false, ControllerConstants.RequestError)
    {
        this.Errors = modelState.Select(x => new Error(x, bodyString)).ToList();
    }

    public IEnumerable<Error>? Errors { get; init; }
}