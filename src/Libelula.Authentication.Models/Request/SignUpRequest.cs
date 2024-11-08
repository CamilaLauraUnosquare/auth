namespace Libelula.Authentication.Models.Request;
using Libelula.Authentication.Models.Enums;

public class SignUpRequest
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}