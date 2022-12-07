using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Application.AggregateRoots;
using CqrsAutoCrud.TestApplication.Application.AggregateRoots.CreateAggregateRootCompositeManyB;
using CqrsAutoCrud.TestApplication.Application.AggregateRoots.DeleteAggregateRootCompositeManyB;
using CqrsAutoCrud.TestApplication.Application.AggregateRoots.UpdateAggregateRootCompositeManyB;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using CqrsAutoCrud.TestApplication.Infrastructure.Persistence;
using CqrsAutoCrud.TestApplication.Infrastructure.Repositories;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace CqrsAutoCrud.TestApplication.IntegrationTests;

[UsesVerify]
public class NestedComposition_Many_CrudTests : SharedDatabaseFixture<ApplicationDbContext, NestedComposition_Many_CrudTests>
{
    public NestedComposition_Many_CrudTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [IgnoreOnCiBuildFact]
    public async Task Test_CreateNestedCompositionCommand_NewComposition()
    {
        var aggregateRoot = new AggregateRoot();
        aggregateRoot.AggregateAttr = "Existing Aggregate Root - Create";
        DbContext.AggregateRoots.Add(aggregateRoot);
        await DbContext.SaveChangesAsync();
        
        var command = new CreateAggregateRootCompositeManyBCommand();
        command.AggregateRootId = aggregateRoot.Id;
        command.CompositeAttr = "New Nested Composition_1";
        command.Composite = CreateAggregateRootCompositeManyBCompositeSingleBBDTO.Create("New Nested Composition_2");
        command.Composites = new List<CreateAggregateRootCompositeManyBCompositeManyBBDTO>
        {
            CreateAggregateRootCompositeManyBCompositeManyBBDTO.Create("New Nested Composition_3", default)
        };

        var handler = new CreateAggregateRootCompositeManyBCommandHandler(new AggregateRootRepository(DbContext));
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
        aggregateRoot.Composites.Add(new CompositeManyB
        {
            CompositeAttr = "Existing Composite_1 - Create",
            Composite = new CompositeSingleBB()
            {
                CompositeAttr = "Existing Composite_2"
            },
            Composites = new List<CompositeManyBB>
            {
                new CompositeManyBB()
                {
                    CompositeAttr = "Existing Composite_3"
                }
            }
        });
        DbContext.AggregateRoots.Add(aggregateRoot);
        await DbContext.SaveChangesAsync();
        
        var command = new CreateAggregateRootCompositeManyBCommand();
        command.AggregateRootId = aggregateRoot.Id;
        command.CompositeAttr = "New Nested Composition_4";
        command.Composite = CreateAggregateRootCompositeManyBCompositeSingleBBDTO.Create("New Nested Composition_5");
        command.Composites = new List<CreateAggregateRootCompositeManyBCompositeManyBBDTO>
        {
            CreateAggregateRootCompositeManyBCompositeManyBBDTO.Create("New Nested Composition_6", default)
        };

        var handler = new CreateAggregateRootCompositeManyBCommandHandler(new AggregateRootRepository(DbContext));
        var id = await handler.Handle(command, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var retrievedAggrRoot = await DbContext.AggregateRoots.FindAsync(aggregateRoot.Id);
        Assert.NotNull(retrievedAggrRoot);
        await Verifier.Verify(retrievedAggrRoot);
    }

    [IgnoreOnCiBuildFact]
    public async Task Test_CreateNestedCompositionCommand_AggregateRootNotFound()
    {
        var command = new CreateAggregateRootCompositeManyBCommand();
        command.AggregateRootId = Guid.NewGuid();
        command.CompositeAttr = "New Nested Composition_4 - Create";
        command.Composite = CreateAggregateRootCompositeManyBCompositeSingleBBDTO.Create("New Nested Composition_5");
        command.Composites = new List<CreateAggregateRootCompositeManyBCompositeManyBBDTO>
        {
            CreateAggregateRootCompositeManyBCompositeManyBBDTO.Create("New Nested Composition_6", default)
        };

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var handler = new CreateAggregateRootCompositeManyBCommandHandler(new AggregateRootRepository(DbContext));
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
        
        var command = new UpdateAggregateRootCompositeManyBCommand();
        command.AggregateRootId = aggregateRoot.Id;
        command.Id = default;
        command.CompositeAttr = "New Nested Composition_1";
        command.Composite = UpdateAggregateRootCompositeManyBCompositeSingleBBDTO.Create(default, "New Nested Composition_2");
        command.Composites = new List<UpdateAggregateRootCompositeManyBCompositeManyBBDTO>
        {
            UpdateAggregateRootCompositeManyBCompositeManyBBDTO.Create(default, "New Nested Composition_3", default)
        };

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var handler = new UpdateAggregateRootCompositeManyBCommandHandler(new AggregateRootRepository(DbContext));
            await handler.Handle(command, CancellationToken.None);
            await DbContext.SaveChangesAsync();    
        });
    }
    
    [IgnoreOnCiBuildFact]
    public async Task Test_UpdateNestedCompositionCommand_ExistingComposition()
    {
        var aggregateRoot = new AggregateRoot();
        aggregateRoot.AggregateAttr = "Existing Aggregate Root - Update";
        aggregateRoot.Composites.Add(new CompositeManyB
        {
            CompositeAttr = "Existing Composite_1 - Create",
            Composite = new CompositeSingleBB()
            {
                CompositeAttr = "Existing Composite_2"
            },
            Composites = new List<CompositeManyBB>
            {
                new CompositeManyBB()
                {
                    CompositeAttr = "Existing Composite_3"
                }
            }
        });
        DbContext.AggregateRoots.Add(aggregateRoot);
        await DbContext.SaveChangesAsync();
        
        var command = new UpdateAggregateRootCompositeManyBCommand();
        command.AggregateRootId = aggregateRoot.Id;
        command.Id = aggregateRoot.Composites.First().Id;
        command.CompositeAttr = "New Nested Composition_1";
        command.Composite = UpdateAggregateRootCompositeManyBCompositeSingleBBDTO.Create(default, "New Nested Composition_2");
        command.Composites = new List<UpdateAggregateRootCompositeManyBCompositeManyBBDTO>
        {
            UpdateAggregateRootCompositeManyBCompositeManyBBDTO.Create(default, "New Nested Composition_3", default)
        };

        var handler = new UpdateAggregateRootCompositeManyBCommandHandler(new AggregateRootRepository(DbContext));
        await handler.Handle(command, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var retrievedAggrRoot = await DbContext.AggregateRoots.FindAsync(aggregateRoot.Id);
        Assert.NotNull(retrievedAggrRoot);
        await Verifier.Verify(retrievedAggrRoot);
    }
    
    [IgnoreOnCiBuildFact]
    public async Task Test_UpdateNestedCompositionCommand_AggregateRootNotFound()
    {
        var command = new UpdateAggregateRootCompositeManyBCommand();
        command.AggregateRootId = Guid.NewGuid();
        command.CompositeAttr = "New Nested Composition_4 - Update";
        command.Composite = UpdateAggregateRootCompositeManyBCompositeSingleBBDTO.Create(default, "New Nested Composition_5");
        command.Composites = new List<UpdateAggregateRootCompositeManyBCompositeManyBBDTO>
        {
            UpdateAggregateRootCompositeManyBCompositeManyBBDTO.Create(default, "New Nested Composition_6", default)
        };

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var handler = new UpdateAggregateRootCompositeManyBCommandHandler(new AggregateRootRepository(DbContext));
            await handler.Handle(command, CancellationToken.None);
            await DbContext.SaveChangesAsync();    
        });
    }

    [IgnoreOnCiBuildFact]
    public async Task Test_DeleteNestedComposititionCommand_ExistingComposition()
    {
        var aggregateRoot = new AggregateRoot();
        aggregateRoot.AggregateAttr = "Existing Aggregate Root - Update";
        aggregateRoot.Composites.Add(new CompositeManyB
        {
            CompositeAttr = "Existing Composite_1 - Create",
            Composite = new CompositeSingleBB()
            {
                CompositeAttr = "Existing Composite_2"
            },
            Composites = new List<CompositeManyBB>
            {
                new CompositeManyBB()
                {
                    CompositeAttr = "Existing Composite_3"
                }
            }
        });
        DbContext.AggregateRoots.Add(aggregateRoot);
        await DbContext.SaveChangesAsync();
        
        var command = new DeleteAggregateRootCompositeManyBCommand();
        command.AggregateRootId = aggregateRoot.Id;
        command.Id = aggregateRoot.Composites.First().Id;
        
        var handler = new DeleteAggregateRootCompositeManyBCommandHandler(new AggregateRootRepository(DbContext));
        await handler.Handle(command, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var retrievedAggrRoot = DbContext.AggregateRoots.FirstOrDefault(p => p.Id == aggregateRoot.Id);
        Assert.NotNull(retrievedAggrRoot);
        await Verifier.Verify(retrievedAggrRoot);
    }
}