namespace ManadaIA.Domain.Entities;

/// <summary>
/// Representa uma predição de IA para prenhez
/// </summary>
public sealed class AIPrediction
{
    public Guid Id { get; private set; }
    public Guid AnimalId { get; private set; }
    public Guid? CycleId { get; private set; }
    public DateTime PredictionDate { get; private set; }
    public decimal? PregnancyRate { get; private set; } // 0.00 a 100.00
    public ConfidenceLevel? ConfidenceLevel { get; private set; }
    public string? Explanation { get; private set; }
    public string? RiskFactors { get; private set; } // JSON array
    public string? Recommendations { get; private set; } // JSON array
    public string? AiModelUsed { get; private set; }
    public string? RawPrompt { get; private set; }
    public string? RawResponse { get; private set; }

    // EF/ORM private constructor
    private AIPrediction() { }

    public static AIPrediction Create(
        Guid animalId,
        Guid? cycleId,
        decimal? pregnancyRate,
        ConfidenceLevel? confidenceLevel,
        string? explanation,
        string? riskFactors,
        string? recommendations,
        string? aiModelUsed,
        string? rawPrompt,
        string? rawResponse)
    {
        if (pregnancyRate.HasValue && (pregnancyRate.Value < 0 || pregnancyRate.Value > 100))
            throw new ArgumentException("Taxa de prenhez deve estar entre 0 e 100", nameof(pregnancyRate));

        return new AIPrediction
        {
            AnimalId = animalId,
            CycleId = cycleId,
            PredictionDate = DateTime.UtcNow,
            PregnancyRate = pregnancyRate,
            ConfidenceLevel = confidenceLevel,
            Explanation = explanation,
            RiskFactors = riskFactors,
            Recommendations = recommendations,
            AiModelUsed = aiModelUsed,
            RawPrompt = rawPrompt,
            RawResponse = rawResponse
        };
    }

    public void SetId(Guid id) => Id = id;
}

public enum ConfidenceLevel
{
    ALTA,
    MEDIA,
    BAIXA
}
