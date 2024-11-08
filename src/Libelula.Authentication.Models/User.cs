namespace Libelula.Authentication.Models;
using System;
using Libelula.Authentication.Models.Enums;

public class User
{
    public User(string firstName, string lastName, string email, byte[] passwordHash, byte[] passwordSalt)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
        this.Email = email;
        this.PasswordHash = passwordHash;
        this.PasswordSalt = passwordSalt;
    }

    public User()
    {
    }

    public int? UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public byte[]? PasswordHash { get; set; }

    public byte[]? PasswordSalt { get; set; }

    public string? RefreshToken { get; set; } = string.Empty;

    public DateTime? TokenCreated { get; set; }

    public DateTime? TokenExpires { get; set; }
}