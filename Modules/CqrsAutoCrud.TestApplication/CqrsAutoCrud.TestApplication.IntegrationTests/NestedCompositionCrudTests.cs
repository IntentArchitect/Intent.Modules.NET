using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeSingleAs;
using CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeSingleAs.CreateAggregateRootCompositeSingleA;
using CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeSingleAs.GetAggregateRootCompositeSingleAById;
using CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeSingleAs.UpdateAggregateRootCompositeSingleA;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using CqrsAutoCrud.TestApplication.Infrastructure.Persistence;
using CqrsAutoCrud.TestApplication.Infrastructure.Repositories;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace CqrsAutoCrud.TestApplication.IntegrationTests;

[UsesVerify]
public class NestedCompositionCrudTests : SharedDatabaseFixture<ApplicationDbContext, NestedCompositionCrudTests>
{
    public NestedCompositionCrudTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [IgnoreOnCiBuildFact]
    public async Task Test_CreateNestedCompositionCommand_NewComposition()
    {
        var aggregateRoot = new AggregateRoot();
        aggregateRoot.AggregateAttr = "Existing Aggregate Root - Create";
        DbContext.AggregateRoots.Add(aggregateRoot);
        await DbContext.SaveChangesAsync();
        
        var command = new CreateAggregateRootCompositeSingleACommand();
        command.AggregateRootId = aggregateRoot.Id;
        command.CompositeAttr = "New Nested Composition_1";
        command.Composite = CreateCompositeSingleAADTO.Create(default, "New Nested Composition_2");
        command.Composites = new List<CreateCompositeManyAADTO>
        {
            CreateCompositeManyAADTO.Create(default, "New Nested Composition_3", default)
        };

        var handler = new CreateAggregateRootCompositeSingleACommandHandler(new AggregateRootRepository(DbContext));
        var id = await handler.Handle(command, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var retrievedAggrRoot = await DbContext.AggregateRoots.FindAsync(aggregateRoot.Id);
        Assert.NotNull(retrievedAggrRoot);
        await Verifier.Verify(retrievedAggrRoot);
    }
    
    [IgnoreOnCiBuildFact]
    public async Task Test_CreateNestedCompositionCommand_ExistingComposition()
    {
        var aggregateRoot = new AggregateRoot();
        aggregateRoot.AggregateAttr = "Existing Aggregate Root - Create";
        aggregateRoot.Composite = new CompositeSingleA
        {
            CompositeAttr = "Existing Nested Composition_1",
            Composite = new CompositeSingleAA
            {
                CompositeAttr = "Existing Nested Composition_2"
            },
            Composites = new List<CompositeManyAA>
            {
                new()
                {
                    CompositeAttr = "Existing Nested Composition_3"
                }
            }
        };
        DbContext.AggregateRoots.Add(aggregateRoot);
        await DbContext.SaveChangesAsync();
        
        var command = new CreateAggregateRootCompositeSingleACommand();
        command.AggregateRootId = aggregateRoot.Id;
        command.CompositeAttr = "New Nested Composition_4";
        command.Composite = CreateCompositeSingleAADTO.Create(default, "New Nested Composition_5");
        command.Composites = new List<CreateCompositeManyAADTO>
        {
            CreateCompositeManyAADTO.Create(default, "New Nested Composition_6", default)
        };

        var handler = new CreateAggregateRootCompositeSingleACommandHandler(new AggregateRootRepository(DbContext));
        var id = await handler.Handle(command, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var retrievedAggrRoot = await DbContext.AggregateRoots.FindAsync(aggregateRoot.Id);
        Assert.NotNull(retrievedAggrRoot);
        await Verifier.Verify(retrievedAggrRoot);
    }

    [IgnoreOnCiBuildFact]
    public async Task Test_CreateNestedCompositionCommand_AggregateRootNotFound()
    {
        var command = new CreateAggregateRootCompositeSingleACommand();
        command.AggregateRootId = Guid.NewGuid();
        command.CompositeAttr = "New Nested Composition_4 - Create";
        command.Composite = CreateCompositeSingleAADTO.Create(default, "New Nested Composition_5");
        command.Composites = new List<CreateCompositeManyAADTO>
        {
            CreateCompositeManyAADTO.Create(default, "New Nested Composition_6", default)
        };

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var handler = new CreateAggregateRootCompositeSingleACommandHandler(new AggregateRootRepository(DbContext));
            var id = await handler.Handle(command, CancellationToken.None);
            await DbContext.SaveChangesAsync();    
        });
    }

