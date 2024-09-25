using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.KeyVault;

public static class KeyVaultExtensions
{
    /// <summary>
    /// Adds Azure Key Vault configuration to the application
    /// </summary>
    /// <param name="builder">The configuration builder</param>
    /// <param name="keyVaultUri">The URI of the Key Vault</param>
    /// <returns>The updated configuration builder</returns>
    public static IConfigurationBuilder AddAzureKeyVault(
        this IConfigurationBuilder builder, 
        string keyVaultUri)
    {
        if (string.IsNullOrEmpty(keyVaultUri))
        {
            return builder;
        }

        var uri = new Uri(keyVaultUri);
        var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
        {
            ExcludeSharedTokenCacheCredential = true
        });

        builder.AddAzureKeyVault(uri, credential, new AzureKeyVaultConfigurationOptions
        {
            ReloadInterval = TimeSpan.FromMinutes(5)
        });

        return builder;
    }

    /// <summary>
    /// Adds the Key Vault secret provider to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddKeyVaultSecretProvider(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var keyVaultSettings = new KeyVaultSettings();
        configuration.GetSection(KeyVaultSettings.SectionName).Bind(keyVaultSettings);

        services.Configure<KeyVaultSettings>(options =>
        {
            options.KeyVaultUri = keyVaultSettings.KeyVaultUri;
        });

        services.AddSingleton<IKeyVaultSecretProvider, KeyVaultSecretProvider>();

        return services;
    }
}