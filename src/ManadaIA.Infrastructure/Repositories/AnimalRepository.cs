using ManadaIA.Domain.Entities;
using ManadaIA.Domain.Interfaces;
using ManadaIA.Infrastructure.Models;
using Supabase;
using System.Data.SqlTypes;

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

    public async Task<Animal> AddAsync(Animal entity, CancellationToken ct = default)
    {
        var model = AnimalModel.FromDomain(entity);

        var response = await supabase
            .From<AnimalModel>()
            .Insert(model, new Postgrest.QueryOptions
            {
                Returning = Postgrest.QueryOptions.ReturnType.Representation
            });

        var inserted = response.Models.FirstOrDefault() ?? throw new SqlNullValueException();

        entity.SetId(inserted.Id);
        return entity;
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
