namespace floral.Application.Features.Users.Validators;

public class UserValidator
{
    public List<string> ValidatePagination(int page, int pageSize)
    {
        var errors = new List<string>();

        if (page < 1)
        {
            errors.Add("Page harus lebih besar dari 0.");
        }

        if (pageSize < 1)
        {
            errors.Add("PageSize harus lebih besar dari 0.");
        }

        if (pageSize > 100)
        {
            errors.Add("PageSize maksimal 100.");
        }

        return errors;
    }

    public List<string> ValidatePublicId(string? publicId)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(publicId))
        {
            errors.Add("PublicId wajib diisi.");
        }

        return errors;
    }
}