using Account.Application.Contracts.Persistence;
using Account.Infrastructure.Persistence;
using Account.Infrastructure.Repositories;
using Common.KeyVault;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Account.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext with a factory that resolves connection string at runtime
            services.AddDbContext<AccountContext>((serviceProvider, options) =>
            {
                var connectionStringManager = serviceProvider.GetRequiredService<ConnectionStringManager>();
                var connectionString = connectionStringManager.GetConnectionStringAsync("AccountConnectionString").GetAwaiter().GetResult();
                options.UseNpgsql(connectionString);
            });
            
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddScoped<IAccountRepository, AccountRepository>();

            return services;
        }
    }
}
