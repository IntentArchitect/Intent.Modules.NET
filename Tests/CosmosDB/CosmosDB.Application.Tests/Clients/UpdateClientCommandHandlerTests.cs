using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CosmosDB.Application.Clients.UpdateClient;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.UpdateCommandHandlerTests", Version = "1.0")]

namespace CosmosDB.Application.Tests.Clients
{
    public class UpdateClientCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<Client>();
            fixture.Customize<UpdateClientCommand>(comp => comp.With(x => x.Identifier, existingEntity.Identifier));
            var testCommand = fixture.Create<UpdateClientCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_UpdatesExistingEntity(
            UpdateClientCommand testCommand,
            Client existingEntity)
        {
            // Arrange
            var clientRepository = Substitute.For<IClientRepository>();
            clientRepository.FindByIdAsync(testCommand.Identifier, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new UpdateClientCommandHandler(clientRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            ClientAssertions.AssertEquivalent(testCommand, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateClientCommand>();
            var clientRepository = Substitute.For<IClientRepository>();
            clientRepository.FindByIdAsync(testCommand.Identifier, CancellationToken.None)!.Returns(Task.FromResult<Client>(default));


            var sut = new UpdateClientCommandHandler(clientRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}