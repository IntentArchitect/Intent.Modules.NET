using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CqrsAutoCrud.TestApplication.Application.AggregateRootAS;
using CqrsAutoCrud.TestApplication.Application.AggregateRootAS.CreateAggregateRootA;
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
        var command = new CreateAggregateRootACommand();
        command.AggregateAttr = "Aggregate Root " + Guid.NewGuid();
        command.Composite = CompositeSingleAADTO.Create(
            Guid.Empty,
            command.AggregateAttr + "_" + Guid.NewGuid(),
            CompositeSingleAAA1DTO.Create(Guid.Empty, command.AggregateAttr + "_" + Guid.NewGuid()),
            new List<CompositeManyAAA1DTO>
            {
                CompositeManyAAA1DTO.Create(Guid.Empty, command.AggregateAttr + "_" + Guid.NewGuid(), Guid.Empty)
            });
        command.Composites = new List<CompositeManyAADTO>
        {
            CompositeManyAADTO.Create(
                Guid.Empty,
                command.AggregateAttr + "_" + Guid.NewGuid(),
                Guid.Empty, 
                CompositeSingleAAA2DTO.Create(Guid.Empty, command.AggregateAttr + "_" + Guid.NewGuid()),
                new List<CompositeManyAAA2DTO>
                {
                    CompositeManyAAA2DTO.Create(Guid.Empty, command.AggregateAttr + "_" + Guid.NewGuid(), Guid.Empty)
                })
        };

        var handler = new CreateAggregateRootACommandHandler(new AggregateRootARepository(DbContext));
        await handler.Handle(command, CancellationToken.None);

        var actual = await DbContext.AggregateRootAs.FirstAsync(p => p.AggregateAttr == command.AggregateAttr);
        Assert.NotNull(actual);
        Assert.Equal(command.Composite.CompositeAttr, actual.Composite.CompositeAttr);
        Assert.Equal(command.Composite.Composite.CompositeAttr, actual.Composite.Composite.CompositeAttr);
        Assert.Equal(command.Composite.Composites.First().CompositeAttr, actual.Composite.Composites.First().CompositeAttr);
        Assert.Equal(command.Composites.First().Composite.CompositeAttr, actual.Composites.First().Composite.CompositeAttr);
        Assert.Equal(command.Composites.First().Composites.First().CompositeAttr, actual.Composites.First().Composites.First().CompositeAttr);
    }
}