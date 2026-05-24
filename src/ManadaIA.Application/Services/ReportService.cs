using ManadaIA.Application.DTOs;
using ManadaIA.Domain.Entities;
using ManadaIA.Domain.Interfaces;

namespace ManadaIA.Application.Services;

public interface IReportService
{
    Task<SummaryReportDto> GetSummaryAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<PregnancyRateReportDto>> GetPregnancyRateAsync(Guid userId, CancellationToken ct = default);
}

public sealed class ReportService(
    IAnimalRepository animalRepository,
    IReproductiveCycleRepository cycleRepository) : IReportService
{
    public async Task<SummaryReportDto> GetSummaryAsync(Guid userId, CancellationToken ct = default)
    {
        var animals = await animalRepository.GetByUserIdAsync(userId, ct);

        return new SummaryReportDto(
            TotalAnimals: animals.Count,
            TotalBovinos: animals.Count(a => a.Species == Species.BOVINO),
            TotalOvinos: animals.Count(a => a.Species == Species.OVINO),
            TotalCaprinos: animals.Count(a => a.Species == Species.CAPRINO),
            TotalFemales: animals.Count(a => a.Sex == Sex.FEMEA),
            TotalMales: animals.Count(a => a.Sex == Sex.MACHO)
        );
    }

    public async Task<IReadOnlyList<PregnancyRateReportDto>> GetPregnancyRateAsync(Guid userId, CancellationToken ct = default)
    {
        var animals = await animalRepository.GetByUserIdAsync(userId, ct);
        var animalIds = animals.Select(a => a.Id).ToList();

        var reports = new List<PregnancyRateReportDto>();

        foreach (var species in Enum.GetValues<Species>())
        {
            var speciesAnimals = animals.Where(a => a.Species == species).Select(a => a.Id).ToList();
            if (!speciesAnimals.Any()) continue;

            var cycles = new List<ReproductiveCycle>();
            foreach (var animalId in speciesAnimals)
            {
                var animalCycles = await cycleRepository.GetByAnimalIdAsync(animalId, ct);
                cycles.AddRange(animalCycles);
            }

            var inseminations = cycles.Count(c => c.EventType == EventType.INSEMINACAO);
            var pregnant = cycles.Count(c => c.Result == Result.PRENHA);

            var rate = inseminations > 0 ? (decimal)pregnant / inseminations * 100 : 0;

            reports.Add(new PregnancyRateReportDto(
                Species: species.ToString().ToUpper(),
                TotalInseminations: inseminations,
                TotalPregnant: pregnant,
                PregnancyRate: Math.Round(rate, 2)
            ));
        }

        return reports;
    }
}
