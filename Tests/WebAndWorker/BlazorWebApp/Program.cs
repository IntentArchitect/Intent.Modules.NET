using BlazorWebApp.Components;
using BlazorWebApp.Components.Services;
using BlazorWebApp.Configuration;
using BlazorWebApp.Filters;
using BlazorWebApp.Logging;
using Intent.RoslynWeaver.Attributes;
using MudBlazor.Services;
using Serilog;
using Serilog.Events;
using WebAndWorker.Application;
using WebAndWorker.Infrastructure;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Program", Version = "1.0")]

namespace BlazorWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                if (builder.Configuration.GetValue<bool?>("KeyVault:Enabled") == true)
                {
                    builder.Configuration.ConfigureAzureKeyVault(builder.Configuration);
                }

                builder.Host.UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Destructure.With(new BoundedLoggingDestructuringPolicy()));

                builder.Services.AddControllers(
                    opt =>
                    {
                        opt.Filters.Add<ExceptionFilter>();
                    });
                builder.Services.AddApplication(builder.Configuration);
                builder.Services.ConfigureApplicationSecurity(builder.Configuration);
                builder.Services.ConfigureHealthChecks(builder.Configuration);
                builder.Services.ConfigureProblemDetails();
                builder.Services.ConfigureApiVersioning();
                builder.Services.AddInfrastructure(builder.Configuration);
                builder.Services.ConfigureSwagger(builder.Configuration);

                builder.Services.AddRazorComponents()
                    .AddInteractiveServerComponents();

                builder.Services.AddMudServices();
                builder.Services.AddScoped<ThemeService>();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                app.UseSerilogRequestLogging();
                app.UseHttpsRedirection();

                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }
                app.UseStaticFiles();
                app.UseRouting();
                app.UseAntiforgery();
                app.MapDefaultHealthChecks();
                app.MapControllers();

                app.MapRazorComponents<App>()
                    .AddInteractiveServerRenderMode();
                app.UseSwashbuckle(builder.Configuration);

                logger.Write(LogEventLevel.Information, "Starting web host");

                app.Run();
            }
            catch (HostAbortedException)
            {
                // Excluding HostAbortedException from being logged, as this is an expected
                // exception when working with EF Core migrations (as per the .NET team on the below link)
                // https://github.com/dotnet/efcore/issues/29809#issuecomment-1344101370
            }
            catch (Exception ex)
            {
                logger.Write(LogEventLevel.Fatal, ex, "Unhandled exception");
            }
        }
    }
}