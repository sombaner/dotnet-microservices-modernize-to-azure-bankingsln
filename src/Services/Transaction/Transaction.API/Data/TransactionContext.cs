using Common.KeyVault;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Transaction.API.Data.Interfaces;

namespace Transaction.API.Data
{
    public class TransactionContext : ITransactionContext
    {
        public TransactionContext(IConfiguration configuration, ConnectionStringManager connectionStringManager)
        {
            // Get the connection string using ConnectionStringManager
            var connectionString = connectionStringManager.GetConnectionStringAsync("DatabaseSettings:ConnectionString").GetAwaiter().GetResult();
            
            // If connection string is empty, fall back to direct configuration value
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = configuration.GetValue<string>("DatabaseSettings:ConnectionString")!;
            }
            
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName")!);

            Transactions = database.GetCollection<Entities.Transaction>(configuration.GetValue<string>("DatabaseSettings:CollectionName")!);
            TransactionContextSeed.SeedData(Transactions);
        }

        public IMongoCollection<Entities.Transaction> Transactions { get; }
    }
}
