namespace Libelula.Authentication.Models;

public class SQLConnection
{
    public string Database { get; init; } = "Libelula_Auth_Conciliations";

    public string? Server { get; init; } = string.Empty;

    public string? User { get; init; } = string.Empty;

    public string? Password { get; init; } = string.Empty;

    public string DefaultConnection => $"Server={this.Server ?? string.Empty};Database={this.Database ?? string.Empty};User Id={this.User ?? string.Empty};Password={this.Password ?? string.Empty};";
}