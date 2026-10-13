using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Probe.Application;
using Probe.Domain;

var services = new ServiceCollection();
services.AddApplication();
var sp = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

using (var scope = sp.CreateScope())
{
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    var db = scope.ServiceProvider.GetRequiredService<FakeDbContext>();
    Log.Lines.Add($"scope db={db.InstanceId}");
    var id = await sender.Send(new CreateOrderCommand("x"));
    await sender.Send(new DeleteOrderCommand(id));
    await sender.Send(new GetOrderQuery(id));
    Log.Lines.Add($"saves={db.Saves}");

    var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
    DomainEvent evt = new OrderCreatedDomainEvent(id);
    try { await publisher.Publish(new OrderCreatedDomainEventNotification((OrderCreatedDomainEvent)evt)); Log.Lines.Add("publish closed subclass returned"); } catch (Exception ex) { Log.Lines.Add("threw: " + ex.GetType().Name + ": " + ex.Message); }
}
foreach (var l in Log.Lines) Console.WriteLine(l);
