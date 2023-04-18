using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Application.Common.Interfaces;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Common;

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Services;

public class DummyDomainEventService : IDomainEventService
{
    public Task Publish(DomainEvent domainEvent)
    {
        return Task.CompletedTask;
    }
}