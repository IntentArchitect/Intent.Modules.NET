using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.EntityWithCtors.CreateEntityWithCtor;
using CleanArchitecture.TestApplication.Application.Tests.Extensions;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.EntityWithCtors
{
    public class CreateEntityWithCtorCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateEntityWithCtorCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsEntityWithCtorToRepository(CreateEntityWithCtorCommand testCommand)
        {
            // Arrange
            var expectedEntityWithCtorId = new Fixture().Create<System.Guid>();
            EntityWithCtor addedEntityWithCtor = null;
            var repository = Substitute.For<IEntityWithCtorRepository>();
            repository.OnAdd(ent => addedEntityWithCtor = ent);
            repository.OnSaveChanges(() => addedEntityWithCtor.Id = expectedEntityWithCtorId);
            var sut = new CreateEntityWithCtorCommandHandler(repository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedEntityWithCtorId);
            await repository.UnitOfWork.Received(1).SaveChangesAsync();
            EntityWithCtorAssertions.AssertEquivalent(testCommand, addedEntityWithCtor);
        }
    }
}