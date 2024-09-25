using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Options;

namespace Common.KeyVault;

/// <summary>
/// Implementation of the KeyVault secret provider
/// </summary>
public class KeyVaultSecretProvider : IKeyVaultSecretProvider
{
    private readonly SecretClient _secretClient;
    private readonly KeyVaultSettings _settings;

    public KeyVaultSecretProvider(IOptions<KeyVaultSettings> settings)
    {
        _settings = settings.Value;

        if (!string.IsNullOrEmpty(_settings.KeyVaultUri))
        {
            var credential = new DefaultAzureCredential();
            _secretClient = new SecretClient(new Uri(_settings.KeyVaultUri), credential);
        }
    }

    /// <inheritdoc/>
    public async Task<string> GetSecretAsync(string secretName)
    {
        if (string.IsNullOrEmpty(_settings.KeyVaultUri))
        {
            throw new InvalidOperationException("KeyVault URI is not configured");
        }

        var secret = await _secretClient.GetSecretAsync(secretName);
        return secret.Value.Value;
    }

    /// <inheritdoc/>
    public async Task<string> GetSecretOrDefaultAsync(string secretName, string defaultValue)
    {
        if (string.IsNullOrEmpty(_settings.KeyVaultUri))
        {
            return defaultValue;
        }

        try
        {
            var secret = await _secretClient.GetSecretAsync(secretName);
            return secret.Value.Value;
        }
        catch
        {
            return defaultValue;
        }
    }
}