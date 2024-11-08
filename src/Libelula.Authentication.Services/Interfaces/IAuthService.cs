namespace Libelula.Authentication.Services.Interfaces;
using Libelula.Authentication.Models;
using Libelula.Authentication.Models.Request;
using Libelula.Authentication.Models.Responses;
using Libelula.Authentication.Services.Models;

public interface IAuthService
{
    public Task<ServiceResult<User>> RegisterUser(SignUpRequest userR);

    public Task<ServiceResult<LoginResponse>> Login(LoginRequest login, RefreshToken refreshToken);

    public Task<User?> GetUser(string email);
}