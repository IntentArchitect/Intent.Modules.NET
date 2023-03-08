using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.UpdateImplicitKeyAggrRoot;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.UpdateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ImplicitKeyAggrRoots
{
    public class UpdateImplicitKeyAggrRootCommandHandlerTests
    {
        [Theory]
        [MemberData(nameof(GetTestData))]
        public async Task Handle_WithValidCommand_UpdatesExistingEntity(UpdateImplicitKeyAggrRootCommand testCommand, ImplicitKeyAggrRoot existingEntity)
        {
            // Arrange
            var expectedImplicitKeyAggrRoot = CreateExpectedImplicitKeyAggrRoot(testCommand);

            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testCommand.Id, CancellationToken.None).Returns(Task.FromResult(existingEntity));

            var sut = new UpdateImplicitKeyAggrRootCommandHandler(repository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            expectedImplicitKeyAggrRoot.Should().BeEquivalentTo(existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateImplicitKeyAggrRootCommand>();

            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testCommand.Id, CancellationToken.None).Returns(Task.FromResult<ImplicitKeyAggrRoot>(null));

            var sut = new UpdateImplicitKeyAggrRootCommandHandler(repository);

            // Act
            // Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () =>
            {
                await sut.Handle(testCommand, CancellationToken.None);
            });
        }

        public static IEnumerable<object[]> GetTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateImplicitKeyAggrRootCommand>();
            yield return new object[] { testCommand, CreateExpectedImplicitKeyAggrRoot(testCommand) };

            fixture = new Fixture();
            fixture.Customize<UpdateImplicitKeyAggrRootCommand>(comp => comp.Without(x => x.ImplicitKeyNestedCompositions));
            testCommand = fixture.Create<UpdateImplicitKeyAggrRootCommand>();
            yield return new object[] { testCommand, CreateExpectedImplicitKeyAggrRoot(testCommand) };
        }

        private static ImplicitKeyAggrRoot CreateExpectedImplicitKeyAggrRoot(UpdateImplicitKeyAggrRootCommand dto)
        {
            return new ImplicitKeyAggrRoot
            {
#warning No matching field found for Id
                Attribute = dto.Attribute,
                ImplicitKeyNestedCompositions = dto.ImplicitKeyNestedCompositions.Select(CreateExpectedImplicitKeyNestedComposition).ToList(),
            };
        }

        private static ImplicitKeyNestedComposition CreateExpectedImplicitKeyNestedComposition(UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionDto dto)
        {
            return new ImplicitKeyNestedComposition
            {
                Attribute = dto.Attribute,
#warning No matching field found for Id
            };
        }
    }
}