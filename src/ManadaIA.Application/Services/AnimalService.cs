using ManadaIA.Application.DTOs;
using ManadaIA.Domain.Entities;
using ManadaIA.Domain.Exceptions;
using ManadaIA.Domain.Interfaces;

namespace ManadaIA.Application.Services;

public interface IAnimalService
{
    Task<AnimalDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<AnimalDto>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<AnimalDto>> GetBySpeciesAsync(Guid userId, string species, CancellationToken ct = default);
    Task<AnimalDto> CreateAsync(Guid userId, CreateAnimalRequest request, CancellationToken ct = default);
    Task<AnimalDto> UpdateAsync(Guid id, UpdateAnimalRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public sealed class AnimalService(IAnimalRepository animalRepository) : IAnimalService
{
    public async Task<AnimalDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var animal = await animalRepository.GetByIdAsync(id, ct);
        if (animal is null)
            throw new NotFoundException("Animal", id);

        return MapToDto(animal);
    }

    public async Task<IReadOnlyList<AnimalDto>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var animals = await animalRepository.GetByUserIdAsync(userId, ct);
        return animals.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<AnimalDto>> GetBySpeciesAsync(Guid userId, string species, CancellationToken ct = default)
    {
        if (!Enum.TryParse<Species>(species, true, out var speciesEnum))
            throw new ArgumentException($"Espécie inválida: {species}");

        var animals = await animalRepository.GetBySpeciesAsync(userId, speciesEnum, ct);
        return animals.Select(MapToDto).ToList();
    }

    public async Task<AnimalDto> CreateAsync(Guid userId, CreateAnimalRequest request, CancellationToken ct = default)
    {
        // Validar se já existe animal com mesmo código
        var existing = await animalRepository.GetByCodeAsync(request.Code, ct);
        if (existing is not null)
            throw new ConflictException($"Já existe um animal com o código '{request.Code}'");

        if (!Enum.TryParse<Species>(request.Species, true, out var species))
            throw new ArgumentException($"Espécie inválida: {request.Species}");

        if (!Enum.TryParse<Sex>(request.Sex, true, out var sex))
            throw new ArgumentException($"Sexo inválido: {request.Sex}");

        var animal = Animal.Create(
            userId,
            request.Code,
            species,
            sex,
            request.Name,
            request.Breed,
            request.Lineage,
            request.BirthDate,
            request.WeightKg,
            request.Notes
        );

        animal = await animalRepository.AddAsync(animal, ct);

        return MapToDto(animal!);
    }

    public async Task<AnimalDto> UpdateAsync(Guid id, UpdateAnimalRequest request, CancellationToken ct = default)
    {
        var animal = await animalRepository.GetByIdAsync(id, ct);
        if (animal is null)
            throw new NotFoundException("Animal", id);

        animal.Update(
            request.Name,
            request.Breed,
            request.Lineage,
            request.BirthDate,
            request.WeightKg,
            request.Notes
        );

        await animalRepository.UpdateAsync(animal, ct);

        return MapToDto(animal);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var animal = await animalRepository.GetByIdAsync(id, ct);
        if (animal is null)
            throw new NotFoundException("Animal", id);

        await animalRepository.DeleteAsync(id, ct);
    }

    private static AnimalDto MapToDto(Animal animal) => new(
        animal.Id,
        animal.UserId,
        animal.Code,
        animal.Name,
        animal.Species.ToString().ToUpper(),
        animal.Sex.ToString().ToUpper(),
        animal.Breed,
        animal.Lineage,
        animal.BirthDate,
        animal.WeightKg,
        animal.Notes,
        animal.CreatedAt,
        animal.UpdatedAt
    );
}
