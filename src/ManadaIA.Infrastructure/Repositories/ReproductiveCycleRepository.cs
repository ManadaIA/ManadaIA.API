using ManadaIA.Domain.Entities;
using ManadaIA.Domain.Interfaces;
using ManadaIA.Infrastructure.Models;
using Supabase;
using System.Data.SqlTypes;

namespace ManadaIA.Infrastructure.Repositories;

public sealed class ReproductiveCycleRepository(Client supabase) : IReproductiveCycleRepository
{
    public async Task<ReproductiveCycle?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var result = await supabase
            .From<ReproductiveCycleModel>()
            .Where(x => x.Id == id)
            .Single();

        return result?.ToDomain();
    }

    public async Task<IReadOnlyList<ReproductiveCycle>> GetAllAsync(CancellationToken ct = default)
    {
        var result = await supabase
            .From<ReproductiveCycleModel>()
            .Get();

        return result.Models.Select(m => m.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<ReproductiveCycle>> GetByAnimalIdAsync(Guid animalId, CancellationToken ct = default)
    {
        var result = await supabase
            .From<ReproductiveCycleModel>()
            .Where(x => x.AnimalId == animalId)
            .Order("event_date", Postgrest.Constants.Ordering.Descending)
            .Get();

        return result.Models.Select(m => m.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<ReproductiveCycle>> GetByEventTypeAsync(EventType eventType, CancellationToken ct = default)
    {
        var eventTypeStr = eventType.ToString().ToUpper();
        var result = await supabase
            .From<ReproductiveCycleModel>()
            .Where(x => x.EventType == eventTypeStr)
            .Get();

        return result.Models.Select(m => m.ToDomain()).ToList();
    }

    public async Task<ReproductiveCycle> AddAsync(ReproductiveCycle entity, CancellationToken ct = default)
    {
        var model = ReproductiveCycleModel.FromDomain(entity);

        var response = await supabase
            .From<ReproductiveCycleModel>()
            .Insert(model, new Postgrest.QueryOptions
            {
                Returning = Postgrest.QueryOptions.ReturnType.Representation
            });

        var inserted = response.Models.FirstOrDefault() ?? throw new SqlNullValueException();

        entity.SetId(inserted.Id);
        return entity;
    }

    public async Task UpdateAsync(ReproductiveCycle entity, CancellationToken ct = default)
    {
        var model = ReproductiveCycleModel.FromDomain(entity);
        await supabase.From<ReproductiveCycleModel>().Update(model);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await supabase
            .From<ReproductiveCycleModel>()
            .Where(x => x.Id == id)
            .Delete();
    }
}
