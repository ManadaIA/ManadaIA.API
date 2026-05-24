using ManadaIA.Application.DTOs;

namespace ManadaIA.Application.ExternalServices.Interfaces;

public interface ISupabaseAuthService
{
    Task<LoginDto> AuthenticateAsync(UserLoginRequest request, CancellationToken ct = default);
}