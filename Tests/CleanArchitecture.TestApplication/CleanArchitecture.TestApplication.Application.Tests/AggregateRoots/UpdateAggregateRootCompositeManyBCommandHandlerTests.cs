using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRoots;
using CleanArchitecture.TestApplication.Application.AggregateRoots.UpdateAggregateRootCompositeManyB;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedUpdateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots
{
    public class UpdateAggregateRootCompositeManyBCommandHandlerTests
    {
        [Theory]
        [MemberData(nameof(GetValidTestData))]
        public async Task Handle_WithValidCommand_UpdatesExistingEntity(UpdateAggregateRootCompositeManyBCommand testCommand, AggregateRoot owner)
        {
            // Arrange
            var expectedNestedEntity = CreateExpectedCompositeManyB(testCommand);

            var repository = Substitute.For<IAggregateRootRepository>();
            repository.FindByIdAsync(testCommand.AggregateRootId, CancellationToken.None).Returns(Task.FromResult(owner));

            var sut = new UpdateAggregateRootCompositeManyBCommandHandler(repository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            owner.Composites.Should().Contain(p => p.Id == testCommand.Id).Which.Should().BeEquivalentTo(expectedNestedEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidAggregateRootIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            var testCommand = fixture.Create<UpdateAggregateRootCompositeManyBCommand>();

            var repository = Substitute.For<IAggregateRootRepository>();
            repository.FindByIdAsync(testCommand.AggregateRootId, CancellationToken.None).Returns(Task.FromResult<AggregateRoot>(null));

            var sut = new UpdateAggregateRootCompositeManyBCommandHandler(repository);

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await sut.Handle(testCommand, CancellationToken.None);
            });
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            var testCommand = fixture.Create<UpdateAggregateRootCompositeManyBCommand>();
            var owner = fixture.Create<AggregateRoot>();
            testCommand.AggregateRootId = owner.Id;

            var repository = Substitute.For<IAggregateRootRepository>();
            repository.FindByIdAsync(testCommand.AggregateRootId, CancellationToken.None).Returns(Task.FromResult(owner));

            var sut = new UpdateAggregateRootCompositeManyBCommandHandler(repository);

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await sut.Handle(testCommand, CancellationToken.None);
            });
        }

        public static IEnumerable<object[]> GetValidTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            var testCommand = fixture.Create<UpdateAggregateRootCompositeManyBCommand>();
            var owner = fixture.Create<AggregateRoot>();
            testCommand.AggregateRootId = owner.Id;
            owner.Composites.Add(CreateExpectedCompositeManyB(testCommand));
            yield return new object[] { testCommand, owner };

            fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<UpdateAggregateRootCompositeManyBCommand>(comp => comp.Without(x => x.Composite));
            testCommand = fixture.Create<UpdateAggregateRootCompositeManyBCommand>();
            owner = fixture.Create<AggregateRoot>();
            testCommand.AggregateRootId = owner.Id;
            owner.Composites.Add(CreateExpectedCompositeManyB(testCommand));
            yield return new object[] { testCommand, owner };
        }

        private static CompositeManyB CreateExpectedCompositeManyB(UpdateAggregateRootCompositeManyBCommand dto)
        {
            return new CompositeManyB
            {
                AggregateRootId = dto.AggregateRootId,
                Id = dto.Id,
                CompositeAttr = dto.CompositeAttr,
                SomeDate = dto.SomeDate,
                Composite = dto.Composite != null ? CreateExpectedCompositeSingleBB(dto.Composite) : null,
                Composites = dto.Composites?.Select(CreateExpectedCompositeManyBB).ToList() ?? new List<CompositeManyBB>(),
            };
        }

        private static CompositeSingleBB CreateExpectedCompositeSingleBB(UpdateAggregateRootCompositeManyBCompositeSingleBBDto dto)
        {
            return new CompositeSingleBB
            {
                CompositeAttr = dto.CompositeAttr,
                Id = dto.Id,
            };
        }

        private static CompositeManyBB CreateExpectedCompositeManyBB(UpdateAggregateRootCompositeManyBCompositeManyBBDto dto)
        {
            return new CompositeManyBB
            {
                CompositeAttr = dto.CompositeAttr,
                CompositeManyBId = dto.CompositeManyBId,
                Id = dto.Id,
            };
        }
    }
}