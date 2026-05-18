using Azure.Identity;
using Microsoft.Azure.Cosmos;
using OutboxSaga.Api.Endpoints;
using OutboxSaga.Application.Pedidos;
using OutboxSaga.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ─── Azure Key Vault ─────────────────────────────────────────────────────────
// Lê as credenciais do Service Principal do appsettings/env vars
// e adiciona o Key Vault como configuration provider.
// Os secrets do KV sobrescrevem qualquer valor do appsettings.json.
var kvSection = builder.Configuration.GetSection("AzureKeyVault");
var keyVaultUri = kvSection["Uri"];

if (!string.IsNullOrEmpty(keyVaultUri))
{
    var tenantId     = kvSection["TenantId"]     ?? throw new InvalidOperationException("AzureKeyVault:TenantId não configurado.");
    var clientId     = kvSection["ClientId"]     ?? throw new InvalidOperationException("AzureKeyVault:ClientId não configurado.");
    var clientSecret = kvSection["ClientSecret"] ?? throw new InvalidOperationException("AzureKeyVault:ClientSecret não configurado.");

    var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), credential);
}
// ─────────────────────────────────────────────────────────────────────────────

// Add services to the container.
builder.Services.AddSingleton<CosmosClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config["CosmosDB:ConnectionString"];
    return new CosmosClient(connectionString);
});
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<CreatePedidoHandler>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapPedidosEndpoints();

app.Run();
