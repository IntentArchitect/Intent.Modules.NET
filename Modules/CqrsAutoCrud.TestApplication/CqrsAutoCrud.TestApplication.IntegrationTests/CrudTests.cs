using System;
using System.Threading;
using System.Threading.Tasks;
using CqrsAutoCrud.TestApplication.Application.A_AggregateRoots;
using CqrsAutoCrud.TestApplication.Application.A_AggregateRoots.CreateA_AggregateRoot;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using CqrsAutoCrud.TestApplication.Infrastructure.Persistence;
using CqrsAutoCrud.TestApplication.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace CqrsAutoCrud.TestApplication.IntegrationTests;

public class CrudTests : SharedDatabaseFixture<ApplicationDbContext>
{
    public CrudTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [IgnoreOnCiBuildFact]
    public async Task Test_CreateCommand()
    {
        var command = new CreateA_AggregateRootCommand();
        command.AggregateAttr = "Aggregate Root " + Guid.NewGuid();
        command.Composite = A_Composite_SingleDTO.Create(
            Guid.Empty, 
            command.AggregateAttr, 
            AA1_Composite_SingleDTO.Create(Guid.Empty, command.AggregateAttr),
            null,
            null);
        
        
        var handler = new CreateA_AggregateRootCommandHandler(new A_AggregateRootRepository(DbContext));
        await handler.Handle(command, CancellationToken.None);

        var actual = await DbContext.A_AggregateRoots.FirstAsync(p => p.AggregateAttr == command.AggregateAttr);
        Assert.NotNull(actual);
        
    }
}