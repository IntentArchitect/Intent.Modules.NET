using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Npgsql;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
[assembly: IntentTemplate("Intent.Services.Guardrails.Contracts.ServiceContract", Version = "1.0")]

namespace eShop.OrderProcessor.Services;

public interface IGracePeriodOrdersRepository
{
    ValueTask<List<int>> GetConfirmedGracePeriodOrdersAsync(
        TimeSpan gracePeriod,
        CancellationToken cancellationToken);
}

internal sealed class GracePeriodOrdersRepository(
    NpgsqlDataSource dataSource,
    ILogger<GracePeriodOrdersRepository> logger) : IGracePeriodOrdersRepository
{
    public async ValueTask<List<int>> GetConfirmedGracePeriodOrdersAsync(
        TimeSpan gracePeriod,
        CancellationToken cancellationToken)
    {
        try
        {
            await using var connection = dataSource.CreateConnection();
            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT "Id"
                FROM ordering.orders
                WHERE CURRENT_TIMESTAMP - "OrderDate" >= @GracePeriodTime AND "OrderStatus" = 'Submitted'
                """;
            command.Parameters.AddWithValue("GracePeriodTime", gracePeriod);

            List<int> ids = [];
            await connection.OpenAsync(cancellationToken);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                ids.Add(reader.GetInt32(0));
            }

            return ids;
        }
        catch (NpgsqlException exception)
        {
            logger.LogError(exception, "Error querying confirmed grace period orders after grace period {GracePeriod}", gracePeriod);
            return [];
        }
    }
}
