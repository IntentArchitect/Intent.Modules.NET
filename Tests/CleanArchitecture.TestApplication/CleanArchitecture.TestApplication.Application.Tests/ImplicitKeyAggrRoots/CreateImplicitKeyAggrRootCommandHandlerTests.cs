using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.CreateImplicitKeyAggrRoot;
using CleanArchitecture.TestApplication.Application.Tests.Extensions;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ImplicitKeyAggrRoots
{
    public class CreateImplicitKeyAggrRootCommandHandlerTests
    {
        [Theory]
        [MemberData(nameof(GetValidTestData))]
        public async Task Handle_WithValidCommand_AddsImplicitKeyAggrRootToRepository(CreateImplicitKeyAggrRootCommand testCommand)
        {
            // Arrange
            var expectedImplicitKeyAggrRoot = CreateExpectedImplicitKeyAggrRoot(testCommand);
            expectedImplicitKeyAggrRoot.AutoAssignId(k => k.Id);

            ImplicitKeyAggrRoot addedImplicitKeyAggrRoot = null;
            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.OnAdd(ent => addedImplicitKeyAggrRoot = ent);
            repository.OnSaveChanges(() => addedImplicitKeyAggrRoot.Id = expectedImplicitKeyAggrRoot.Id);

            var sut = new CreateImplicitKeyAggrRootCommandHandler(repository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedImplicitKeyAggrRoot.Id);
            addedImplicitKeyAggrRoot.Should().BeEquivalentTo(expectedImplicitKeyAggrRoot);
        }

        [Theory]
        [MemberData(nameof(GetInvalidTestData))]
        public async Task Handle_WithInvalidCommand_ThrowsException(CreateImplicitKeyAggrRootCommand testCommand)
        {
            // Arrange
            var expectedImplicitKeyAggrRoot = CreateExpectedImplicitKeyAggrRoot(testCommand);

            ImplicitKeyAggrRoot addedImplicitKeyAggrRoot = null;
            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.OnAdd(ent => addedImplicitKeyAggrRoot = ent);
            repository.OnSaveChanges(() => addedImplicitKeyAggrRoot.Id = expectedImplicitKeyAggrRoot.Id);

            var sut = new CreateImplicitKeyAggrRootCommandHandler(repository);
            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () =>
                {
                    await sut.Handle(testCommand, CancellationToken.None);
                });
        }

        public static IEnumerable<object[]> GetValidTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateImplicitKeyAggrRootCommand>() };
        }

        public static IEnumerable<object[]> GetInvalidTestData()
        {
            Fixture fixture;

            fixture = new Fixture();
            fixture.Customize<CreateImplicitKeyAggrRootCommand>(comp => comp.Without(x => x.ImplicitKeyNestedCompositions));
            yield return new object[] { fixture.Create<CreateImplicitKeyAggrRootCommand>() };
        }

        private static ImplicitKeyAggrRoot CreateExpectedImplicitKeyAggrRoot(CreateImplicitKeyAggrRootCommand dto)
        {
            return new ImplicitKeyAggrRoot
            {
                Attribute = dto.Attribute,
                ImplicitKeyNestedCompositions = dto.ImplicitKeyNestedCompositions?.Select(CreateExpectedImplicitKeyNestedComposition).ToList() ?? new List<ImplicitKeyNestedComposition>(),
            };
        }

        private static ImplicitKeyNestedComposition CreateExpectedImplicitKeyNestedComposition(CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDto dto)
        {
            return new ImplicitKeyNestedComposition
            {
                Attribute = dto.Attribute,
            };
        }
    }
}