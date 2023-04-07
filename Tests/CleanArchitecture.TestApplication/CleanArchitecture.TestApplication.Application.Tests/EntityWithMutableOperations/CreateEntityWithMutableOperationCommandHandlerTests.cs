using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.EntityWithMutableOperations.CreateEntityWithMutableOperation;
using CleanArchitecture.TestApplication.Application.Tests.Extensions;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.EntityWithMutableOperations
{
    public class CreateEntityWithMutableOperationCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateEntityWithMutableOperationCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsEntityWithMutableOperationToRepository(CreateEntityWithMutableOperationCommand testCommand)
        {
            // Arrange
            var expectedEntityWithMutableOperationId = new Fixture().Create<System.Guid>();
            EntityWithMutableOperation addedEntityWithMutableOperation = null;
            var repository = Substitute.For<IEntityWithMutableOperationRepository>();
            repository.OnAdd(ent => addedEntityWithMutableOperation = ent);
            repository.OnSaveChanges(() => addedEntityWithMutableOperation.Id = expectedEntityWithMutableOperationId);
            var sut = new CreateEntityWithMutableOperationCommandHandler(repository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedEntityWithMutableOperationId);
            await repository.UnitOfWork.Received(1).SaveChangesAsync();
            EntityWithMutableOperationAssertions.AssertEquivalent(testCommand, addedEntityWithMutableOperation);
        }
    }
}