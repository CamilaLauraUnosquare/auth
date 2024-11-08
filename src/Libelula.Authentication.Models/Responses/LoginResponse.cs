namespace Libelula.Authentication.Models.Responses;
using System.Data;
using Libelula.Authentication.Models.Enums;

public class LoginResponse
{
    public LoginResponse(string email, string jwtToken)
    {
        this.JwtToken = jwtToken;
        this.Email = email;
    }

    public string JwtToken { get; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime LoginDate { get; set; } = DateTime.Now;
}