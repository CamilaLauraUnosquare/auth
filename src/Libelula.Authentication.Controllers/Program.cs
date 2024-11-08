using System.Data.SqlClient;
using System.Reflection;
using Libelula.Authentication.Controllers.Helpers;
using Libelula.Authentication.DataAccess.Context;
using Libelula.Authentication.DataAccess.Interfaces;
using Libelula.Authentication.DataAccess.Services;
using Libelula.Authentication.Models;
using Libelula.Authentication.Services.Interfaces;
using Libelula.Authentication.Services.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

public class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Authentication API",
                Description = "An ASP.NET Core Web API for manaßging authentication service",
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
            });
            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });

        IServiceCollection services = builder.Services;
        IWebHostEnvironment env = builder.Environment;

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins("*").AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });
        builder.Services.AddSingleton<DataContext>();
        builder.Services.AddScoped<IAuthService, AuthService>();

        string? sqlSetup = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddScoped<IAuthRepository>(x => new AuthRepository(sqlSetup!));
        string? secret = builder.Configuration.GetSection("AuthSettings:Secret").Value;
        builder.Services.AddBearerJWTAuthentication(secret!);
        builder.Services.UseCustomBadExceptionHandler();
        builder.Services.AddSingleton(new AuthSettings(secret!));

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            await context.Init();
        }

        app.Use(async (context, next) =>
        {
            context.Request.EnableBuffering();
            await next();
        });

        app.UseSwagger(options =>
        {
            options.SerializeAsV2 = true;
        });

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/authentication/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = "info/swagger";
        });

        app.UseCustomExceptionHandler();

        app.UseSwagger();

        app.UseSwaggerUI();

        app.UseCors();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}