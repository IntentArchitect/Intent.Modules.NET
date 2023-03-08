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
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ImplicitKeyAggrRoots
{
    public class CreateImplicitKeyAggrRootCommandHandlerTests
    {
        [Theory]
        [MemberData(nameof(GetTestData))]
        public async Task Handle_WithValidCommand_AddsImplicitKeyAggrRootToRepository(CreateImplicitKeyAggrRootCommand testCommand)
        {
            // Arrange
            var expectedImplicitKeyAggrRoot = CreateExpectedImplicitKeyAggrRoot(testCommand);

            ImplicitKeyAggrRoot addedImplicitKeyAggrRoot = null;
            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.OnAdd(ent => addedImplicitKeyAggrRoot = ent);
            repository.OnSave(() => addedImplicitKeyAggrRoot.Id = expectedImplicitKeyAggrRoot.Id);

            var sut = new CreateImplicitKeyAggrRootCommandHandler(repository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedImplicitKeyAggrRoot.Id);
            expectedImplicitKeyAggrRoot.Should().BeEquivalentTo(addedImplicitKeyAggrRoot);
        }

        public static IEnumerable<object[]> GetTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateImplicitKeyAggrRootCommand>() };

            fixture = new Fixture();
            fixture.Customize<CreateImplicitKeyAggrRootCommand>(comp => comp.Without(x => x.ImplicitKeyNestedCompositions));
            yield return new object[] { fixture.Create<CreateImplicitKeyAggrRootCommand>() };
        }

        private static ImplicitKeyAggrRoot CreateExpectedImplicitKeyAggrRoot(CreateImplicitKeyAggrRootCommand dto)
        {
            return new ImplicitKeyAggrRoot
            {
                Attribute = dto.Attribute,
                ImplicitKeyNestedCompositions = dto.ImplicitKeyNestedCompositions.Select(CreateExpectedImplicitKeyNestedComposition).ToList(),
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