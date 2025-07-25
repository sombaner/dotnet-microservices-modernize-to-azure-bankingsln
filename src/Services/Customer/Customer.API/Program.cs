using Common.KeyVault;
using Common.Logging;
using Customer.API.Extensions;
using Customer.Application;
using Customer.Infrastructure;
using Customer.Infrastructure.Persistence;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add Key Vault configuration
var keyVaultUri = builder.Configuration["KeyVaultSettings:KeyVaultUri"];
if (!string.IsNullOrEmpty(keyVaultUri))
{
    builder.Configuration.AddAzureKeyVault(keyVaultUri);
}

builder.Services.AddKeyVaultSecretProvider(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<ConnectionStringManager>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var secretProvider = sp.GetRequiredService<IKeyVaultSecretProvider>();
    bool useKeyVault = config.GetValue<bool>("UseKeyVault");
    return new ConnectionStringManager(config, secretProvider, useKeyVault);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo { Title = "Customer.API", Version = "v1" });
});
builder.Services.AddHealthChecks()
    .AddDbContextCheck<CustomerContext>();
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
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer.API v1"));
}

//app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/hc", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
