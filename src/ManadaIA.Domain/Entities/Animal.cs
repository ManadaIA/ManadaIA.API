namespace ManadaIA.Domain.Entities;

/// <summary>
/// Representa um animal do rebanho
/// </summary>
public sealed class Animal
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string? Name { get; private set; }
    public Species Species { get; private set; }
    public Sex Sex { get; private set; }
    public string? Breed { get; private set; }
    public string? Lineage { get; private set; }
    public DateTime? BirthDate { get; private set; }
    public decimal? WeightKg { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Animal() { }

    public static Animal Create(
        Guid userId,
        string code,
        Species species,
        Sex sex,
        string? name = null,
        string? breed = null,
        string? lineage = null,
        DateTime? birthDate = null,
        decimal? weightKg = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Código do animal é obrigatório", nameof(code));

        if (birthDate.HasValue && birthDate.Value > DateTime.UtcNow)
            throw new ArgumentException("Data de nascimento não pode ser futura", nameof(birthDate));

        return new Animal
        {
            UserId = userId,
            Code = code,
            Name = name,
            Species = species,
            Sex = sex,
            Breed = breed,
            Lineage = lineage,
            BirthDate = birthDate,
            WeightKg = weightKg,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string? name = null,
        string? breed = null,
        string? lineage = null,
        DateTime? birthDate = null,
        decimal? weightKg = null,
        string? notes = null)
    {
        if (name is not null) Name = name;
        if (breed is not null) Breed = breed;
        if (lineage is not null) Lineage = lineage;
        if (birthDate.HasValue) BirthDate = birthDate;
        if (weightKg.HasValue) WeightKg = weightKg;
        if (notes is not null) Notes = notes;
        
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetId(Guid id) => Id = id;
}

public enum Species
{
    BOVINO,
    OVINO,
    CAPRINO
}

public enum Sex
{
    FEMEA,
    MACHO
}
