namespace Libelula.Authentication.Controllers.Helpers;

using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Libelula.Authentication.Controllers.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

public static class WebApplicationExtensionMethods
{
    public static void UseCustomExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                context.Response.ContentType = "application/json";

                var exceptionHandlerPathFeature =
                    context.Features.Get<IExceptionHandlerPathFeature>();

                ApiBasicResult apiErrorResult = new ApiBasicResult(exceptionHandlerPathFeature?.Error);

                await context.Response.WriteAsJsonAsync(apiErrorResult);
            });
        });
    }

    public static void UseCustomBadExceptionHandler(this IServiceCollection service)
    {
        service.AddMvc()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = (context) =>
                {
                    string bodyStr = string.Empty;
                    HttpRequest req = context.HttpContext.Request;
                    req.Body.Position = 0;
                    using (StreamReader reader = new (req.Body, Encoding.UTF8, true, 1024, true))
                    {
                        Task<string> body = reader.ReadToEndAsync();
                        body.Wait();
                        bodyStr = Regex.Replace(body.Result, @"\t|\n|\r", string.Empty);
                    }

                    var apiErrorResult = new ApiErrorResult(context.ModelState, bodyStr);
                    return new BadRequestObjectResult(apiErrorResult);
                };
            });
    }

    public static void AddSwaggerDoc(this IServiceCollection service)
    {
        service.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Libélula Authentication API",
                Description = "An ASP.NET Core Web API for managing authentication of Libélula Conciliations",
            });

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
            });

            options.OperationFilter<SecurityRequirementsOperationFilter>();

            string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }

    public static void AddBearerJWTAuthentication(this IServiceCollection service, string secret)
    {
        service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });
    }
}