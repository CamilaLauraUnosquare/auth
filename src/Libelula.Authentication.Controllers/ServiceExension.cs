namespace Libelula.Authentication.Controllers;

public static class ServiceExension
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();

        // services.AddMappings();
        return services;
    }
}