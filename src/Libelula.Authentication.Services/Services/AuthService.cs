namespace Libelula.Authentication.Services.Services;
using System;
using FluentValidation.Results;
using Libelula.Authentication.DataAccess.Interfaces;
using System.Security.Claims;
using System.Security.Cryptography;
using Libelula.Authentication.Models;
using Libelula.Authentication.Models.Request;
using Libelula.Authentication.Models.Responses;
using Libelula.Authentication.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using ValidationFailure = FluentValidation.Results.ValidationFailure;
using Libelula.Authentication.Services.Validators;
using Libelula.Authentication.Services.Models;
using System.ComponentModel.DataAnnotations;

public class AuthService : IAuthService
{
    private readonly IAuthRepository authRepository;
    private readonly AuthSettings authSettings;

    public AuthService(IAuthRepository authRepository, AuthSettings authSettings)
    {
        this.authRepository = authRepository;
        this.authSettings = authSettings;
    }

    public async Task<ServiceResult<User>> RegisterUser(SignUpRequest newUser)
    {
        RegisterValidator validationRules = new RegisterValidator(this.authRepository);
        FluentValidation.Results.ValidationResult result = await validationRules.ValidateAsync(newUser);

        if (!result.IsValid)
        {
            return result.Errors;
        }

        using var hmac = new HMACSHA512();
        byte[] passwordSalt = hmac.Key;
        byte[] passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(newUser.Password));
        User user = new (newUser.FirstName, newUser.LastName, newUser.Email, passwordHash, passwordSalt);
        await this.authRepository.RegisterUser(user);
        return user;
    }

    public async Task<ServiceResult<LoginResponse>> Login(LoginRequest login, RefreshToken refreshToken)
    {
        List<ValidationFailure>? error = await this.Validate(login);

        if (error != null)
        {
            return error;
        }

        User? user = await this.GetUser(login.Email) !;
        await this.SetRefreshToken(user!, refreshToken);
        return new LoginResponse(login.Email, await this.CreateToken(user!));
    }

    public async Task<string> CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.FirstName + user.LastName!.ToString())
        };
        SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
            .GetBytes(this.authSettings.Secret));

        var creds = new SigningCredentials(key, SecurityAlgorithms.Aes128CbcHmacSha256);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.ToLocalTime().AddMinutes(60),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return await Task.Run(() => jwt);
    }

    public async Task SetRefreshToken(User user, RefreshToken newRefreshToken)
    {
        user.RefreshToken = newRefreshToken.Token;
        user.TokenExpires = newRefreshToken.Expires;
        user.TokenCreated = newRefreshToken.Created;
        await this.authRepository.UpdateUser(user);
    }

    public async Task<User?> GetUser(string email)
    {
        return await this.authRepository.GetUser(email);
    }

    public async Task<List<ValidationFailure>?> Validate(LoginRequest login)
    {
        AuthValidator validator = new (this.authRepository);
        FluentValidation.Results.ValidationResult result = await validator.ValidateAsync(login);
        return !result.IsValid ? result.Errors : null;
    }
}