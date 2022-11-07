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
using VerifyTests;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace CqrsAutoCrud.TestApplication.IntegrationTests;

[UsesVerify]
public class CrudTests : SharedDatabaseFixture<ApplicationDbContext>
{
    public CrudTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [IgnoreOnCiBuildFact]
    public async Task Test_CreateCommand()
    {
        var command = new CreateAggregateRootCommand();
        command.AggregateAttr = "Aggregate Root";
        command.Composite = CompositeSingleADTO.Create(
            Guid.Empty,
            command.AggregateAttr + "_1",
            CompositeSingleAADTO.Create(default, command.AggregateAttr + "_2"),
            new List<CompositeManyAADTO>
            {
                CompositeManyAADTO.Create(default, command.AggregateAttr + "_3", default)
            });
        command.Composites = new List<CompositeManyBDTO>
        {
            CompositeManyBDTO.Create(
                Guid.Empty,
                command.AggregateAttr + "_4",
                Guid.Empty, 
                CompositeSingleBBDTO.Create(default, command.AggregateAttr + "_5"),
                new List<CompositeManyBBDTO>
                {
                    CompositeManyBBDTO.Create(default, command.AggregateAttr + "_6", default)
                })
        };
        command.Aggregate = AggregateSingleCDTO.Create(default, command.AggregateAttr + "_7");

        var handler = new CreateAggregateRootCommandHandler(new AggregateRootRepository(DbContext));
        await handler.Handle(command, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var actual = await DbContext.AggregateRoots.FirstAsync(p => p.AggregateAttr == command.AggregateAttr);
        Assert.NotNull(actual);
        await Verifier.Verify(actual);
    }

    [IgnoreOnCiBuildFact]
    public async Task Test_UpdateCommand()
    {
        var root = new AggregateRoot();
        root.AggregateAttr = "Aggregate Root";
        root.Composites = new List<CompositeManyB>
        {
            new CompositeManyB
            {
                Id = new Guid("0483ae48-4606-45ec-8ca4-66355aa5bbbf"),
                CompositeAttr = root.AggregateAttr + "_1"
            },
            new CompositeManyB
            {
                Id = new Guid("d34db3c5-2db3-47e3-aa7c-1fb8b7cb52df"),
                CompositeAttr = root.AggregateAttr + "_2"
            }
        };
        root.Composite = new CompositeSingleA();
        root.Composite.CompositeAttr = root.AggregateAttr + "_2";
        DbContext.AggregateRoots.Add(root);
        await DbContext.SaveChangesAsync();
        
        var command = new UpdateAggregateRootCommand();
        command.Id = root.Id;
        command.AggregateAttr = root.AggregateAttr;
        command.Composite = CompositeSingleADTO.Create(default, command.AggregateAttr + "_3",
            CompositeSingleAADTO.Create(default, command.AggregateAttr + "_4"), 
            new List<CompositeManyAADTO>
            {
                CompositeManyAADTO.Create(default, command.AggregateAttr + "_5", default)
            });
        command.Composites = new List<CompositeManyBDTO>
        {
            CompositeManyBDTO.Create(
                new Guid("0483ae48-4606-45ec-8ca4-66355aa5bbbf"),
                command.AggregateAttr + "_6",
                default,
                CompositeSingleBBDTO.Create(default, command.AggregateAttr + "_7"),
                new List<CompositeManyBBDTO>
                {
                    CompositeManyBBDTO.Create(default, command.AggregateAttr + "_8", default)
                }),
            CompositeManyBDTO.Create(
                new Guid("d34db3c5-2db3-47e3-aa7c-1fb8b7cb52df"),
                command.AggregateAttr + "_8",
                default,
                CompositeSingleBBDTO.Create(default, command.AggregateAttr + "_9"),
                new List<CompositeManyBBDTO>
                {
                    CompositeManyBBDTO.Create(default, command.AggregateAttr + "_10", default)
                }),
            CompositeManyBDTO.Create(
                default,
                command.AggregateAttr + "_11",
                default,
                CompositeSingleBBDTO.Create(default, command.AggregateAttr + "_12"),
                new List<CompositeManyBBDTO>
                {
                    CompositeManyBBDTO.Create(default, command.AggregateAttr + "_13", default)
                })
        };
        
        var handler = new UpdateAggregateRootCommandHandler(new AggregateRootRepository(DbContext));
        await handler.Handle(command, CancellationToken.None);
        await DbContext.SaveChangesAsync();
        
        var actual = await DbContext.AggregateRoots.FirstAsync(p => p.AggregateAttr == command.AggregateAttr);
        Assert.NotNull(actual);
        await Verifier.Verify(actual).UseTextForParameters("Before");
        
        command.Composites.RemoveAt(0);
        handler = new UpdateAggregateRootCommandHandler(new AggregateRootRepository(DbContext));
        await handler.Handle(command, CancellationToken.None);
        await DbContext.SaveChangesAsync();
        
        actual = await DbContext.AggregateRoots.FirstAsync(p => p.AggregateAttr == command.AggregateAttr);
        await Verifier.Verify(actual).UseTextForParameters("After");
    }

    [IgnoreOnCiBuildFact]
    public async Task Test_CreateAndUpdate_WithLongPk()
    {
        var createCommand = new CreateAggregateRootLongCommand();
        createCommand.Attribute = "Long PK Aggr Root";
        createCommand.CompositeOfAggrLong = CompositeOfAggrLongDTO.Create(default, createCommand.Attribute + "_1");

        var createHandler = new CreateAggregateRootLongCommandHandler(new AggregateRootLongRepository(DbContext));
        var id = await createHandler.Handle(createCommand, CancellationToken.None);
        await DbContext.SaveChangesAsync();
        
        Assert.Equal(1, DbContext.AggregateRootLongs.Count());
        var actual = await DbContext.AggregateRootLongs.FirstAsync(p => p.Id == id);
        Assert.Equal(createCommand.Attribute, actual.Attribute);
        Assert.Equal(createCommand.CompositeOfAggrLong.Attribute, actual.CompositeOfAggrLong.Attribute);
        
        var updateCommand = new UpdateAggregateRootLongCommand();
        updateCommand.Id = id;
        updateCommand.Attribute = createCommand.Attribute + "ZZZ";
        updateCommand.CompositeOfAggrLong = CompositeOfAggrLongDTO.Create(default, updateCommand.Attribute + "_2");

        var updateHandler = new UpdateAggregateRootLongCommandHandler(new AggregateRootLongRepository(DbContext));
        await updateHandler.Handle(updateCommand, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        Assert.Equal(1, DbContext.AggregateRootLongs.Count());
        actual = await DbContext.AggregateRootLongs.FirstAsync(p => p.Id == id);
        Assert.NotNull(actual);
        Assert.Equal(updateCommand.Attribute, actual.Attribute);
        Assert.Equal(updateCommand.CompositeOfAggrLong.Attribute, actual.CompositeOfAggrLong.Attribute);
    }
}