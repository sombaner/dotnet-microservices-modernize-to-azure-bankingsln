using Microsoft.Extensions.Configuration;

namespace Common.KeyVault;

/// <summary>
/// Helper class to manage connection strings with fallback to configuration
/// </summary>
public class ConnectionStringManager
{
    private readonly IConfiguration _configuration;
    private readonly IKeyVaultSecretProvider _secretProvider;
    private readonly bool _useKeyVault;

    /// <summary>
    /// Creates a new instance of ConnectionStringManager
    /// </summary>
    /// <param name="configuration">The application configuration</param>
    /// <param name="secretProvider">The Key Vault secret provider</param>
    /// <param name="useKeyVault">Whether to use Key Vault or not</param>
    public ConnectionStringManager(
        IConfiguration configuration, 
        IKeyVaultSecretProvider secretProvider,
        bool useKeyVault = true)
    {
        _configuration = configuration;
        _secretProvider = secretProvider;
        _useKeyVault = useKeyVault;
    }

    /// <summary>
    /// Gets a connection string from Key Vault if available, or from configuration
    /// </summary>
    /// <param name="connectionStringName">The connection string name</param>
    /// <returns>The connection string value</returns>
    public async Task<string> GetConnectionStringAsync(string connectionStringName)
    {
        if (_useKeyVault)
        {
            try
            {
                return await _secretProvider.GetSecretOrDefaultAsync(
                    connectionStringName,
                    _configuration.GetConnectionString(connectionStringName) ?? string.Empty);
            }
            catch
            {
                // Fallback to configuration if Key Vault access fails
                return _configuration.GetConnectionString(connectionStringName) ?? string.Empty;
            }
        }
        
        return _configuration.GetConnectionString(connectionStringName) ?? string.Empty;
    }
}