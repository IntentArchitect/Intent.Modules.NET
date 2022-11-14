using System.Linq;
using System.Threading.Tasks;
using EfCoreRepositoryTestSuite.IntentGenerated.Core;
using EfCoreRepositoryTestSuite.IntentGenerated.Entities;
using EfCoreRepositoryTestSuite.IntentGenerated.Repositories;
using Xunit;
using Xunit.Abstractions;

namespace EfCoreTestSuite.IntegrationTests;

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