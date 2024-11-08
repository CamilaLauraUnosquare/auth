namespace Libelula.Authentication.Controllers.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Libelula.Authentication.Controllers.Models;
using Libelula.Authentication.Models;
using Libelula.Authentication.Models.Request;
using Libelula.Authentication.Models.Responses;
using Libelula.Authentication.Services.Interfaces;
using Libelula.Authentication.Services.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;

    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Authentication([FromBody] LoginRequest loginRequest)
    {
        RefreshToken refreshToken = await this.CreateRefreshToken();
        ServiceResult<LoginResponse> serviceResult = await this.authService.Login(loginRequest, refreshToken);
        if (!serviceResult.IsValid)
        {
            return this.BadRequest(new ApiErrorResult(serviceResult.Errors!.ToList().ConvertAll(error => new Error(error)), ControllerConstants.ErrorMessage));
        }
        else
        {
            return this.Ok(new ApiDataResult<LoginResponse>(serviceResult.Result!));
        }
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> RegisterUser(SignUpRequest request)
    {
        return this.Ok(await this.authService.RegisterUser(request));
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUser(string email)
    {
        return this.Ok(await this.authService.GetUser(email));
    }

    private async Task<RefreshToken> CreateRefreshToken()
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.Now.AddDays(1),
            Created = DateTime.Now,
        };
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = refreshToken.Expires,
        };
        return await Task.Run(() => refreshToken);
    }
}