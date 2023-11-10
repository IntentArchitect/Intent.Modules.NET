using System.Data.Common;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Services;
using Intent.IntegrationTest.EfCore.SqlServer.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Intent.IntegrationTest.EfCore.SqlServer;

public class RepositoryTests : SharedDatabaseFixture<ApplicationDbContext, RepositoryTests>
{
    public RepositoryTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    protected override ApplicationDbContext CreateContext(DbTransaction transaction = null)
    {
        return new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .LogTo(OutputHelper.WriteLine)
                .UseSqlServer(Connection)
                .EnableSensitiveDataLogging()
                .Options,
            new DummyDomainEventService());
    }

    [IgnoreOnCiBuildFact]
    public async Task Test_Repository_FindAsyncWithIQueryable()
    {
        var repo = new AggregateRoot1Repository(DbContext, null);
        
        repo.Add(new AggregateRoot1(){Tag = "Internet"});
        repo.Add(new AggregateRoot1(){Tag = "Cloud"});
        await repo.UnitOfWork.SaveChangesAsync();

        var element = await repo.FindAsync(x => true, q => q.OrderBy(o => o.Tag).Take(1));
        Assert.Equal("Cloud", element.Tag);
    }
}