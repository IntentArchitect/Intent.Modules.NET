using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CosmosDB.Application.Clients.CreateClient;
using CosmosDB.Application.Tests.Extensions;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CosmosDB.Application.Tests.Clients
{
    public class CreateClientCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateClientCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsClientToRepository(CreateClientCommand testCommand)
        {
            // Arrange
            var clientRepository = Substitute.For<IClientRepository>();
            var expectedClientIdentifier = new Fixture().Create<string>();
            Client addedClient = null;
            clientRepository.OnAdd(ent => addedClient = ent);
            clientRepository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ => addedClient.Identifier = expectedClientIdentifier);

            var sut = new CreateClientCommandHandler(clientRepository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedClientIdentifier);
            await clientRepository.UnitOfWork.Received(1).SaveChangesAsync();
            ClientAssertions.AssertEquivalent(testCommand, addedClient);
        }
    }
}