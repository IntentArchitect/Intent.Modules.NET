using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CqrsAutoCrud.TestApplication.Application.AggregateRootLongs;
using CqrsAutoCrud.TestApplication.Application.AggregateRootLongs.CreateAggregateRootLong;
using CqrsAutoCrud.TestApplication.Application.AggregateRootLongs.UpdateAggregateRootLong;
using CqrsAutoCrud.TestApplication.Application.AggregateRoots;
using CqrsAutoCrud.TestApplication.Application.AggregateRoots.CreateAggregateRoot;
using CqrsAutoCrud.TestApplication.Application.AggregateRoots.UpdateAggregateRoot;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using CqrsAutoCrud.TestApplication.Domain.Entities.Common;
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
        var command = new CreateAggregateRootCommand();
        command.AggregateAttr = "Aggregate Root " + Guid.NewGuid();
        command.Composite = CompositeSingleADTO.Create(
            Guid.Empty,
            command.AggregateAttr + "_" + Guid.NewGuid(),
            CompositeSingleAADTO.Create(default, command.AggregateAttr + "_" + Guid.NewGuid()),
            new List<CompositeManyAADTO>
            {
                CompositeManyAADTO.Create(default, command.AggregateAttr + "_" + Guid.NewGuid(), default)
            });
        command.Composites = new List<CompositeManyBDTO>
        {
            CompositeManyBDTO.Create(
                Guid.Empty,
                command.AggregateAttr + "_" + Guid.NewGuid(),
                Guid.Empty, 
                CompositeSingleBBDTO.Create(default, command.AggregateAttr + "_" + Guid.NewGuid()),
                new List<CompositeManyBBDTO>
                {
                    CompositeManyBBDTO.Create(default, command.AggregateAttr + "_" + Guid.NewGuid(), default)
                })
        };
        command.Aggregate = AggregateSingleCDTO.Create(default, command.AggregateAttr + "_" + Guid.NewGuid());

        var handler = new CreateAggregateRootCommandHandler(new AggregateRootRepository(DbContext));
        await handler.Handle(command, CancellationToken.None);

        var actual = await DbContext.AggregateRoots.FirstAsync(p => p.AggregateAttr == command.AggregateAttr);
        Assert.NotNull(actual);
        Assert.Equal(command.Composite.CompositeAttr, actual.Composite.CompositeAttr);
        Assert.Equal(command.Composite.Composite.CompositeAttr, actual.Composite.Composite.CompositeAttr);
        Assert.Equal(command.Composite.Composites.First().CompositeAttr, actual.Composite.Composites.First().CompositeAttr);
        Assert.Equal(command.Composites.First().Composite.CompositeAttr, actual.Composites.First().Composite.CompositeAttr);
        Assert.Equal(command.Composites.First().Composites.First().CompositeAttr, actual.Composites.First().Composites.First().CompositeAttr);
        Assert.Null(actual.Aggregate);
    }

    [IgnoreOnCiBuildFact]
    public async Task Test_UpdateCommand()
    {
        var root = new AggregateRoot();
        root.Id = IdentityGenerator.NewSequentialId();
        root.AggregateAttr = "Aggregate Root " + Guid.NewGuid();
        root.Composites = new List<CompositeManyB>();
        root.Composite = new CompositeSingleA();
        root.Composite.Id = IdentityGenerator.NewSequentialId();
        root.Composite.CompositeAttr = root.AggregateAttr + "_" + Guid.NewGuid();
        DbContext.AggregateRoots.Add(root);
        await DbContext.SaveChangesAsync();
        
        var command = new UpdateAggregateRootCommand();
        command.Id = root.Id;
        command.AggregateAttr = root.AggregateAttr;
        command.Composite = CompositeSingleADTO.Create(default, command.AggregateAttr + "_" + Guid.NewGuid(),
            CompositeSingleAADTO.Create(default, command.AggregateAttr + "_" + Guid.NewGuid()), 
            new List<CompositeManyAADTO>
            {
                CompositeManyAADTO.Create(default, command.AggregateAttr + "_" + Guid.NewGuid(), default)
            });
        command.Composites = new List<CompositeManyBDTO>
        {
            CompositeManyBDTO.Create(
                default,
                command.AggregateAttr + "_" + Guid.NewGuid(),
                default,
                CompositeSingleBBDTO.Create(default, command.AggregateAttr + "_" + Guid.NewGuid()),
                new List<CompositeManyBBDTO>
                {
                    CompositeManyBBDTO.Create(default, command.AggregateAttr + "_" + Guid.NewGuid(), default)
                }),
            CompositeManyBDTO.Create(
                default,
                command.AggregateAttr + "_" + Guid.NewGuid(),
                default,
                CompositeSingleBBDTO.Create(default, command.AggregateAttr + "_" + Guid.NewGuid()),
                new List<CompositeManyBBDTO>
                {
                    CompositeManyBBDTO.Create(default, command.AggregateAttr + "_" + Guid.NewGuid(), default)
                })
        };
        
        var handler = new UpdateAggregateRootCommandHandler(new AggregateRootRepository(DbContext));
        await handler.Handle(command, CancellationToken.None);
        
        var actual = await DbContext.AggregateRoots.FirstAsync(p => p.AggregateAttr == command.AggregateAttr);
        Assert.NotNull(actual);
        Assert.Equal(command.Composite.CompositeAttr, actual.Composite.CompositeAttr);
        Assert.Equal(command.Composite.Composite.CompositeAttr, actual.Composite.Composite.CompositeAttr);
        Assert.Equal(command.Composite.Composites.First().CompositeAttr, actual.Composite.Composites.First().CompositeAttr);
        Assert.Equal(command.Composites.Count, actual.Composites.Count);
        Assert.Equal(command.Composites.First().Composite.CompositeAttr, actual.Composites.First().Composite.CompositeAttr);
        Assert.Equal(command.Composites.First().Composites.First().CompositeAttr, actual.Composites.First().Composites.First().CompositeAttr);
        Assert.Null(actual.Aggregate);
        
        command.Composites.RemoveAt(0);
        handler = new UpdateAggregateRootCommandHandler(new AggregateRootRepository(DbContext));
        await handler.Handle(command, CancellationToken.None);
        
        actual = await DbContext.AggregateRoots.FirstAsync(p => p.AggregateAttr == command.AggregateAttr);
        Assert.Equal(command.Composites.Count, actual.Composites.Count);
    }

    [IgnoreOnCiBuildFact]
    public async Task Test_CreateAndUpdate_WithLongPk()
    {
        var createCommand = new CreateAggregateRootLongCommand();
        createCommand.Attribute = "Long PK Aggr Root " + Guid.NewGuid();
        createCommand.CompositeOfAggrLong = CompositeOfAggrLongDTO.Create(default, createCommand.Attribute + "_" + Guid.NewGuid());

        var createHandler = new CreateAggregateRootLongCommandHandler(new AggregateRootLongRepository(DbContext));
        var id = await createHandler.Handle(createCommand, CancellationToken.None);
        
        Assert.Equal(1, DbContext.AggregateRootLongs.Count());
        var actual = await DbContext.AggregateRootLongs.FirstAsync(p => p.Id == id);
        Assert.Equal(createCommand.Attribute, actual.Attribute);
        Assert.Equal(createCommand.CompositeOfAggrLong.Attribute, actual.CompositeOfAggrLong.Attribute);
        
        var updateCommand = new UpdateAggregateRootLongCommand();
        updateCommand.Id = id;
        updateCommand.Attribute = createCommand.Attribute + "ZZZ";
        updateCommand.CompositeOfAggrLong = CompositeOfAggrLongDTO.Create(default, updateCommand.Attribute + "_" + Guid.NewGuid());

        var updateHandler = new UpdateAggregateRootLongCommandHandler(new AggregateRootLongRepository(DbContext));
        await updateHandler.Handle(updateCommand, CancellationToken.None);

        Assert.Equal(1, DbContext.AggregateRootLongs.Count());
        actual = await DbContext.AggregateRootLongs.FirstAsync(p => p.Id == id);
        Assert.NotNull(actual);
        Assert.Equal(updateCommand.Attribute, actual.Attribute);
        Assert.Equal(updateCommand.CompositeOfAggrLong.Attribute, actual.CompositeOfAggrLong.Attribute);
    }
}