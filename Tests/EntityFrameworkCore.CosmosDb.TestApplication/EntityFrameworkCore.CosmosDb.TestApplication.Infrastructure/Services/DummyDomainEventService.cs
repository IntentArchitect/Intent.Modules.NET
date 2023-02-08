using System.Threading.Tasks;
using EntityFrameworkCore.CosmosDb.TestApplication.Application.Common.Interfaces;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Services;

public class DummyDomainEventService : IDomainEventService
{
    public Task Publish(DomainEvent domainEvent)
    {
        return Task.CompletedTask;
    }
}