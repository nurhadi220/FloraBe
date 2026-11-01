using floral.Application.Features.Users.DTOs;
using floral.Application.Features.Users.Validators;
using floral.Application.Interfaces.Repositories;

namespace floral.Application.Features.Users.Services;

public class UserServices
{
    private readonly IUserRepository _userRepository;
    private readonly UserValidator _validator;

    public UserServices(
        IUserRepository userRepository,
        UserValidator validator)
    {
        _userRepository = userRepository;
        _validator = validator;
    }

    public async Task<UserDto?> GetByPublicIdAsync(string publicId)
    {
        var errors = _validator.ValidatePublicId(publicId);

        if (errors.Count > 0)
        {
            throw new ArgumentException(string.Join(" ", errors));
        }

        var user = await _userRepository.GetByPublicIdAsync(publicId);

        if (user is null)
        {
            return null;
        }

        return UserDto.FromEntity(user);
    }

    public async Task<UserListDto> GetAllAsync(
        int page,
        int pageSize,
        string? search)
    {
        var errors = _validator.ValidatePagination(page, pageSize);

        if (errors.Count > 0)
        {
            throw new ArgumentException(string.Join(" ", errors));
        }

        var users = await _userRepository.GetAllAsync(
            page,
            pageSize,
            search);

        var totalCount = await _userRepository.CountAsync(search);

        return new UserListDto
        {
            Items = users.Select(UserDto.FromEntity).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}