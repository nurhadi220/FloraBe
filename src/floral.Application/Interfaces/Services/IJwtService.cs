using floral.Domain.Entities;
namespace floral.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}