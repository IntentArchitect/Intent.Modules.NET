using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeMany;
using CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeMany.CreateAggregateRootCompositeMany;
using CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeMany.DeleteAggregateRootCompositeMany;
using CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeMany.UpdateAggregateRootCompositeMany;
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
        
        var command = new CreateAggregateRootCompositeManyCommand();
        command.AggregateRootId = aggregateRoot.Id;
        command.CompositeAttr = "New Nested Composition_1";
        command.Composite = CreateCompositeSingleBBDTO.Create(default, "New Nested Composition_2");
        command.Composites = new List<CreateCompositeManyBBDTO>
        {
            CreateCompositeManyBBDTO.Create(default, "New Nested Composition_3", default)
        };

        var handler = new CreateAggregateRootCompositeManyCommandHandler(new AggregateRootRepository(DbContext));
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
        
        var command = new CreateAggregateRootCompositeManyCommand();
        command.AggregateRootId = aggregateRoot.Id;
        command.CompositeAttr = "New Nested Composition_4";
        command.Composite = CreateCompositeSingleBBDTO.Create(default, "New Nested Composition_5");
        command.Composites = new List<CreateCompositeManyBBDTO>
        {
            CreateCompositeManyBBDTO.Create(default, "New Nested Composition_6", default)
        };

        var handler = new CreateAggregateRootCompositeManyCommandHandler(new AggregateRootRepository(DbContext));
        var id = await handler.Handle(command, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var retrievedAggrRoot = await DbContext.AggregateRoots.FindAsync(aggregateRoot.Id);
        Assert.NotNull(retrievedAggrRoot);
        await Verifier.Verify(retrievedAggrRoot);
    }

    [IgnoreOnCiBuildFact]
    public async Task Test_CreateNestedCompositionCommand_AggregateRootNotFound()
    {
        var command = new CreateAggregateRootCompositeManyCommand();
        command.AggregateRootId = Guid.NewGuid();
        command.CompositeAttr = "New Nested Composition_4 - Create";
        command.Composite = CreateCompositeSingleBBDTO.Create(default, "New Nested Composition_5");
        command.Composites = new List<CreateCompositeManyBBDTO>
        {
            CreateCompositeManyBBDTO.Create(default, "New Nested Composition_6", default)
        };

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var handler = new CreateAggregateRootCompositeManyCommandHandler(new AggregateRootRepository(DbContext));
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
        
        var command = new UpdateAggregateRootCompositeManyCommand();
        command.AggregateRootId = aggregateRoot.Id;
        command.Id = default;
        command.CompositeAttr = "New Nested Composition_1";
        command.Composite = UpdateCompositeSingleBBDTO.Create(default, "New Nested Composition_2");
        command.Composites = new List<UpdateCompositeManyBBDTO>
        {
            UpdateCompositeManyBBDTO.Create(default, "New Nested Composition_3", default)
        };

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var handler = new UpdateAggregateRootCompositeManyCommandHandler(new AggregateRootRepository(DbContext));
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
        
        var command = new UpdateAggregateRootCompositeManyCommand();
        command.AggregateRootId = aggregateRoot.Id;
        command.Id = aggregateRoot.Composites.First().Id;
        command.CompositeAttr = "New Nested Composition_1";
        command.Composite = UpdateCompositeSingleBBDTO.Create(default, "New Nested Composition_2");
        command.Composites = new List<UpdateCompositeManyBBDTO>
        {
            UpdateCompositeManyBBDTO.Create(default, "New Nested Composition_3", default)
        };

        var handler = new UpdateAggregateRootCompositeManyCommandHandler(new AggregateRootRepository(DbContext));
        await handler.Handle(command, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var retrievedAggrRoot = await DbContext.AggregateRoots.FindAsync(aggregateRoot.Id);
        Assert.NotNull(retrievedAggrRoot);
        await Verifier.Verify(retrievedAggrRoot);
    }
    
    [IgnoreOnCiBuildFact]
    public async Task Test_UpdateNestedCompositionCommand_AggregateRootNotFound()
    {
        var command = new UpdateAggregateRootCompositeManyCommand();
        command.AggregateRootId = Guid.NewGuid();
        command.CompositeAttr = "New Nested Composition_4 - Update";
        command.Composite = UpdateCompositeSingleBBDTO.Create(default, "New Nested Composition_5");
        command.Composites = new List<UpdateCompositeManyBBDTO>
        {
            UpdateCompositeManyBBDTO.Create(default, "New Nested Composition_6", default)
        };

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var handler = new UpdateAggregateRootCompositeManyCommandHandler(new AggregateRootRepository(DbContext));
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
        
        var command = new DeleteAggregateRootCompositeManyCommand();
        command.AggregateRootId = aggregateRoot.Id;
        command.Id = aggregateRoot.Composites.First().Id;
        
        var handler = new DeleteAggregateRootCompositeManyCommandHandler(new AggregateRootRepository(DbContext));
        await handler.Handle(command, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var retrievedAggrRoot = DbContext.AggregateRoots.FirstOrDefault(p => p.Id == aggregateRoot.Id);
        Assert.NotNull(retrievedAggrRoot);
        await Verifier.Verify(retrievedAggrRoot);
    }
}