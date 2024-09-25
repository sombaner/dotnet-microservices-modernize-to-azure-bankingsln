namespace Common.KeyVault;

/// <summary>
/// Settings for Azure Key Vault
/// </summary>
public class KeyVaultSettings
{
    /// <summary>
    /// The section name in the configuration
    /// </summary>
    public const string SectionName = "KeyVaultSettings";

    /// <summary>
    /// The URI of the Key Vault
    /// </summary>
    public string KeyVaultUri { get; set; } = string.Empty;
}