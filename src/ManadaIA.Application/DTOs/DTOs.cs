namespace ManadaIA.Application.DTOs;

// ═══════════════════════════════════════════
// ANIMALS DTOs
// ═══════════════════════════════════════════

public sealed record AnimalDto(
    Guid Id,
    Guid UserId,
    string Code,
    string? Name,
    string Species,
    string Sex,
    string? Breed,
    string? Lineage,
    DateTime? BirthDate,
    decimal? WeightKg,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public sealed record CreateAnimalRequest(
    string Code,
    string? Name,
    string Species,
    string Sex,
    string? Breed,
    string? Lineage,
    DateTime? BirthDate,
    decimal? WeightKg,
    string? Notes
);

public sealed record UpdateAnimalRequest(
    string? Name,
    string? Breed,
    string? Lineage,
    DateTime? BirthDate,
    decimal? WeightKg,
    string? Notes
);

// ═══════════════════════════════════════════
// REPRODUCTIVE CYCLES DTOs
// ═══════════════════════════════════════════

public sealed record ReproductiveCycleDto(
    Guid Id,
    Guid AnimalId,
    DateTime EventDate,
    string EventType,
    string? SireName,
    string? SemenBatch,
    string? Technique,
    string? Technician,
    string? Result,
    string? Notes,
    DateTime CreatedAt
);

public sealed record CreateReproductiveCycleRequest(
    Guid AnimalId,
    DateTime EventDate,
    string EventType,
    string? SireName,
    string? SemenBatch,
    string? Technique,
    string? Technician,
    string? Result,
    string? Notes
);

public sealed record UpdateReproductiveCycleRequest(
    string Result,
    string? Notes
);

// ═══════════════════════════════════════════
// AI PREDICTIONS DTOs
// ═══════════════════════════════════════════

public sealed record AIPredictionDto(
    Guid Id,
    Guid AnimalId,
    Guid? CycleId,
    DateTime PredictionDate,
    decimal? PregnancyRate,
    string? ConfidenceLevel,
    string? Explanation,
    string? RiskFactors,
    string? Recommendations,
    string? AiModelUsed
);

public sealed record CreateAIPredictionRequest(
    Guid AnimalId,
    Guid? CycleId,
    decimal? PregnancyRate,
    string? ConfidenceLevel,
    string? Explanation,
    string? RiskFactors,
    string? Recommendations,
    string? AiModelUsed,
    string? RawPrompt,
    string? RawResponse
);

// ═══════════════════════════════════════════
// REPORTS DTOs
// ═══════════════════════════════════════════

public sealed record SummaryReportDto(
    int TotalAnimals,
    int TotalBovinos,
    int TotalOvinos,
    int TotalCaprinos,
    int TotalFemales,
    int TotalMales
);

public sealed record PregnancyRateReportDto(
    string Species,
    int TotalInseminations,
    int TotalPregnant,
    decimal PregnancyRate
);

// ═══════════════════════════════════════════
// USERS DTOs
// ═══════════════════════════════════════════

public sealed record UserLoginRequest(
    string User,
    string Password
);

public sealed record UserLoginDto(
    Guid Id,
    string Email,
    string Phone,
    DateTime? EmailConfirmedAt,
    DateTime? ConfirmedAt
);

public sealed record LoginDto(
    string AccessToken,
    string TokenType,
    string ExpiresIn,
    string ExpiresAt,
    UserLoginDto User
);
