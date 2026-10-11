using System.Security.Claims;
using System.Text.Encodings.Web;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PublisherEventBus = CqrsSpike.Publisher.Application.Common.Eventing.IMessageBus;
using PublisherMessageBus = CqrsSpike.Publisher.Infrastructure.Eventing.MassTransitMessageBus;
using PublisherDbContext = CqrsSpike.Publisher.Infrastructure.Persistence.ApplicationDbContext;
using PublisherUnitOfWork = CqrsSpike.Publisher.Domain.Common.Interfaces.IUnitOfWork;

namespace CqrsSpike.Scenarios.Infrastructure;

internal static class FactoryConfiguration
{
    public static void Apply(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RabbitMq:Host"] = "localhost",
                ["RabbitMq:VirtualHost"] = "/",
                ["RabbitMq:Username"] = SpikeFixture.BrokerUser,
                ["RabbitMq:Password"] = SpikeFixture.BrokerPassword,
                ["MassTransit:RetryInterval:RetryCount"] = "2",
                ["MassTransit:RetryInterval:Interval"] = "00:00:00.500",
            });
        });
        builder.ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning));
        builder.ConfigureTestServices(services =>
        {
            // Host start (and so CreateClient) returns only once the bus is connected and its
            // queues and bindings exist, so no scenario can publish into a missing binding.
            services.Configure<MassTransitHostOptions>(options =>
            {
                options.WaitUntilStarted = true;
                options.StartTimeout = TimeSpan.FromSeconds(30);
            });
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                    options.DefaultScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });
    }
}

public sealed class PublisherFactory : WebApplicationFactory<CqrsSpike.Publisher.Api.Program>
{
    private readonly PipelineRecorder _recorder;

    public PublisherFactory(PipelineRecorder recorder)
    {
        _recorder = recorder;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        FactoryConfiguration.Apply(builder);
        builder.ConfigureTestServices(services =>
        {
            services.AddHttpContextAccessor();
            services.AddSingleton(_recorder);
            services.AddTransient<IStartupFilter, TraceHeaderStartupFilter>();
            // Decorate (never replace) the real collaborators so the pipeline can be observed
            // while the real broker round trip keeps working.
            services.AddScoped<PublisherEventBus>(sp => new RecordingEventBus(
                sp.GetRequiredService<PublisherMessageBus>(), _recorder, sp));
            services.AddScoped<PublisherUnitOfWork>(sp => new RecordingUnitOfWork(
                sp.GetRequiredService<PublisherDbContext>(), _recorder, sp));
        });
    }
}

public sealed class SubscriberFactory : WebApplicationFactory<CqrsSpike.Subscriber.Api.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        FactoryConfiguration.Apply(builder);
    }
}

/// <summary>
/// Echoes HttpContext.TraceIdentifier as a response header so a test can correlate pipeline
/// recordings with the request that produced them.
/// </summary>
public sealed class TraceHeaderStartupFilter : IStartupFilter
{
    public const string HeaderName = "X-Spike-Trace";

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => app =>
    {
        app.Use(async (context, nextMiddleware) =>
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers[HeaderName] = context.TraceIdentifier;
                return Task.CompletedTask;
            });
            await nextMiddleware();
        });
        next(app);
    };
}

/// <summary>
/// Header-driven authentication for tests: X-Test-User marks the caller as authenticated and
/// X-Test-Roles (comma separated) supplies role claims. No header = anonymous.
/// </summary>
public sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "Test";

    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Test-User", out var user))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new List<Claim> { new("sub", user.ToString()), new("name", user.ToString()) };
        if (Request.Headers.TryGetValue("X-Test-Roles", out var roles))
        {
            claims.AddRange(roles.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(r => new Claim(ClaimTypes.Role, r.Trim())));
        }

        var identity = new ClaimsIdentity(claims, SchemeName);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName)));
    }
}
