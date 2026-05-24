using ManadaIA.Application.DTOs;

namespace ManadaIA.Application.Services;

public interface IUserService
{
    Task<LoginDto> Login(UserLoginRequest request, CancellationToken ct = default);
}

public sealed class UserService() : IUserService
{
    public Task<LoginDto> Login(UserLoginRequest request, CancellationToken ct = default)
    {
        return null;
    }
}
