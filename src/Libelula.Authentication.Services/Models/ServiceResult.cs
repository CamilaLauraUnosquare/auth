namespace Libelula.Authentication.Services.Models;
using System;
using FluentValidation.Results;

public class ServiceResult<TResult>
        where TResult : class
{
    public TResult? Result { get; init; }

    public IEnumerable<ValidationFailure>? Errors { get; init; }

    public bool IsValid { get; init; }

    public static implicit operator ServiceResult<TResult>(TResult result) => new ()
    {
        Result = result,
        IsValid = true,
    };

    public static implicit operator ServiceResult<TResult>(List<ValidationFailure> errors) => new ()
    {
        Errors = errors,
        IsValid = false,
    };
}