using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CqrsSpike.Scenarios.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace CqrsSpike.Scenarios;

/// <summary>
/// Runtime scenarios shared, unchanged, by the MediatR baseline and the Mediator variant. They
/// talk to the apps over HTTP only, so the same assertions prove behavioural parity.
/// </summary>
[Collection(SpikeCollection.Name)]
public class Scenarios
{
    private static readonly TimeSpan BrokerTimeout = TimeSpan.FromSeconds(30);
    private readonly SpikeFixture _fixture;
    private readonly ITestOutputHelper _output;

    public Scenarios(SpikeFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    private HttpClient Publisher(string? user = null, string? roles = null)
    {
        var client = _fixture.Publisher.CreateClient();
        if (user is not null) client.DefaultRequestHeaders.Add("X-Test-User", user);
        if (roles is not null) client.DefaultRequestHeaders.Add("X-Test-Roles", roles);
        return client;
    }

    private HttpClient Subscriber() => _fixture.Subscriber.CreateClient();

    private static async Task<Guid> CreateOrderAsync(HttpClient client, string customer, decimal total)
    {
        var response = await client.PostAsJsonAsync("api/orders", new { customerName = customer, total });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        // Generated controllers return the bare id; JsonResponse-wrapped ids are accepted too.
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.ValueKind == JsonValueKind.Object ? body.GetProperty("value").GetGuid() : body.GetGuid();
    }

    private static async Task<JsonElement> WaitForAsync(Func<Task<HttpResponseMessage>> call, Func<JsonElement, bool> until, string what)
    {
        var deadline = DateTime.UtcNow + BrokerTimeout;
        string last = "<no response>";
        while (DateTime.UtcNow < deadline)
        {
            var response = await call();
            last = $"{(int)response.StatusCode} {await response.Content.ReadAsStringAsync()}";
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadFromJsonAsync<JsonElement>();
                if (until(json)) return json;
            }
            await Task.Delay(250);
        }
        throw new TimeoutException($"Timed out waiting for {what}. Last response: {last}");
    }

