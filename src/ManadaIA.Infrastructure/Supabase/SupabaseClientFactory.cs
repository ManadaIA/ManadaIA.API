using Supabase;

namespace ManadaIA.Infrastructure.Supabase;

public static class SupabaseClientFactory
{
    public static async Task<Client> CreateAsync(SupabaseSettings settings, string accessToken)
    {
        var options = new SupabaseOptions
        {
            AutoConnectRealtime = false,
            AutoRefreshToken = false,
            Headers = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {accessToken}" }
            }
        };

        var client = new Client(
            settings.Url,
            settings.AnonKey,
            options
        );

        await client.InitializeAsync();

        return client;
    }
}