using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories;
using Intent.IntegrationTest.EfCore.SqlServer.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Intent.IntegrationTest.EfCore.SqlServer;

public class RepositoryTests : SharedDatabaseFixture<ApplicationDbContext, RepositoryTests>
{
    public RepositoryTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [IgnoreOnCiBuildFact]
    public async Task Test_Repository_FindAsyncWithIQueryable()
    {
        var repo = new AggregateRoot1Repository(DbContext);
        
        repo.Add(new AggregateRoot1(){Tag = "Internet"});
        repo.Add(new AggregateRoot1(){Tag = "Cloud"});
        await repo.UnitOfWork.SaveChangesAsync();

        var element = await repo.FindAsync(x => true, q => q.OrderBy(o => o.Tag).Take(1));
        Assert.Equal("Cloud", element.Tag);
    }
}