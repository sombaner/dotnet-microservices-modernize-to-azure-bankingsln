using Common.KeyVault;
using Customer.Application.Contracts.Persistence;
using Customer.Infrastructure.Persistence;
using Customer.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext with a factory that resolves connection string at runtime
            services.AddDbContext<CustomerContext>((serviceProvider, options) =>
            {
                var connectionStringManager = serviceProvider.GetRequiredService<ConnectionStringManager>();
                var connectionString = connectionStringManager.GetConnectionStringAsync("CustomerConnectionString").GetAwaiter().GetResult();
                options.UseNpgsql(connectionString);
            });
            
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            return services;
        }
    }
}
