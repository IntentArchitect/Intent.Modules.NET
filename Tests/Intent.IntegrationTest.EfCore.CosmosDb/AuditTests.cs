using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.BasicAudit;
using EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence;
using Intent.IntegrationTest.EfCore.CosmosDb.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Intent.IntegrationTest.EfCore.CosmosDb;

[Collection(CollectionFixture.CollectionDefinitionName)]
public class AuditTests
{
    private readonly DataContainerFixture _fixture;

    public AuditTests(DataContainerFixture fixture)
    {
        _fixture = fixture;
    }
    
    private ApplicationDbContext DbContext => _fixture.DbContext;

    [IgnoreOnCiBuildFact]
    public async Task TestBasicAuditing()
    {
        var soloClass = new Audit_SoloClass
        {
            SoloAttr = "Test entry",
            PartitionKey = "ABC123"
        };
        
        DbContext.Audit_SoloClasses.Add(soloClass);
        await DbContext.SaveChangesAsync();

        var existingClass = DbContext.Audit_SoloClasses.FirstOrDefault(p => p.Id == soloClass.Id);
        Assert.NotNull(existingClass);
        Assert.Equal("user@test.com", existingClass.CreatedBy);
        Assert.NotNull(existingClass.CreatedDate);

        soloClass.SoloAttr += " Modify";
        await DbContext.SaveChangesAsync();
        
        var existingClassModify = DbContext.Audit_SoloClasses.FirstOrDefault(p => p.Id == soloClass.Id);
        Assert.NotNull(existingClassModify);
        Assert.Equal("user@test.com", existingClassModify.UpdatedBy);
        Assert.NotNull(existingClassModify.UpdatedDate);
    }
}