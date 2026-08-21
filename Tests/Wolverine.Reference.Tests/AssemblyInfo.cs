using Xunit;

// The reference tests share one RabbitMQ + SQL Server Testcontainer pair per test class via a
// collection fixture, and each test starts/stops its own in-process publisher/subscriber IHost
// pair against real queues. Running them in parallel would race on the same durable queues.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
