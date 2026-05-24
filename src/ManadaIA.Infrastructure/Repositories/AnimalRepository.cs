using ManadaIA.Domain.Entities;
using ManadaIA.Domain.Interfaces;
using ManadaIA.Infrastructure.Models;
using Supabase;

namespace ManadaIA.Infrastructure.Repositories;

public sealed class AnimalRepository(Client supabase) : IAnimalRepository
{
    public async Task<Animal?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var result = await supabase
            .From<AnimalModel>()
            .Where(x => x.Id == id)
            .Single();

        return result?.ToDomain();
    }

    public async Task<Animal?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        var result = await supabase
            .From<AnimalModel>()
            .Where(x => x.Code == code)
            .Single();

        return result?.ToDomain();
    }

    public async Task<IReadOnlyList<Animal>> GetAllAsync(CancellationToken ct = default)
    {
        var result = await supabase
            .From<AnimalModel>()
            .Get();

        return result.Models.Select(m => m.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<Animal>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var result = await supabase
            .From<AnimalModel>()
            .Where(x => x.UserId == userId)
            .Get();

        return result.Models.Select(m => m.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<Animal>> GetBySpeciesAsync(Guid userId, Species species, CancellationToken ct = default)
    {
        var speciesStr = species.ToString().ToUpper();
        var result = await supabase
            .From<AnimalModel>()
            .Where(x => x.UserId == userId)
            .Where(x => x.Species == speciesStr)
            .Get();

        return result.Models.Select(m => m.ToDomain()).ToList();
    }

    public async Task AddAsync(Animal entity, CancellationToken ct = default)
    {
        var model = AnimalModel.FromDomain(entity);
        await supabase.From<AnimalModel>().Insert(model);
    }

    public async Task UpdateAsync(Animal entity, CancellationToken ct = default)
    {
        var model = AnimalModel.FromDomain(entity);
        await supabase.From<AnimalModel>().Update(model);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await supabase
            .From<AnimalModel>()
            .Where(x => x.Id == id)
            .Delete();
    }
}
