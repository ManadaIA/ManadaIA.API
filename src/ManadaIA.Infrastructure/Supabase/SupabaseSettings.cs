namespace ManadaIA.Infrastructure.Supabase;

public sealed class SupabaseSettings
{
    public const string SectionName = "Supabase";

    public string Url { get; init; } = string.Empty;
    public string AnonKey { get; init; } = string.Empty;
    public string ServiceRoleKey { get; init; } = string.Empty;
}
