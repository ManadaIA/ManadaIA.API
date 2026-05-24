using ManadaIA.Application.DTOs;
using ManadaIA.Domain.Entities;
using ManadaIA.Domain.Exceptions;
using ManadaIA.Domain.Interfaces;

namespace ManadaIA.Application.Services;

public interface IAIPredictionService
{
    Task<AIPredictionDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<AIPredictionDto>> GetByAnimalIdAsync(Guid animalId, CancellationToken ct = default);
    Task<AIPredictionDto> CreateAsync(CreateAIPredictionRequest request, CancellationToken ct = default);
}

public sealed class AIPredictionService(
    IAIPredictionRepository predictionRepository,
    IAnimalRepository animalRepository) : IAIPredictionService
{
    public async Task<AIPredictionDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var prediction = await predictionRepository.GetByIdAsync(id, ct);
        if (prediction is null)
            throw new NotFoundException("Predição", id);

        return MapToDto(prediction);
    }

    public async Task<IReadOnlyList<AIPredictionDto>> GetByAnimalIdAsync(Guid animalId, CancellationToken ct = default)
    {
        var predictions = await predictionRepository.GetByAnimalIdAsync(animalId, ct);
        return predictions.Select(MapToDto).ToList();
    }

    public async Task<AIPredictionDto> CreateAsync(CreateAIPredictionRequest request, CancellationToken ct = default)
    {
        // Validar se animal existe
        var animal = await animalRepository.GetByIdAsync(request.AnimalId, ct);
        if (animal is null)
            throw new NotFoundException("Animal", request.AnimalId);

        ConfidenceLevel? confidenceLevel = null;
        if (!string.IsNullOrWhiteSpace(request.ConfidenceLevel))
        {
            if (!Enum.TryParse<ConfidenceLevel>(request.ConfidenceLevel, true, out var conf))
                throw new ArgumentException($"Nível de confiança inválido: {request.ConfidenceLevel}");
            confidenceLevel = conf;
        }

        var prediction = AIPrediction.Create(
            request.AnimalId,
            request.CycleId,
            request.PregnancyRate,
            confidenceLevel,
            request.Explanation,
            request.RiskFactors,
            request.Recommendations,
            request.AiModelUsed,
            request.RawPrompt,
            request.RawResponse
        );

        prediction = await predictionRepository.AddAsync(prediction, ct);

        return MapToDto(prediction!);
    }

    private static AIPredictionDto MapToDto(AIPrediction prediction) => new(
        prediction.Id,
        prediction.AnimalId,
        prediction.CycleId,
        prediction.PredictionDate,
        prediction.PregnancyRate,
        prediction.ConfidenceLevel?.ToString().ToUpper(),
        prediction.Explanation,
        prediction.RiskFactors,
        prediction.Recommendations,
        prediction.AiModelUsed
    );
}
