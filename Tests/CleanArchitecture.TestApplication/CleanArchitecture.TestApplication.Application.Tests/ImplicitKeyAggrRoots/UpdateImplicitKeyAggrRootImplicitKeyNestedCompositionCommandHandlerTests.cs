using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.UpdateImplicitKeyAggrRootImplicitKeyNestedComposition;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedUpdateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ImplicitKeyAggrRoots
{
    public class UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandlerTests
    {
        [Theory]
        [MemberData(nameof(GetValidTestData))]
        public async Task Handle_WithValidCommand_UpdatesExistingEntity(UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand testCommand, ImplicitKeyAggrRoot owner)
        {
            // Arrange
            var expectedNestedEntity = CreateExpectedImplicitKeyNestedComposition(testCommand);

            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testCommand.ImplicitKeyAggrRootId, CancellationToken.None).Returns(Task.FromResult(owner));

            var sut = new UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(repository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            owner.ImplicitKeyNestedCompositions.Should().Contain(p => p.Id == testCommand.Id).Which.Should().BeEquivalentTo(expectedNestedEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidImplicitKeyAggrRootIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            var testCommand = fixture.Create<UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>();

            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testCommand.ImplicitKeyAggrRootId, CancellationToken.None).Returns(Task.FromResult<ImplicitKeyAggrRoot>(null));

            var sut = new UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(repository);

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
            var testCommand = fixture.Create<UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>();
            var owner = fixture.Create<ImplicitKeyAggrRoot>();
            testCommand.ImplicitKeyAggrRootId = owner.Id;

            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testCommand.ImplicitKeyAggrRootId, CancellationToken.None).Returns(Task.FromResult(owner));

            var sut = new UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(repository);

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
            var testCommand = fixture.Create<UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>();
            var owner = fixture.Create<ImplicitKeyAggrRoot>();
            testCommand.ImplicitKeyAggrRootId = owner.Id;
            owner.ImplicitKeyNestedCompositions.Add(CreateExpectedImplicitKeyNestedComposition(testCommand));
            yield return new object[] { testCommand, owner };
        }

        private static ImplicitKeyNestedComposition CreateExpectedImplicitKeyNestedComposition(UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand dto)
        {
            return new ImplicitKeyNestedComposition
            {
                ImplicitKeyAggrRootId = dto.ImplicitKeyAggrRootId,
                Id = dto.Id,
                Attribute = dto.Attribute,
            };
        }

    }
}