    // ------------------------------------------------------------------ S1: CRUD commands/queries
    [Fact]
    public async Task S1_Crud_CommandsAndQueries_RoundTripThroughDispatcher()
    {
        var client = Publisher();
        var id = await CreateOrderAsync(client, "Alice", 10m);

        var created = await client.GetFromJsonAsync<JsonElement>($"api/orders/{id}");
        Assert.Equal("Alice", created.GetProperty("customerName").GetString());
        Assert.Equal(10m, created.GetProperty("total").GetDecimal());

        var update = await client.PutAsJsonAsync($"api/orders/{id}", new { id, customerName = "Alicia", total = 12.5m });
        Assert.Equal(HttpStatusCode.NoContent, update.StatusCode);

        var updated = await client.GetFromJsonAsync<JsonElement>($"api/orders/{id}");
        Assert.Equal("Alicia", updated.GetProperty("customerName").GetString());
        Assert.Equal(12.5m, updated.GetProperty("total").GetDecimal());

        var all = await client.GetFromJsonAsync<JsonElement>("api/orders");
        Assert.Contains(all.EnumerateArray(), o => o.GetProperty("id").GetGuid() == id);

        var missing = await client.GetAsync($"api/orders/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }

    // ------------------------------------------------------------------ S2: validation behaviour
    [Fact]
    public async Task S2_Validation_RejectsInvalidCommand_BeforeHandlerRuns()
    {
        var client = Publisher();
        var response = await client.PostAsJsonAsync("api/orders", new { customerName = "", total = 0m });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = (await response.Content.ReadAsStringAsync()).ToLowerInvariant();
        Assert.Contains("customername", problem);
        Assert.Contains("total", problem);

        // The handler never ran: nothing was buffered for the bus and nothing was committed.
        var entries = _fixture.Recorder.ForTrace(TraceOf(response)).Select(e => e.What).ToList();
        _output.WriteLine(string.Join(" -> ", entries));
        Assert.DoesNotContain(entries, e => e.StartsWith("publish:") || e.StartsWith("send:") || e == "commit");
    }

    // ------------------------------------------------------------------ S3: authorization behaviour
    [Fact]
    public async Task S3_Authorization_SecuredCommandRequiresRole()
    {
        var id = await CreateOrderAsync(Publisher(), "Bob", 5m);

        // The generated CurrentUserService reports no user for an anonymous caller -> 401.
        var anonymous = await Publisher().DeleteAsync($"api/orders/{id}");
        Assert.Equal(HttpStatusCode.Unauthorized, anonymous.StatusCode);

        var wrongRole = await Publisher("bob", "Clerk").DeleteAsync($"api/orders/{id}");
        Assert.Equal(HttpStatusCode.Forbidden, wrongRole.StatusCode);

        var admin = await Publisher("root", "Admin").DeleteAsync($"api/orders/{id}");
        Assert.Equal(HttpStatusCode.OK, admin.StatusCode);

        var gone = await Publisher().GetAsync($"api/orders/{id}");
        Assert.Equal(HttpStatusCode.NotFound, gone.StatusCode);
    }

    // ------------------------------------------------------------------ S4: domain events
    [Fact]
    public async Task S4_DomainEvent_HandlerDispatchesNestedCommand()
    {
        var customer = $"Carol-{Guid.NewGuid():N}";
        await CreateOrderAsync(Publisher(), customer, 7m);

        var audit = await Publisher().GetFromJsonAsync<JsonElement>("api/audit-entries");
        Assert.Contains(audit.EnumerateArray(), a => a.GetProperty("description").GetString() == $"Order placed for {customer}");
    }

    // ------------------------------------------------------------------ S5/S6: integration round trip
    [Fact]
    public async Task S5_S6_Integration_PublishSendAndHandlerDispatch_AcrossBothApps()
    {
        var customer = $"Dave-{Guid.NewGuid():N}";
        var orderId = await CreateOrderAsync(Publisher(), customer, 99m);

        // Publisher published OrderCreatedEvent -> Subscriber integration EVENT handler -> RecordOrderCommand
        var record = await WaitForAsync(() => Subscriber().GetAsync($"api/order-records/{orderId}"), _ => true, "OrderRecord in Subscriber");
        Assert.Equal(customer, record.GetProperty("customerName").GetString());

        // Publisher sent RequestShipmentCommand -> Subscriber integration COMMAND handler -> CreateShipmentCommand
        var shipment = await WaitForAsync(() => Subscriber().GetAsync($"api/shipments/by-order/{orderId}"), _ => true, "Shipment in Subscriber");
        var shipmentId = shipment.GetProperty("id").GetGuid();

        // CreateShipmentCommand published ShipmentCreatedEvent -> Publisher integration handler -> MarkOrderShippedCommand
        var order = await WaitForAsync(() => Publisher().GetAsync($"api/orders/{orderId}"),
            o => o.GetProperty("status").GetString() == "Shipped", "Order marked Shipped in Publisher");
        Assert.Equal(shipmentId, order.GetProperty("shipmentId").GetGuid());
        _output.WriteLine($"Round trip complete: order {orderId} -> shipment {shipmentId}");
    }

    // ------------------------------------------------------------------ S7: pipeline ordering
    [Fact]
    public async Task S7_Pipeline_BusFlushHappensAfterUnitOfWorkCommit()
    {
        var response = await Publisher().PostAsJsonAsync("api/orders", new { customerName = "Erin", total = 1m });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var entries = _fixture.Recorder.ForTrace(TraceOf(response)).Select(e => e.What).ToList();
        _output.WriteLine(string.Join(" -> ", entries));
        Assert.Contains("publish:OrderCreatedEvent", entries);
        Assert.Contains("send:RequestShipmentCommand", entries);

        var commit = entries.LastIndexOf("commit");
        var flush = entries.FindLastIndex(e => e.StartsWith("flush:"));
        Assert.True(commit >= 0, "UnitOfWork commit not observed");
        Assert.True(flush > commit, $"Bus flush must follow the commit: {string.Join(" -> ", entries)}");
        Assert.Contains("flush:2", entries);
    }

    [Fact]
    public async Task S8_Pipeline_UnitOfWorkWrapsCommandsOnly()
    {
        var id = await CreateOrderAsync(Publisher(), "Frank", 3m);
        var response = await Publisher().GetAsync($"api/orders/{id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // The query passes the bus-flush behaviour (flush:0) but must never reach UnitOfWork.
        var entries = _fixture.Recorder.ForTrace(TraceOf(response)).Select(e => e.What).ToList();
        _output.WriteLine(string.Join(" -> ", entries));
        Assert.Contains("flush:0", entries);
        Assert.DoesNotContain("commit", entries);
    }

    // ------------------------------------------------------------------ S9: FastEndpoints host
    [Fact]
    public async Task S9_FastEndpoints_CommandValidationAndQueries()
    {
        var invalid = await Subscriber().PostAsJsonAsync("api/shipments", new { orderId = Guid.Empty });
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);

        var orderId = Guid.NewGuid();
        var created = await Subscriber().PostAsJsonAsync("api/shipments", new { orderId });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var shipment = await Subscriber().GetFromJsonAsync<JsonElement>($"api/shipments/by-order/{orderId}");
        Assert.Equal(orderId, shipment.GetProperty("orderId").GetGuid());

        var all = await Subscriber().GetFromJsonAsync<JsonElement>("api/shipments");
        Assert.Contains(all.EnumerateArray(), s => s.GetProperty("orderId").GetGuid() == orderId);

        var missing = await Subscriber().GetAsync($"api/shipments/by-order/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }

    private static string TraceOf(HttpResponseMessage response) =>
        response.Headers.TryGetValues(TraceHeaderStartupFilter.HeaderName, out var values)
            ? values.Single()
            : throw new InvalidOperationException("Trace header missing");
}
