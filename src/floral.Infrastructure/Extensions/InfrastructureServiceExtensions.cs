using floral.Application.Interfaces;
using floral.Application.Interfaces.Services;
using floral.Infrastructure.Persistence;
using floral.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using floral.Application.Interfaces.Repositories;
using floral.Infrastructure.Persistence.Repositories;
using System.Reflection.Metadata.Ecma335;
using floral.Application.Interfaces.Repositories;
using floral.Infrastructure.Persistence.Repositories;

namespace floral.Infrastructure.Extensions;

/// <summary>Extension methods untuk registrasi semua dependency Infrastructure.</summary>
public static class InfrastructureServiceExtensions
{
    /// <summary>
    /// Mendaftarkan semua service Infrastructure ke DI container.
    /// </summary>
public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration)
{
   // Database
var connectionString =
    configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection belum dikonfigurasi.");

    services.AddDbContext<AppDbContext>(options =>
        options.UseMySQL(connectionString)
    );

    // Repositories
    services.AddScoped<IUserRepository, UserRepository>();

    // Services
    services.AddScoped<IPasswordHasher, PasswordHasher>();
    services.AddScoped<IJwtService, JwtService>();
    services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    services.AddScoped<ITransactionManager, TransactionManager>();

    // relasi user
    services.AddScoped<IRoleRepository, RoleRepository>();
    services.AddScoped<IUserRoleRepository, UserRoleRepository>();


    return services;
    }
}
