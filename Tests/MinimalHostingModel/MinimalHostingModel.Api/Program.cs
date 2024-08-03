using System;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalHostingModel.Api;
using MinimalHostingModel.Api.Configuration;
using MinimalHostingModel.Api.Filters;
using MinimalHostingModel.Application;
using MinimalHostingModel.Infrastructure;
using Serilog;
using Serilog.Events;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Program", Version = "1.0")]

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services));

    builder.Services.AddControllers(
        opt =>
        {
            opt.Filters.Add<ExceptionFilter>();
        })
    .AddOData(options =>
    {
        options.Filter().OrderBy().Select().SetMaxTop(200);
    });
    builder.Services.AddApplication(builder.Configuration);
    builder.Services.ConfigureApplicationSecurity(builder.Configuration);
    builder.Services.ConfigureCors(builder.Configuration);
    builder.Services.ConfigureHealthChecks(builder.Configuration);
    builder.Services.ConfigureProblemDetails();
    builder.Services.ConfigureApiVersioning();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.ConfigureSwagger(builder.Configuration);
    builder.Services.ConfigureMultiTenancy(builder.Configuration);

    // Add services to the container.

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseSerilogRequestLogging();
    app.UseExceptionHandler();
    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseMultiTenancy();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseCors();
    app.MapDefaultHealthChecks();
    app.MapDefaultHealthChecksUI();
    app.MapControllers();
    app.UseSwashbuckle(builder.Configuration);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
