namespace Libelula.Authentication.DataAccess.Interfaces;
using Libelula.Authentication.Models;

public interface IAuthRepository
{
    public Task<User?> GetUser(string userEmail);

    public Task RegisterUser(User user);

    public Task UpdateUser(User user);
}