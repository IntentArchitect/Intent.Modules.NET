using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CosmosDB.Application.Clients;
using CosmosDB.Application.Clients.GetClientsPaged;
using CosmosDB.Application.Common.Pagination;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetAllPaginationQueryHandlerTests", Version = "1.0")]

namespace CosmosDB.Application.Tests.Clients
{
    public class GetClientsPagedQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetClientsPagedQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetClientsPagedQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            yield return new object[] { fixture.CreateMany<Client>(5).ToList() };
            yield return new object[] { fixture.CreateMany<Client>(0).ToList() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesClients(List<Client> testEntities)
        {
            // Arrange
            var fixture = new Fixture();
            var testQuery = fixture.Create<GetClientsPagedQuery>();
            testQuery.PageNo = 1;
            testQuery.PageSize = 5;
            var clientRepository = Substitute.For<IClientRepository>();
            var fetchedResults = Substitute.For<IPagedResult<Client>>();
            fetchedResults.GetEnumerator().Returns(c => testEntities.GetEnumerator());
            clientRepository.FindAllAsync(1, 5, CancellationToken.None).Returns(Task.FromResult(fetchedResults));

            var sut = new GetClientsPagedQueryHandler(clientRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            ClientAssertions.AssertEquivalent(results, fetchedResults);
        }
    }
}