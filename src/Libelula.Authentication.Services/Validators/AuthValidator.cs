namespace Libelula.Authentication.Services.Validators;

using System.Security.Cryptography;
using FluentValidation;
using Libelula.Authentication.DataAccess.Interfaces;
using Libelula.Authentication.Models;
using Libelula.Authentication.Models.Request;
using Libelula.Authentication.Services.Models;

public class AuthValidator : AbstractValidator<LoginRequest>
{
    public AuthValidator(IAuthRepository authRepository)
    {
        this.RuleFor(user => user.Email)
            .NotEmpty().WithMessage("'{PropertyName}' mustn't empty")
            .MaximumLength(150).WithMessage("'{PropertyName}' must contain less than 15 letters.")
            .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").WithMessage("'{PropertyName}' must be an email")
            .Must(username => authRepository.GetUser(username) != null)
            .WithMessage("Invalid Credential");

        this.RuleFor(user => user.Password)
            .NotEmpty().WithMessage("'{PropertyName}' must be not empty")
            .MinimumLength(6).WithMessage("'{PropertyName}' must contain at least 6 characters.")
            .MaximumLength(50).WithMessage("'{PropertyName}' must contain at most 50 characters.")
            .WithErrorCode(ServiceConstants.InvalidCredentials);

        this.RuleFor(user => user)
         .Must(user => authRepository.GetUser(user.Email) !.Result is not null)
         .WithMessage("User not found");

        this.RuleFor(user => user)
            .Must(user =>
            {
                User? userFound = authRepository.GetUser(user.Email) !.Result;
                return this.VerifyPasswordHash(userFound!, user.Password);
            })
            .WithMessage("Invalid Credentials");
    }

    private bool VerifyPasswordHash(User? user, string password)
    {
        if (user is null)
        {
            return false;
        }

        using (var hmac = new HMACSHA512(user.PasswordSalt!))
        {
            var loginPassword = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] computeHash = hmac.ComputeHash(loginPassword);
            bool isUserPassword = computeHash.SequenceEqual(user.PasswordHash!);
            return isUserPassword;
        }
    }
}