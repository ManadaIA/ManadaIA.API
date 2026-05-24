using ManadaIA.Application.DTOs;
using ManadaIA.Application.ExternalServices.Interfaces;

namespace ManadaIA.Application.Services;

public interface IUserService
{
    Task<LoginDto> Login(UserLoginRequest request, CancellationToken ct = default);
}

public sealed class UserService(ISupabaseAuthService supabaseAuthService) : IUserService
{
    public async Task<LoginDto> Login(UserLoginRequest request, CancellationToken ct = default)
    {
        return await supabaseAuthService.AuthenticateAsync(request, ct);
    }
}
