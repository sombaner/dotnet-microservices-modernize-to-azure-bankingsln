using Common.KeyVault;
using Common.Logging;
using Customer.GRPC.Extensions;
using Customer.GRPC.Services;
using Customer.Infrastructure;
using Customer.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add Key Vault configuration
var keyVaultUri = builder.Configuration["KeyVaultSettings:KeyVaultUri"];
if (!string.IsNullOrEmpty(keyVaultUri))
{
    builder.Configuration.AddAzureKeyVault(keyVaultUri);
}

builder.Services.AddKeyVaultSecretProvider(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<ConnectionStringManager>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var secretProvider = sp.GetRequiredService<IKeyVaultSecretProvider>();
    bool useKeyVault = config.GetValue<bool>("UseKeyVault");
    return new ConnectionStringManager(config, secretProvider, useKeyVault);
});

builder.Services.AddGrpc();
builder.Host.UseSerilog(SeriLogger.Configure);
var app = builder.Build();

app.MigrateDatabase<CustomerContext>((context, services) =>
{
    var logger = services.GetService<ILogger<CustomerContextSeed>>();
    CustomerContextSeed
        .SeedAsync(context, logger!)
        .Wait();
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


app.UseRouting();

app.MapGrpcService<CustomerService>();
app.MapGet("/", async context =>
{
    await context.Response.WriteAsync("Customer gRPC service.");
});

app.Run();

