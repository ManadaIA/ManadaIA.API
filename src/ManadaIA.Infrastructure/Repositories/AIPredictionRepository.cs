using ManadaIA.Domain.Entities;
using ManadaIA.Domain.Interfaces;
using ManadaIA.Infrastructure.Models;
using Supabase;
using System.Data.SqlTypes;

namespace ManadaIA.Infrastructure.Repositories;

public sealed class AIPredictionRepository(Client supabase) : IAIPredictionRepository
{
    public async Task<AIPrediction?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var result = await supabase
            .From<AIPredictionModel>()
            .Where(x => x.Id == id)
            .Single();

        return result?.ToDomain();
    }

    public async Task<IReadOnlyList<AIPrediction>> GetAllAsync(CancellationToken ct = default)
    {
        var result = await supabase
            .From<AIPredictionModel>()
            .Get();

        return result.Models.Select(m => m.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<AIPrediction>> GetByAnimalIdAsync(Guid animalId, CancellationToken ct = default)
    {
        var result = await supabase
            .From<AIPredictionModel>()
            .Where(x => x.AnimalId == animalId)
            .Order("prediction_date", Postgrest.Constants.Ordering.Descending)
            .Get();

        return result.Models.Select(m => m.ToDomain()).ToList();
    }

    public async Task<AIPrediction?> GetLatestByAnimalIdAsync(Guid animalId, CancellationToken ct = default)
    {
        var result = await supabase
            .From<AIPredictionModel>()
            .Where(x => x.AnimalId == animalId)
            .Order("prediction_date", Postgrest.Constants.Ordering.Descending)
            .Limit(1)
            .Single();

        return result?.ToDomain();
    }

    public async Task<AIPrediction> AddAsync(AIPrediction entity, CancellationToken ct = default)
    {
        var model = AIPredictionModel.FromDomain(entity);

        var response = await supabase
            .From<AIPredictionModel>()
            .Insert(model, new Postgrest.QueryOptions
            {
                Returning = Postgrest.QueryOptions.ReturnType.Representation
            });

        var inserted = response.Models.FirstOrDefault() ?? throw new SqlNullValueException();

        entity.SetId(inserted.Id);
        return entity;
    }

    public async Task UpdateAsync(AIPrediction entity, CancellationToken ct = default)
    {
        var model = AIPredictionModel.FromDomain(entity);
        await supabase.From<AIPredictionModel>().Update(model);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await supabase
            .From<AIPredictionModel>()
            .Where(x => x.Id == id)
            .Delete();
    }
}
