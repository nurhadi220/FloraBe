using floral.Application.Features.Auth.Services;
using floral.Application.Features.Auth.Validators;
using floral.Application.Features.Users.Services;
using floral.Application.Features.Users.Validators;
using Microsoft.Extensions.DependencyInjection;


namespace floral.Application.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<UserServices>();
        services.AddScoped<UserValidator>();

        services.AddScoped<AuthService>();
        services.AddScoped<RegisterValidator>();
        services.AddScoped<LoginValidator>();
        

        return services;
    }
}