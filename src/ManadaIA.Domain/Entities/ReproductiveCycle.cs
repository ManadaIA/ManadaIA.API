namespace ManadaIA.Domain.Entities;

/// <summary>
/// Representa um ciclo reprodutivo (inseminação, parto, diagnóstico etc.)
/// </summary>
public sealed class ReproductiveCycle
{
    public Guid Id { get; private set; }
    public Guid AnimalId { get; private set; }
    public DateTime EventDate { get; private set; }
    public EventType EventType { get; private set; }
    public string? SireName { get; private set; }
    public string? SemenBatch { get; private set; }
    public Technique? Technique { get; private set; }
    public string? Technician { get; private set; }
    public Result? Result { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private ReproductiveCycle() { }

    public static ReproductiveCycle Create(
        Guid animalId,
        DateTime eventDate,
        EventType eventType,
        string? sireName = null,
        string? semenBatch = null,
        Technique? technique = null,
        string? technician = null,
        Result? result = null,
        string? notes = null)
    {
        if (eventDate > DateTime.UtcNow)
            throw new ArgumentException("Data do evento não pode ser futura", nameof(eventDate));

        return new ReproductiveCycle
        {
            AnimalId = animalId,
            EventDate = eventDate,
            EventType = eventType,
            SireName = sireName,
            SemenBatch = semenBatch,
            Technique = technique,
            Technician = technician,
            Result = result,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateResult(Result result, string? notes = null)
    {
        Result = result;
        if (notes is not null)
            Notes = notes;
    }

    public void SetId(Guid id) => Id = id;
}

public enum EventType
{
    INSEMINACAO,
    PARTO,
    DIAGNOSTICO,
    RETORNO
}

public enum Technique
{
    IATF,
    IA,
    MONTA
}

public enum Result
{
    PRENHA,
    VAZIA,
    AGUARDANDO,
    PERDIDA
}
