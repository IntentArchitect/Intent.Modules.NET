using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.CreateImplicitKeyAggrRoot;
using CleanArchitecture.TestApplication.Application.Tests.CRUD.ImplicitKeyAggrRoots;
using CleanArchitecture.TestApplication.Application.Tests.Extensions;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
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
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateImplicitKeyAggrRootCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsImplicitKeyAggrRootToRepository(CreateImplicitKeyAggrRootCommand testCommand)
        {
            // Arrange
            var expectedImplicitKeyAggrRootId = new Fixture().Create<System.Guid>();
            ImplicitKeyAggrRoot addedImplicitKeyAggrRoot = null;
            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.OnAdd(ent => addedImplicitKeyAggrRoot = ent);
            repository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ => addedImplicitKeyAggrRoot.Id = expectedImplicitKeyAggrRootId);
            var sut = new CreateImplicitKeyAggrRootCommandHandler(repository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedImplicitKeyAggrRootId);
            await repository.UnitOfWork.Received(1).SaveChangesAsync();
            ImplicitKeyAggrRootAssertions.AssertEquivalent(testCommand, addedImplicitKeyAggrRoot);
        }
    }
}