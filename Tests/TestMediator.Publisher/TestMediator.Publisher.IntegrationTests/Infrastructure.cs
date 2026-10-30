using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text.Encodings.Web;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using Xunit;
using TestMediator.Publisher.Application.Common.Eventing;
using TestMediator.Publisher.Domain.Common.Interfaces;
using TestMediator.Publisher.Infrastructure.Eventing;
using TestMediator.Publisher.Infrastructure.Persistence;

namespace TestMediator.Publisher.IntegrationTests;

public sealed record PipelineEntry(long Sequence, string? TraceId, string What);

/// <summary>Records bus buffering, unit-of-work commits and bus flushes, tagged with the HTTP request that caused them.</summary>
public sealed class PipelineRecorder
{
    private long _sequence;
    private readonly ConcurrentQueue<PipelineEntry> _entries = new();

    public void Record(IServiceProvider sp, string what)
    {
        var traceId = sp.GetService<IHttpContextAccessor>()?.HttpContext?.TraceIdentifier;
        _entries.Enqueue(new PipelineEntry(Interlocked.Increment(ref _sequence), traceId, what));
    }

    public IReadOnlyList<PipelineEntry> ForTrace(string traceId) =>
        _entries.Where(e => e.TraceId == traceId).OrderBy(e => e.Sequence).ToList();
}

internal sealed class RecordingMessageBus : IMessageBus
{
    private readonly IMessageBus _inner;
    private readonly PipelineRecorder _recorder;
    private readonly IServiceProvider _sp;
    private int _buffered;

    public RecordingMessageBus(IMessageBus inner, PipelineRecorder recorder, IServiceProvider sp)
    {
        _inner = inner;
        _recorder = recorder;
        _sp = sp;
    }

    public void Publish<TMessage>(TMessage message) where TMessage : class
    {
        _buffered++;
        _recorder.Record(_sp, $"publish:{typeof(TMessage).Name}");
        _inner.Publish(message);
    }

    public void Send<TMessage>(TMessage message) where TMessage : class
    {
        _buffered++;
        _recorder.Record(_sp, $"send:{typeof(TMessage).Name}");
        _inner.Send(message);
    }

    public void Send<TMessage>(TMessage message, Uri address) where TMessage : class
    {
        _buffered++;
        _recorder.Record(_sp, $"send:{typeof(TMessage).Name}");
        _inner.Send(message, address);
    }

    public async Task FlushAllAsync(CancellationToken cancellationToken = default)
    {
        _recorder.Record(_sp, $"flush:{_buffered}");
        _buffered = 0;
        await _inner.FlushAllAsync(cancellationToken);
    }
}

internal sealed class RecordingUnitOfWork : IUnitOfWork
{
    private readonly IUnitOfWork _inner;
    private readonly PipelineRecorder _recorder;
    private readonly IServiceProvider _sp;

    public RecordingUnitOfWork(IUnitOfWork inner, PipelineRecorder recorder, IServiceProvider sp)
    {
        _inner = inner;
        _recorder = recorder;
        _sp = sp;
    }

    private int _saves;

    /// <summary>When set, the second SaveChanges in a consumer scope throws: the consumer's own save, after the nested command committed.</summary>
    public static volatile bool FailSecondSave;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _saves++;
        // Only a consumer scope (no HTTP request): an HTTP create also saves twice now that it raises a domain event.
        if (FailSecondSave && _saves == 2 && _sp.GetService<IHttpContextAccessor>()?.HttpContext is null)
        {
            _recorder.Record(_sp, "save-failed");
            throw new InvalidOperationException("Injected failure on the consumer's own SaveChanges.");
        }

        var result = await _inner.SaveChangesAsync(cancellationToken);
        _recorder.Record(_sp, "commit");
        return result;
    }

    public bool HasDbTransaction() => _inner.HasDbTransaction();
}

/// <summary>Echoes HttpContext.TraceIdentifier as a response header so a test can correlate recordings with its request.</summary>
public sealed class TraceHeaderStartupFilter : IStartupFilter
{
    public const string HeaderName = "X-Test-Trace";

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

/// <summary>Header-driven authentication: X-Test-User marks the caller authenticated, X-Test-Roles supplies role claims.</summary>
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

public sealed class PublisherFactory : WebApplicationFactory<TestMediator.Publisher.Api.Program>
{
    private readonly PipelineRecorder _recorder;
    private readonly string _database;

    public PublisherFactory(PipelineRecorder recorder, string database)
    {
        _recorder = recorder;
        _database = database;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _database,
                ["RabbitMq:Host"] = "localhost",
                ["RabbitMq:VirtualHost"] = "/",
                ["RabbitMq:Username"] = Fixture.BrokerUser,
                ["RabbitMq:Password"] = Fixture.BrokerPassword,
                ["MassTransit:RetryInterval:RetryCount"] = "2",
                ["MassTransit:RetryInterval:Interval"] = "00:00:00.500",
            });
        });
        builder.ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning));
        builder.ConfigureTestServices(services =>
        {
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
            services.AddHttpContextAccessor();
            services.AddSingleton(_recorder);
            services.AddTransient<IStartupFilter, TraceHeaderStartupFilter>();
            // Decorate (never replace) the real collaborators so the pipeline can be observed while the real broker round trip keeps working.
            services.AddScoped<IMessageBus>(sp => new RecordingMessageBus(sp.GetRequiredService<MassTransitMessageBus>(), _recorder, sp));
            services.AddScoped<IUnitOfWork>(sp => new RecordingUnitOfWork(sp.GetRequiredService<ApplicationDbContext>(), _recorder, sp));
        });
    }
}

/// <summary>Starts (or reuses) RabbitMQ on localhost:5672 and a SQL Server container, then boots the Publisher in-process.</summary>
public sealed class Fixture : IAsyncLifetime
{
    public const string BrokerUser = "guest";
    public const string BrokerPassword = "guest";

    private RabbitMqContainer? _broker;
    private MsSqlContainer? _sqlServer;

    public PipelineRecorder Recorder { get; } = new();
    public PublisherFactory Publisher { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        if (!await IsPortOpenAsync("localhost", 5672))
        {
            _broker = new RabbitMqBuilder("rabbitmq:3.13-management")
                .WithUsername(BrokerUser)
                .WithPassword(BrokerPassword)
                .WithPortBinding(5672, 5672)
                .Build();
            await _broker.StartAsync();
        }

        _sqlServer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();
        await _sqlServer.StartAsync();
        var database = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(_sqlServer.GetConnectionString())
        {
            InitialCatalog = "TestMediatorPublisher"
        }.ConnectionString;

        await using (var db = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(database).Options, null!))
        {
            await db.Database.EnsureCreatedAsync();
        }

        Publisher = new PublisherFactory(Recorder, database);
        _ = Publisher.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await Publisher.DisposeAsync();
        if (_broker is not null)
        {
            await _broker.DisposeAsync();
        }

        if (_sqlServer is not null)
        {
            await _sqlServer.DisposeAsync();
        }
    }

    private static async Task<bool> IsPortOpenAsync(string host, int port)
    {
        try
        {
            using var client = new TcpClient();
            var connect = client.ConnectAsync(host, port);
            return await Task.WhenAny(connect, Task.Delay(500)) == connect && client.Connected;
        }
        catch
        {
            return false;
        }
    }
}

[CollectionDefinition(Name)]
public sealed class FixtureCollection : ICollectionFixture<Fixture>
{
    public const string Name = "publisher";
}
