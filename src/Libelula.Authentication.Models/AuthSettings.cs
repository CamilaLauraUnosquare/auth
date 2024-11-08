namespace Libelula.Authentication.Models;

public class AuthSettings
{
    public AuthSettings(string secret)
    {
        this.Secret = secret;
    }

    public string Secret { get; set; }
}