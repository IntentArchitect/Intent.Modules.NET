using BlazorServerTests.Api.Common;
using BlazorServerTests.Api.Components;
using BlazorServerTests.Api.Components.Account;
using BlazorServerTests.Api.Configuration;
using BlazorServerTests.Api.Logging;
using BlazorServerTests.Application;
using BlazorServerTests.Infrastructure;
using BlazorServerTests.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Serilog;
using Serilog.Events;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Program", Version = "1.0")]

namespace BlazorServerTests.Api
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
                builder.Services.AddApplication(builder.Configuration);
                builder.Services.ConfigureApplicationSecurity(builder.Configuration);
                builder.Services.ConfigureHealthChecks(builder.Configuration);
                builder.Services.ConfigureIdentity();
                builder.Services.ConfigureProblemDetails();
                builder.Services.AddInfrastructure(builder.Configuration);

                builder.Host.UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Destructure.With(new BoundedLoggingDestructuringPolicy()));
                builder.Services.AddCascadingAuthenticationState();
                builder.Services.AddHttpContextAccessor();
                builder.Services.AddScoped<IdentityRedirectManager>();
                builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
                builder.Services.AddScoped<IAuthService, AspNetCoreIdentityAuthServiceConcrete>();
                builder.Services.AddAuthorization();
                builder.Services.AddScoped<IdentityUserAccessor>();
                builder.Services.AddScoped<IdentityRedirectManager>();
                builder.Services.AddAuthentication(options =>
                                        {
                                            options.DefaultScheme = IdentityConstants.ApplicationScheme;
                                            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                                        }).AddIdentityCookies();
                builder.Services.AddSingleton<IEmailSender<IdentityUser>, IdentityNoOpEmailSender>();

                builder.Services.AddRazorComponents()
                    .AddInteractiveServerComponents();

                builder.Services.AddMudServices();

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
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();
                app.UseStaticFiles();
                app.UseAntiforgery();
                app.MapDefaultHealthChecks();

                app.MapRazorComponents<App>()
                    .AddInteractiveServerRenderMode();
                app.MapAdditionalIdentityEndpoints();

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