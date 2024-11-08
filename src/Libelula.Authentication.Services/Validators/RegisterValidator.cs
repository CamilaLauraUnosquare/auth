namespace Libelula.Authentication.Services.Validators;

using System.Text.RegularExpressions;
using FluentValidation;
using Libelula.Authentication.DataAccess.Interfaces;
using Libelula.Authentication.Models;
using Libelula.Authentication.Models.Request;

public class RegisterValidator : AbstractValidator<SignUpRequest>
{
    public RegisterValidator(IAuthRepository authRepository)
    {
        this.RuleFor(user => user.FirstName)
            .NotEmpty()
            .NotNull()
            .MaximumLength(100)
            .MinimumLength(3)
            .WithMessage("{PropertyName} cannot be null or empty");

        this.RuleFor(user => user.LastName)
            .NotEmpty()
            .NotNull()
            .MaximumLength(100)
            .MinimumLength(3)
            .WithMessage("{PropertyName} cannot be null or empty");

        this.RuleFor(user => user.Email)
            .NotEmpty()
            .NotNull()
            .WithMessage("{PropertyName} cannot be null or empty");

        this.RuleFor(user => user)
            .Must(user => authRepository.GetUser(user.Email) !.Result is null)
            .WithMessage("User already exists");

        this.RuleFor(user => user)
            .Must(user =>
            {
                string password = user.Password;

                Regex hasNumber = new Regex(@"[0-9]+");
                Regex hasUpperChar = new Regex(@"[A-Z]+");
                Regex hasMinimum8Chars = new Regex(@".{8,}");

                bool isValid = hasNumber.IsMatch(password) && hasUpperChar.IsMatch(password) && hasMinimum8Chars.IsMatch(password);
                return isValid;
            })
            .WithMessage("Must have symbols, numbers and upper case characters");
    }
}