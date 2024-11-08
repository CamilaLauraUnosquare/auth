namespace Libelula.Authentication.Controllers.Models;
using System;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class Error
{
    public Error(ValidationFailure error)
    {
        this.PropertyName = error.PropertyName;
        this.ErrorMessage = error.ErrorMessage;
        this.AttemptedValue = error.AttemptedValue.ToString() ?? ControllerConstants.NoValueProvidedMessage;
    }

    public Error(KeyValuePair<string, ModelStateEntry?> pair, string attemptedValue)
    {
        string key = pair.Key.ToString();
        this.PropertyName = key.Equals("$") ? "Request body" : key;
        this.ErrorMessage = pair.Value is not null ? pair.Value.Errors[0].ErrorMessage : ControllerConstants.NoValueProvidedMessage;
        this.AttemptedValue = attemptedValue.ToString() ?? ControllerConstants.NoValueProvidedMessage;
    }

    public string PropertyName { get; init; }

    public string ErrorMessage { get; init; }

    public string AttemptedValue { get; init; }
}