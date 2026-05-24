using ManadaIA.Application.DTOs;
using ManadaIA.Domain.Entities;
using ManadaIA.Domain.Exceptions;
using ManadaIA.Domain.Interfaces;

namespace ManadaIA.Application.Services;

public interface IReproductiveCycleService
{
    Task<ReproductiveCycleDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ReproductiveCycleDto>> GetByAnimalIdAsync(Guid animalId, CancellationToken ct = default);
    Task<ReproductiveCycleDto> CreateAsync(CreateReproductiveCycleRequest request, CancellationToken ct = default);
    Task<ReproductiveCycleDto> UpdateResultAsync(Guid id, UpdateReproductiveCycleRequest request, CancellationToken ct = default);
}

public sealed class ReproductiveCycleService(
    IReproductiveCycleRepository cycleRepository,
    IAnimalRepository animalRepository) : IReproductiveCycleService
{
    public async Task<ReproductiveCycleDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var cycle = await cycleRepository.GetByIdAsync(id, ct);
        if (cycle is null)
            throw new NotFoundException("Ciclo reprodutivo", id);

        return MapToDto(cycle);
    }

    public async Task<IReadOnlyList<ReproductiveCycleDto>> GetByAnimalIdAsync(Guid animalId, CancellationToken ct = default)
    {
        var cycles = await cycleRepository.GetByAnimalIdAsync(animalId, ct);
        return cycles.Select(MapToDto).ToList();
    }

    public async Task<ReproductiveCycleDto> CreateAsync(CreateReproductiveCycleRequest request, CancellationToken ct = default)
    {
        // Validar se animal existe
        var animal = await animalRepository.GetByIdAsync(request.AnimalId, ct);
        if (animal is null)
            throw new NotFoundException("Animal", request.AnimalId);

        if (!Enum.TryParse<EventType>(request.EventType, true, out var eventType))
            throw new ArgumentException($"Tipo de evento inválido: {request.EventType}");

        Technique? technique = null;
        if (!string.IsNullOrWhiteSpace(request.Technique))
        {
            if (!Enum.TryParse<Technique>(request.Technique, true, out var tech))
                throw new ArgumentException($"Técnica inválida: {request.Technique}");
            technique = tech;
        }

        Result? result = null;
        if (!string.IsNullOrWhiteSpace(request.Result))
        {
            if (!Enum.TryParse<Result>(request.Result, true, out var res))
                throw new ArgumentException($"Resultado inválido: {request.Result}");
            result = res;
        }

        var cycle = ReproductiveCycle.Create(
            request.AnimalId,
            request.EventDate,
            eventType,
            request.SireName,
            request.SemenBatch,
            technique,
            request.Technician,
            result,
            request.Notes
        );

        cycle = await cycleRepository.AddAsync(cycle, ct);

        return MapToDto(cycle!);
    }

    public async Task<ReproductiveCycleDto> UpdateResultAsync(Guid id, UpdateReproductiveCycleRequest request, CancellationToken ct = default)
    {
        var cycle = await cycleRepository.GetByIdAsync(id, ct);
        if (cycle is null)
            throw new NotFoundException("Ciclo reprodutivo", id);

        if (!Enum.TryParse<Result>(request.Result, true, out var result))
            throw new ArgumentException($"Resultado inválido: {request.Result}");

        cycle.UpdateResult(result, request.Notes);

        await cycleRepository.UpdateAsync(cycle, ct);

        return MapToDto(cycle);
    }

    private static ReproductiveCycleDto MapToDto(ReproductiveCycle cycle) => new(
        cycle.Id,
        cycle.AnimalId,
        cycle.EventDate,
        cycle.EventType.ToString().ToUpper(),
        cycle.SireName,
        cycle.SemenBatch,
        cycle.Technique?.ToString().ToUpper(),
        cycle.Technician,
        cycle.Result?.ToString().ToUpper(),
        cycle.Notes,
        cycle.CreatedAt
    );
}
