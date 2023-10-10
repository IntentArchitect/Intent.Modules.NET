using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CosmosDB.Application.Clients.DeleteClient;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.DeleteCommandHandlerTests", Version = "1.0")]

namespace CosmosDB.Application.Tests.Clients
{
    public class DeleteClientCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<Client>();
            fixture.Customize<DeleteClientCommand>(comp => comp.With(x => x.Identifier, existingEntity.Identifier));
            var testCommand = fixture.Create<DeleteClientCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesClientFromRepository(
            DeleteClientCommand testCommand,
            Client existingEntity)
        {
            // Arrange
            var clientRepository = Substitute.For<IClientRepository>();
            clientRepository.FindByIdAsync(testCommand.Identifier, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new DeleteClientCommandHandler(clientRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            clientRepository.Received(1).Remove(Arg.Is<Client>(p => testCommand.Identifier == p.Identifier));
        }

        [Fact]
        public async Task Handle_WithInvalidClientId_ReturnsNotFound()
        {
            // Arrange
            var clientRepository = Substitute.For<IClientRepository>();
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteClientCommand>();
            clientRepository.FindByIdAsync(testCommand.Identifier, CancellationToken.None)!.Returns(Task.FromResult<Client>(default));


            var sut = new DeleteClientCommandHandler(clientRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}