using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CosmosDB.Application.Clients;
using CosmosDB.Application.Clients.GetClientById;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetByIdQueryHandlerTests", Version = "1.0")]

namespace CosmosDB.Application.Tests.Clients
{
    public class GetClientByIdQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetClientByIdQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetClientByIdQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);

            var existingEntity = fixture.Create<Client>();
            fixture.Customize<GetClientByIdQuery>(comp => comp.With(x => x.Identifier, existingEntity.Identifier));
            var testQuery = fixture.Create<GetClientByIdQuery>();
            yield return new object[] { testQuery, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesClient(GetClientByIdQuery testQuery, Client existingEntity)
        {
            // Arrange
            var clientRepository = Substitute.For<IClientRepository>();
            clientRepository.FindByIdAsync(testQuery.Identifier, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));


            var sut = new GetClientByIdQueryHandler(clientRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            ClientAssertions.AssertEquivalent(results, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ThrowsNotFoundException()
        {
            // Arrange
            var fixture = new Fixture();
            var query = fixture.Create<GetClientByIdQuery>();
            var clientRepository = Substitute.For<IClientRepository>();
            clientRepository.FindByIdAsync(query.Identifier, CancellationToken.None)!.Returns(Task.FromResult<Client>(default));

            var sut = new GetClientByIdQueryHandler(clientRepository, _mapper);

            // Act
            var act = async () => await sut.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}