    [IgnoreOnCiBuildFact]
    public async Task Test_UpdateNestedCompositionCommand_NoComposition()
    {
        var aggregateRoot = new AggregateRoot();
        aggregateRoot.AggregateAttr = "Existing Aggregate Root - Update";
        DbContext.AggregateRoots.Add(aggregateRoot);
        await DbContext.SaveChangesAsync();
        
        var command = new UpdateAggregateRootCompositeSingleACommand();
        command.AggregateRootId = aggregateRoot.Id;
        command.CompositeAttr = "New Nested Composition_1";
        command.Composite = UpdateCompositeSingleAADTO.Create(default, "New Nested Composition_2");
        command.Composites = new List<UpdateCompositeManyAADTO>
        {
            UpdateCompositeManyAADTO.Create(default, "New Nested Composition_3", default)
        };

        var handler = new UpdateAggregateRootCompositeSingleACommandHandler(new AggregateRootRepository(DbContext));
        await handler.Handle(command, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var retrievedAggrRoot = await DbContext.AggregateRoots.FindAsync(aggregateRoot.Id);
        Assert.NotNull(retrievedAggrRoot);
        await Verifier.Verify(retrievedAggrRoot);
    }
    
    [IgnoreOnCiBuildFact]
    public async Task Test_UpdateNestedCompositionCommand_ExistingComposition()
    {
        var aggregateRoot = new AggregateRoot();
        aggregateRoot.AggregateAttr = "Existing Aggregate Root - Update";
        aggregateRoot.Composite = new CompositeSingleA
        {
            CompositeAttr = "Existing Nested Composition_1",
            Composite = new CompositeSingleAA
            {
                CompositeAttr = "Existing Nested Composition_2"
            },
            Composites = new List<CompositeManyAA>
            {
                new()
                {
                    CompositeAttr = "Existing Nested Composition_3"
                }
            }
        };
        DbContext.AggregateRoots.Add(aggregateRoot);
        await DbContext.SaveChangesAsync();
        
        var command = new UpdateAggregateRootCompositeSingleACommand();
        command.AggregateRootId = aggregateRoot.Id;
        command.CompositeAttr = "New Nested Composition_1";
        command.Composite = UpdateCompositeSingleAADTO.Create(default, "New Nested Composition_2");
        command.Composites = new List<UpdateCompositeManyAADTO>
        {
            UpdateCompositeManyAADTO.Create(default, "New Nested Composition_3", default)
        };

        var handler = new UpdateAggregateRootCompositeSingleACommandHandler(new AggregateRootRepository(DbContext));
        await handler.Handle(command, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var retrievedAggrRoot = await DbContext.AggregateRoots.FindAsync(aggregateRoot.Id);
        Assert.NotNull(retrievedAggrRoot);
        await Verifier.Verify(retrievedAggrRoot);
    }
    
    [IgnoreOnCiBuildFact]
    public async Task Test_UpdateNestedCompositionCommand_AggregateRootNotFound()
    {
        var command = new UpdateAggregateRootCompositeSingleACommand();
        command.AggregateRootId = Guid.NewGuid();
        command.CompositeAttr = "New Nested Composition_4 - Update";
        command.Composite = UpdateCompositeSingleAADTO.Create(default, "New Nested Composition_5");
        command.Composites = new List<UpdateCompositeManyAADTO>
        {
            UpdateCompositeManyAADTO.Create(default, "New Nested Composition_6", default)
        };

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var handler = new UpdateAggregateRootCompositeSingleACommandHandler(new AggregateRootRepository(DbContext));
            await handler.Handle(command, CancellationToken.None);
            await DbContext.SaveChangesAsync();    
        });
    }
}