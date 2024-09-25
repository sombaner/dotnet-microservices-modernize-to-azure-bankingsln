namespace Common.KeyVault;

/// <summary>
/// Interface for accessing Azure Key Vault secrets
/// </summary>
public interface IKeyVaultSecretProvider
{
    /// <summary>
    /// Gets a secret from Azure Key Vault
    /// </summary>
    /// <param name="secretName">The name of the secret</param>
    /// <returns>The secret value</returns>
    Task<string> GetSecretAsync(string secretName);

    /// <summary>
    /// Gets a secret from Azure Key Vault, or returns the default value if not found
    /// </summary>
    /// <param name="secretName">The name of the secret</param>
    /// <param name="defaultValue">The default value if the secret is not found</param>
    /// <returns>The secret value, or the default value if not found</returns>
    Task<string> GetSecretOrDefaultAsync(string secretName, string defaultValue);
}