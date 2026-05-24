using ManadaIA.Domain.Entities;
using Postgrest.Attributes;
using Postgrest.Models;

namespace ManadaIA.Infrastructure.Models;

[Table("animals")]
public sealed class AnimalModel : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [Column("name")]
    public string? Name { get; set; }

    [Column("species")]
    public string Species { get; set; } = string.Empty;

    [Column("sex")]
    public string Sex { get; set; } = string.Empty;

    [Column("breed")]
    public string? Breed { get; set; }

    [Column("lineage")]
    public string? Lineage { get; set; }

    [Column("birth_date")]
    public DateTime? BirthDate { get; set; }

    [Column("weight_kg")]
    public decimal? WeightKg { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    public Animal ToDomain()
    {
        var species = Enum.Parse<Species>(Species, true);
        var sex = Enum.Parse<Sex>(Sex, true);

        var animal = (Animal)Activator.CreateInstance(typeof(Animal), true)!;

        typeof(Animal).GetProperty("Id")!.SetValue(animal, Id);
        typeof(Animal).GetProperty("UserId")!.SetValue(animal, UserId);
        typeof(Animal).GetProperty("Code")!.SetValue(animal, Code);
        typeof(Animal).GetProperty("Name")!.SetValue(animal, Name);
        typeof(Animal).GetProperty("Species")!.SetValue(animal, species);
        typeof(Animal).GetProperty("Sex")!.SetValue(animal, sex);
        typeof(Animal).GetProperty("Breed")!.SetValue(animal, Breed);
        typeof(Animal).GetProperty("Lineage")!.SetValue(animal, Lineage);
        typeof(Animal).GetProperty("BirthDate")!.SetValue(animal, BirthDate);
        typeof(Animal).GetProperty("WeightKg")!.SetValue(animal, WeightKg);
        typeof(Animal).GetProperty("Notes")!.SetValue(animal, Notes);
        typeof(Animal).GetProperty("CreatedAt")!.SetValue(animal, CreatedAt);
        typeof(Animal).GetProperty("UpdatedAt")!.SetValue(animal, UpdatedAt);

        return animal;
    }

    public static AnimalModel FromDomain(Animal animal) => new()
    {
        Id = animal.Id,
        UserId = animal.UserId,
        Code = animal.Code,
        Name = animal.Name,
        Species = animal.Species.ToString().ToUpper(),
        Sex = animal.Sex.ToString().ToUpper(),
        Breed = animal.Breed,
        Lineage = animal.Lineage,
        BirthDate = animal.BirthDate,
        WeightKg = animal.WeightKg,
        Notes = animal.Notes,
        CreatedAt = animal.CreatedAt,
        UpdatedAt = animal.UpdatedAt
    };
}

[Table("reproductive_cycles")]
public sealed class ReproductiveCycleModel : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("animal_id")]
    public Guid AnimalId { get; set; }

    [Column("event_date")]
    public DateTime EventDate { get; set; }

    [Column("event_type")]
    public string EventType { get; set; } = string.Empty;

    [Column("sire_name")]
    public string? SireName { get; set; }

    [Column("semen_batch")]
    public string? SemenBatch { get; set; }

    [Column("technique")]
    public string? Technique { get; set; }

    [Column("technician")]
    public string? Technician { get; set; }

    [Column("result")]
    public string? Result { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public ReproductiveCycle ToDomain()
    {
        var eventType = Enum.Parse<EventType>(EventType, true);
        Technique? technique = string.IsNullOrWhiteSpace(Technique) ? null : Enum.Parse<Technique>(Technique, true);
        Result? result = string.IsNullOrWhiteSpace(Result) ? null : Enum.Parse<Result>(Result, true);

        var cycle = (ReproductiveCycle)Activator.CreateInstance(typeof(ReproductiveCycle), true)!;

        typeof(ReproductiveCycle).GetProperty("Id")!.SetValue(cycle, Id);
        typeof(ReproductiveCycle).GetProperty("AnimalId")!.SetValue(cycle, AnimalId);
        typeof(ReproductiveCycle).GetProperty("EventDate")!.SetValue(cycle, EventDate);
        typeof(ReproductiveCycle).GetProperty("EventType")!.SetValue(cycle, eventType);
        typeof(ReproductiveCycle).GetProperty("SireName")!.SetValue(cycle, SireName);
        typeof(ReproductiveCycle).GetProperty("SemenBatch")!.SetValue(cycle, SemenBatch);
        typeof(ReproductiveCycle).GetProperty("Technique")!.SetValue(cycle, technique);
        typeof(ReproductiveCycle).GetProperty("Technician")!.SetValue(cycle, Technician);
        typeof(ReproductiveCycle).GetProperty("Result")!.SetValue(cycle, result);
        typeof(ReproductiveCycle).GetProperty("Notes")!.SetValue(cycle, Notes);
        typeof(ReproductiveCycle).GetProperty("CreatedAt")!.SetValue(cycle, CreatedAt);

        return cycle;
    }

    public static ReproductiveCycleModel FromDomain(ReproductiveCycle cycle) => new()
    {
        Id = cycle.Id,
        AnimalId = cycle.AnimalId,
        EventDate = cycle.EventDate,
        EventType = cycle.EventType.ToString().ToUpper(),
        SireName = cycle.SireName,
        SemenBatch = cycle.SemenBatch,
        Technique = cycle.Technique?.ToString().ToUpper(),
        Technician = cycle.Technician,
        Result = cycle.Result?.ToString().ToUpper(),
        Notes = cycle.Notes,
        CreatedAt = cycle.CreatedAt
    };
}

