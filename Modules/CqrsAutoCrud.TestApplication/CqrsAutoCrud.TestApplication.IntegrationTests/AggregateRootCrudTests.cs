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
using CqrsAutoCrud.TestApplication.Infrastructure.Persistence;
using CqrsAutoCrud.TestApplication.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace CqrsAutoCrud.TestApplication.IntegrationTests;

[UsesVerify]
public class AggregateRootCrudTests : SharedDatabaseFixture<ApplicationDbContext, AggregateRootCrudTests>
{
    public AggregateRootCrudTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [IgnoreOnCiBuildFact]
    public async Task Test_CreateCommand()
    {
        var command = new CreateAggregateRootCommand();
        command.AggregateAttr = "Create Aggregate Root";
        command.Composite = CreateCompositeSingleADTO.Create(
            Guid.Empty,
            command.AggregateAttr + "_1",
            CreateCompositeSingleAADTO.Create(default, command.AggregateAttr + "_2"),
            new List<CreateCompositeManyAADTO>
            {
                CreateCompositeManyAADTO.Create(default, command.AggregateAttr + "_3", default)
            });
        command.Composites = new List<CreateCompositeManyBDTO>
        {
            CreateCompositeManyBDTO.Create(
                Guid.Empty,
                command.AggregateAttr + "_4",
                Guid.Empty, 
                CreateCompositeSingleBBDTO.Create(default, command.AggregateAttr + "_5"),
                new List<CreateCompositeManyBBDTO>
                {
                    CreateCompositeManyBBDTO.Create(default, command.AggregateAttr + "_6", default)
                })
        };
        command.Aggregate = CreateAggregateSingleCDTO.Create(default, command.AggregateAttr + "_7");

        var handler = new CreateAggregateRootCommandHandler(new AggregateRootRepository(DbContext));
        var id = await handler.Handle(command, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var actual = await DbContext.AggregateRoots.FindAsync(id);
        Assert.NotNull(actual);
        await Verifier.Verify(actual);
    }

    [IgnoreOnCiBuildFact]
    public async Task Test_UpdateCommand()
    {
        var root = new AggregateRoot();
        root.AggregateAttr = "Update Aggregate Root";
        root.Composites = new List<CompositeManyB>
        {
            new CompositeManyB
            {
                Id = new Guid("0483ae48-4606-45ec-8ca4-66355aa5bbbf"),
                CompositeAttr = root.AggregateAttr + "_1",
                SomeDate = DateTime.Parse("2022/11/08 12:00:00")
            },
            new CompositeManyB
            {
                Id = new Guid("d34db3c5-2db3-47e3-aa7c-1fb8b7cb52df"),
                CompositeAttr = root.AggregateAttr + "_2",
                SomeDate = DateTime.Parse("2022/11/08 12:10:00")
            }
        };
        root.Composite = new CompositeSingleA();
        root.Composite.CompositeAttr = root.AggregateAttr + "_2";
        DbContext.AggregateRoots.Add(root);
        await DbContext.SaveChangesAsync();
        
        var command = new UpdateAggregateRootCommand();
        command.Id = root.Id;
        command.AggregateAttr = root.AggregateAttr;
        command.Composite = UpdateCompositeSingleADTO.Create(default, command.AggregateAttr + "_3",
            UpdateCompositeSingleAADTO.Create(default, command.AggregateAttr + "_4"), 
            new List<UpdateCompositeManyAADTO>
            {
                UpdateCompositeManyAADTO.Create(default, command.AggregateAttr + "_5", default)
            });
        command.Composites = new List<UpdateCompositeManyBDTO>
        {
            UpdateCompositeManyBDTO.Create(
                new Guid("0483ae48-4606-45ec-8ca4-66355aa5bbbf"),
                command.AggregateAttr + "_6",
                default,
                UpdateCompositeSingleBBDTO.Create(default, command.AggregateAttr + "_7"),
                new List<UpdateCompositeManyBBDTO>
                {
                    UpdateCompositeManyBBDTO.Create(default, command.AggregateAttr + "_8", default)
                }),
            UpdateCompositeManyBDTO.Create(
                new Guid("d34db3c5-2db3-47e3-aa7c-1fb8b7cb52df"),
                command.AggregateAttr + "_8",
                default,
                UpdateCompositeSingleBBDTO.Create(default, command.AggregateAttr + "_9"),
                new List<UpdateCompositeManyBBDTO>
                {
                    UpdateCompositeManyBBDTO.Create(default, command.AggregateAttr + "_10", default)
                }),
            UpdateCompositeManyBDTO.Create(
                default,
                command.AggregateAttr + "_11",
                default,
                UpdateCompositeSingleBBDTO.Create(default, command.AggregateAttr + "_12"),
                new List<UpdateCompositeManyBBDTO>
                {
                    UpdateCompositeManyBBDTO.Create(default, command.AggregateAttr + "_13", default)
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
        createCommand.CompositeOfAggrLong = CreateCompositeOfAggrLongDTO.Create(default, createCommand.Attribute + "_1");

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
        updateCommand.CompositeOfAggrLong = UpdateCompositeOfAggrLongDTO.Create(default, updateCommand.Attribute + "_2");

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