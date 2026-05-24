using ManadaIA.Application.DTOs;
using ManadaIA.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ManadaIA.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/animals")]
[Authorize]
public sealed class AnimalsController(IAnimalService animalService) : ControllerBase
{
    /// <summary>
    /// Lista todos os animais do usuário
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AnimalDto>), 200)]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
    {
        var userId = GetUserId();
        var result = await animalService.GetByUserIdAsync(userId, ct);
        return Ok(result);
    }

    /// <summary>
    /// Obtém um animal pelo ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AnimalDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await animalService.GetByIdAsync(id, ct);
        return Ok(result);
    }

    /// <summary>
    /// Filtra animais por espécie
    /// </summary>
    [HttpGet("species/{species}")]
    [ProducesResponseType(typeof(IReadOnlyList<AnimalDto>), 200)]
    public async Task<IActionResult> GetBySpecies(string species, CancellationToken ct = default)
    {
        var userId = GetUserId();
        var result = await animalService.GetBySpeciesAsync(userId, species, ct);
        return Ok(result);
    }

    /// <summary>
    /// Cadastra um novo animal
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AnimalDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create(
        [FromBody] CreateAnimalRequest request,
        CancellationToken ct = default)
    {
        var userId = GetUserId();
        var result = await animalService.CreateAsync(userId, request, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Atualiza um animal
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AnimalDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateAnimalRequest request,
        CancellationToken ct = default)
    {
        var result = await animalService.UpdateAsync(id, request, ct);
        return Ok(result);
    }

    /// <summary>
    /// Remove um animal
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        await animalService.DeleteAsync(id, ct);
        return NoContent();
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Usuário não autenticado");

        return userId;
    }
}