[Table("ai_predictions")]
public sealed class AIPredictionModel : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("animal_id")]
    public Guid AnimalId { get; set; }

    [Column("cycle_id")]
    public Guid? CycleId { get; set; }

    [Column("prediction_date")]
    public DateTime PredictionDate { get; set; }

    [Column("pregnancy_rate")]
    public decimal? PregnancyRate { get; set; }

    [Column("confidence_level")]
    public string? ConfidenceLevel { get; set; }

    [Column("explanation")]
    public string? Explanation { get; set; }

    [Column("risk_factors")]
    public string? RiskFactors { get; set; }

    [Column("recommendations")]
    public string? Recommendations { get; set; }

    [Column("ai_model_used")]
    public string? AiModelUsed { get; set; }

    [Column("raw_prompt")]
    public string? RawPrompt { get; set; }

    [Column("raw_response")]
    public string? RawResponse { get; set; }

    public AIPrediction ToDomain()
    {
        ConfidenceLevel? confidenceLevel = string.IsNullOrWhiteSpace(ConfidenceLevel)
            ? null
            : Enum.Parse<ConfidenceLevel>(ConfidenceLevel, true);

        var prediction = (AIPrediction)Activator.CreateInstance(typeof(AIPrediction), true)!;

        typeof(AIPrediction).GetProperty("Id")!.SetValue(prediction, Id);
        typeof(AIPrediction).GetProperty("AnimalId")!.SetValue(prediction, AnimalId);
        typeof(AIPrediction).GetProperty("CycleId")!.SetValue(prediction, CycleId);
        typeof(AIPrediction).GetProperty("PredictionDate")!.SetValue(prediction, PredictionDate);
        typeof(AIPrediction).GetProperty("PregnancyRate")!.SetValue(prediction, PregnancyRate);
        typeof(AIPrediction).GetProperty("ConfidenceLevel")!.SetValue(prediction, confidenceLevel);
        typeof(AIPrediction).GetProperty("Explanation")!.SetValue(prediction, Explanation);
        typeof(AIPrediction).GetProperty("RiskFactors")!.SetValue(prediction, RiskFactors);
        typeof(AIPrediction).GetProperty("Recommendations")!.SetValue(prediction, Recommendations);
        typeof(AIPrediction).GetProperty("AiModelUsed")!.SetValue(prediction, AiModelUsed);
        typeof(AIPrediction).GetProperty("RawPrompt")!.SetValue(prediction, RawPrompt);
        typeof(AIPrediction).GetProperty("RawResponse")!.SetValue(prediction, RawResponse);

        return prediction;
    }

    public static AIPredictionModel FromDomain(AIPrediction prediction) => new()
    {
        Id = prediction.Id,
        AnimalId = prediction.AnimalId,
        CycleId = prediction.CycleId,
        PredictionDate = prediction.PredictionDate,
        PregnancyRate = prediction.PregnancyRate,
        ConfidenceLevel = prediction.ConfidenceLevel?.ToString().ToUpper(),
        Explanation = prediction.Explanation,
        RiskFactors = prediction.RiskFactors,
        Recommendations = prediction.Recommendations,
        AiModelUsed = prediction.AiModelUsed,
        RawPrompt = prediction.RawPrompt,
        RawResponse = prediction.RawResponse
    };
}