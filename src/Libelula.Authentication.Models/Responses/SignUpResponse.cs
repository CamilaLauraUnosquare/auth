namespace Libelula.Authentication.Models.Responses;

using Libelula.Authentication.Models.Enums;

public class SignUpResponse
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}