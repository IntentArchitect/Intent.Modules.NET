using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.BasicAudit;
using EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence;
using Intent.IntegrationTest.EfCore.SqlServer.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Intent.IntegrationTest.EfCore.SqlServer;

public class AuditTests : SharedDatabaseFixture<ApplicationDbContext, AuditTests>
{
    public AuditTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [IgnoreOnCiBuildFact]
    public async Task TestBasicAuditing()
    {
        var soloClass = new Audit_SoloClass
        {
            SoloAttr = "Test entry"
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