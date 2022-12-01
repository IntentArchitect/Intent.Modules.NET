using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Application.AggregateRootComposite;
using CqrsAutoCrud.TestApplication.Application.AggregateRootComposite.CreateAggregateRootComposite;
using CqrsAutoCrud.TestApplication.Application.AggregateRootComposite.DeleteAggregateRootComposite;
using CqrsAutoCrud.TestApplication.Application.AggregateRootComposite.UpdateAggregateRootComposite;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using CqrsAutoCrud.TestApplication.Infrastructure.Persistence;
using CqrsAutoCrud.TestApplication.Infrastructure.Repositories;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace CqrsAutoCrud.TestApplication.IntegrationTests;

// This test class was used to explore the scenario for 1->1 associations
// but since we're not going with this use case we won't be "needing" this anymore.
// However, I'm keeping this just in case.
[UsesVerify]
public class NestedComposition_Single_CrudTests : SharedDatabaseFixture<ApplicationDbContext, NestedComposition_Single_CrudTests>
{
    public NestedComposition_Single_CrudTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }
    
    //[IgnoreOnCiBuildFact]
    public async Task Test_CreateNestedCompositionCommand_NewComposition()
    {
        var aggregateRoot = new AggregateRoot();
        aggregateRoot.AggregateAttr = "Existing Aggregate Root - Create";
        DbContext.AggregateRoots.Add(aggregateRoot);
        await DbContext.SaveChangesAsync();
        
        var command = new CreateAggregateRootCompositeCommand();
        command.AggregateRootId = aggregateRoot.Id;
        command.CompositeAttr = "New Nested Composition_1";
        command.Composite = CreateCompositeSingleAADTO.Create(default, "New Nested Composition_2");
        command.Composites = new List<CreateCompositeManyAADTO>
        {
            CreateCompositeManyAADTO.Create(default, "New Nested Composition_3", default)
        };

        var handler = new CreateAggregateRootCompositeCommandHandler(new AggregateRootRepository(DbContext));
        var id = await handler.Handle(command, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var retrievedAggrRoot = await DbContext.AggregateRoots.FindAsync(aggregateRoot.Id);
        Assert.NotNull(retrievedAggrRoot);
        await Verifier.Verify(retrievedAggrRoot);
    }
    
    //[IgnoreOnCiBuildFact]
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
        
        var command = new CreateAggregateRootCompositeCommand();
        command.AggregateRootId = aggregateRoot.Id;
        command.CompositeAttr = "New Nested Composition_4";
        command.Composite = CreateCompositeSingleAADTO.Create(default, "New Nested Composition_5");
        command.Composites = new List<CreateCompositeManyAADTO>
        {
            CreateCompositeManyAADTO.Create(default, "New Nested Composition_6", default)
        };

        var handler = new CreateAggregateRootCompositeCommandHandler(new AggregateRootRepository(DbContext));
        var id = await handler.Handle(command, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var retrievedAggrRoot = await DbContext.AggregateRoots.FindAsync(aggregateRoot.Id);
        Assert.NotNull(retrievedAggrRoot);
        await Verifier.Verify(retrievedAggrRoot);
    }

    //[IgnoreOnCiBuildFact]
    public async Task Test_CreateNestedCompositionCommand_AggregateRootNotFound()
    {
        var command = new CreateAggregateRootCompositeCommand();
        command.AggregateRootId = Guid.NewGuid();
        command.CompositeAttr = "New Nested Composition_4 - Create";
        command.Composite = CreateCompositeSingleAADTO.Create(default, "New Nested Composition_5");
        command.Composites = new List<CreateCompositeManyAADTO>
        {
            CreateCompositeManyAADTO.Create(default, "New Nested Composition_6", default)
        };

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var handler = new CreateAggregateRootCompositeCommandHandler(new AggregateRootRepository(DbContext));
            var id = await handler.Handle(command, CancellationToken.None);
            await DbContext.SaveChangesAsync();    
        });
    }

    //[IgnoreOnCiBuildFact]
    public async Task Test_UpdateNestedCompositionCommand_NoComposition()
    {
        var aggregateRoot = new AggregateRoot();
        aggregateRoot.AggregateAttr = "Existing Aggregate Root - Update";
        DbContext.AggregateRoots.Add(aggregateRoot);
        await DbContext.SaveChangesAsync();
        
        var command = new UpdateAggregateRootCompositeCommand();
        command.AggregateRootId = aggregateRoot.Id;
        command.CompositeAttr = "New Nested Composition_1";
        command.Composite = UpdateCompositeSingleAADTO.Create(default, "New Nested Composition_2");
        command.Composites = new List<UpdateCompositeManyAADTO>
        {
            UpdateCompositeManyAADTO.Create(default, "New Nested Composition_3", default)
        };

        var handler = new UpdateAggregateRootCompositeCommandHandler(new AggregateRootRepository(DbContext));
        await handler.Handle(command, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var retrievedAggrRoot = await DbContext.AggregateRoots.FindAsync(aggregateRoot.Id);
        Assert.NotNull(retrievedAggrRoot);
        await Verifier.Verify(retrievedAggrRoot);
    }
    
    //[IgnoreOnCiBuildFact]
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
        
        var command = new UpdateAggregateRootCompositeCommand();
        command.AggregateRootId = aggregateRoot.Id;
        command.CompositeAttr = "New Nested Composition_1";
        command.Composite = UpdateCompositeSingleAADTO.Create(default, "New Nested Composition_2");
        command.Composites = new List<UpdateCompositeManyAADTO>
        {
            UpdateCompositeManyAADTO.Create(default, "New Nested Composition_3", default)
        };

        var handler = new UpdateAggregateRootCompositeCommandHandler(new AggregateRootRepository(DbContext));
        await handler.Handle(command, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var retrievedAggrRoot = await DbContext.AggregateRoots.FindAsync(aggregateRoot.Id);
        Assert.NotNull(retrievedAggrRoot);
        await Verifier.Verify(retrievedAggrRoot);
    }
    
    //[IgnoreOnCiBuildFact]
    public async Task Test_UpdateNestedCompositionCommand_AggregateRootNotFound()
    {
        var command = new UpdateAggregateRootCompositeCommand();
        command.AggregateRootId = Guid.NewGuid();
        command.CompositeAttr = "New Nested Composition_4 - Update";
        command.Composite = UpdateCompositeSingleAADTO.Create(default, "New Nested Composition_5");
        command.Composites = new List<UpdateCompositeManyAADTO>
        {
            UpdateCompositeManyAADTO.Create(default, "New Nested Composition_6", default)
        };

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var handler = new UpdateAggregateRootCompositeCommandHandler(new AggregateRootRepository(DbContext));
            await handler.Handle(command, CancellationToken.None);
            await DbContext.SaveChangesAsync();    
        });
    }

    //[IgnoreOnCiBuildFact]
    public async Task Test_DeleteNestedComposititionCommand_ExistingComposition()
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
        
        var command = new DeleteAggregateRootCompositeCommand();
        command.AggregateRootId = aggregateRoot.Id;
        command.Id = aggregateRoot.Composite.Id;
        
        var handler = new DeleteAggregateRootCompositeCommandHandler(new AggregateRootRepository(DbContext));
        await handler.Handle(command, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var retrievedAggrRoot = DbContext.AggregateRoots.FirstOrDefault(p => p.Id == aggregateRoot.Id);
        Assert.NotNull(retrievedAggrRoot);
        await Verifier.Verify(retrievedAggrRoot);
    }
}