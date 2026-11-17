using BuildingBlocks.Api.Scheduling;
using Catalog.Api;
using Host.Api.Composition;
using Identity.Api;
using Orders.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPlatform(builder.Configuration)
    .AddIdentityModule(builder.Configuration)
    .AddCatalogModule(builder.Configuration)
    .AddOrdersModule(builder.Configuration);

var app = builder.Build();

app.UseReverseProxySupport();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors(CorsSetup.PolicyName);
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapApiDocumentation();
app.MapHealthChecks("/health");
app.MapOutboxTick();
app.MapIdentityModule();
app.MapCatalogModule();
app.MapOrdersModule();

await app.RunAsync();

/// <summary>Lets the integration tests start the real host.</summary>
public partial class Program;
