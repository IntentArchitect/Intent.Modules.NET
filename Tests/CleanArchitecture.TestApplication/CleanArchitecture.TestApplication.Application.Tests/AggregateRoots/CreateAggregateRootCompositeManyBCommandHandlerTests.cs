using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRoots;
using CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRootCompositeManyB;
using CleanArchitecture.TestApplication.Application.Tests.Extensions;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedCreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots
{
    public class CreateAggregateRootCompositeManyBCommandHandlerTests
    {
        [Theory]
        [MemberData(nameof(GetValidTestData))]
        public async Task Handle_WithValidCommand_AddsCompositeManyBToRepository(AggregateRoot owner, CreateAggregateRootCompositeManyBCommand testCommand)
        {
            // Arrange
            var expectedCompositeManyB = CreateExpectedCompositeManyB(testCommand);
            expectedCompositeManyB.AutoAssignId(k => k.Id);

            CompositeManyB addedCompositeManyB = null;
            var repository = Substitute.For<IAggregateRootRepository>();
            repository.FindByIdAsync(testCommand.AggregateRootId, CancellationToken.None).Returns(Task.FromResult(owner));
            repository.OnSave(
                () =>
                {
                    addedCompositeManyB = owner.Composites.Single(p => p.Id == default);
                    addedCompositeManyB.Id = expectedCompositeManyB.Id;
                    addedCompositeManyB.AggregateRootId = expectedCompositeManyB.AggregateRootId;
                });
            var sut = new CreateAggregateRootCompositeManyBCommandHandler(repository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedCompositeManyB.Id);
            addedCompositeManyB.Should().BeEquivalentTo(expectedCompositeManyB);
        }

        public static IEnumerable<object[]> GetValidTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            var owner = fixture.Create<AggregateRoot>();
            var command = fixture.Create<CreateAggregateRootCompositeManyBCommand>();
            command.AggregateRootId = owner.Id;
            yield return new object[] { owner, command };

            fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<CreateAggregateRootCompositeManyBCommand>(comp => comp.Without(x => x.Composite));
            owner = fixture.Create<AggregateRoot>();
            command = fixture.Create<CreateAggregateRootCompositeManyBCommand>();
            command.AggregateRootId = owner.Id;
            yield return new object[] { owner, command };
        }

        private static CompositeManyB CreateExpectedCompositeManyB(CreateAggregateRootCompositeManyBCommand dto)
        {
            return new CompositeManyB
            {
                AggregateRootId = dto.AggregateRootId,
                CompositeAttr = dto.CompositeAttr,
                SomeDate = dto.SomeDate,
                Composite = dto.Composite != null ? CreateExpectedCompositeSingleBB(dto.Composite) : null,
                Composites = dto.Composites?.Select(CreateExpectedCompositeManyBB).ToList() ?? new List<CompositeManyBB>(),
            };
        }

        private static CompositeSingleBB CreateExpectedCompositeSingleBB(CreateAggregateRootCompositeManyBCompositeSingleBBDto dto)
        {
            return new CompositeSingleBB
            {
                CompositeAttr = dto.CompositeAttr,
            };
        }

        private static CompositeManyBB CreateExpectedCompositeManyBB(CreateAggregateRootCompositeManyBCompositeManyBBDto dto)
        {
            return new CompositeManyBB
            {
                CompositeAttr = dto.CompositeAttr,
            };
        }
    }
}