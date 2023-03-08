using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.CreateImplicitKeyAggrRootImplicitKeyNestedComposition;
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

namespace CleanArchitecture.TestApplication.Application.Tests.ImplicitKeyAggrRoots
{
    public class CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandlerTests
    {
        [Theory]
        [MemberData(nameof(GetValidTestData))]
        public async Task Handle_WithValidCommand_AddsImplicitKeyNestedCompositionToRepository(ImplicitKeyAggrRoot owner, CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand testCommand)
        {
            // Arrange
            var expectedImplicitKeyNestedComposition = CreateExpectedImplicitKeyNestedComposition(testCommand);

            ImplicitKeyNestedComposition addedImplicitKeyNestedComposition = null;
            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testCommand.ImplicitKeyAggrRootId, CancellationToken.None).Returns(Task.FromResult(owner));
            repository.OnSave(
                () =>
                {
                    addedImplicitKeyNestedComposition = owner.ImplicitKeyNestedCompositions.Single(p => p.Id == default);
                    addedImplicitKeyNestedComposition.Id = expectedImplicitKeyNestedComposition.Id;
                });
            var sut = new CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(repository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedImplicitKeyNestedComposition.Id);
            expectedImplicitKeyNestedComposition.Should().BeEquivalentTo(addedImplicitKeyNestedComposition);
        }

        public static IEnumerable<object[]> GetValidTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            var owner = fixture.Create<ImplicitKeyAggrRoot>();
            var command = fixture.Create<CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>();
            command.ImplicitKeyAggrRootId = owner.Id;
            yield return new object[] { owner, command };
        }

        private static ImplicitKeyNestedComposition CreateExpectedImplicitKeyNestedComposition(CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand dto)
        {
            return new ImplicitKeyNestedComposition
            {
#warning No matching field found for ImplicitKeyAggrRootId
                Attribute = dto.Attribute,
            };
        }
    }
}