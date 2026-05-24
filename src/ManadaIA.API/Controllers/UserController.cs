using ManadaIA.Application.DTOs;
using ManadaIA.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManadaIA.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/users")]
public sealed class UserController(IUserService userService) : ControllerBase
{
    /// <summary>
    /// Realiza login do usuário e retorna um token de autenticação
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(UserLoginDto), 200)]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest request, CancellationToken ct = default)
    {
        var result = await userService.Login(request, ct);
        return Ok(result);
    }
}
