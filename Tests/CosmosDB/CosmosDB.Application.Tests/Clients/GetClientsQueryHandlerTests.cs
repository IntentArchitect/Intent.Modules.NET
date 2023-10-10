using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CosmosDB.Application.Clients;
using CosmosDB.Application.Clients.GetClients;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetAllQueryHandlerTests", Version = "1.0")]

namespace CosmosDB.Application.Tests.Clients
{
    public class GetClientsQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetClientsQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetClientsQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            yield return new object[] { fixture.CreateMany<Client>().ToList() };
            yield return new object[] { fixture.CreateMany<Client>(0).ToList() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesClients(List<Client> testEntities)
        {
            // Arrange
            var fixture = new Fixture();
            var testQuery = fixture.Create<GetClientsQuery>();
            var clientRepository = Substitute.For<IClientRepository>();
            clientRepository.FindAllAsync(CancellationToken.None).Returns(Task.FromResult(testEntities));

            var sut = new GetClientsQueryHandler(clientRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            ClientAssertions.AssertEquivalent(results, testEntities);
        }
    }
}