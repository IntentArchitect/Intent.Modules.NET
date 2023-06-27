using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.TestApplication.Application.Common.Interfaces;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;

namespace EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Services;

public class DummyDomainEventService : IDomainEventService
{
    public Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}