using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CosmosDB.Application.DerivedOfTS;
using CosmosDB.Application.DerivedOfTS.GetDerivedOfTS;
using CosmosDB.Application.Tests.DerivedOfTs;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetAllQueryHandlerTests", Version = "1.0")]

namespace CosmosDB.Application.Tests.DerivedOfTS
{
    public class GetDerivedOfTSQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetDerivedOfTSQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetDerivedOfTSQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            yield return new object[] { fixture.CreateMany<DerivedOfT>().ToList() };
            yield return new object[] { fixture.CreateMany<DerivedOfT>(0).ToList() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesDerivedOfTs(List<DerivedOfT> testEntities)
        {
            // Arrange
            var fixture = new Fixture();
            var testQuery = fixture.Create<GetDerivedOfTSQuery>();
            var derivedOfTRepository = Substitute.For<IDerivedOfTRepository>();
            derivedOfTRepository.FindAllAsync(CancellationToken.None).Returns(Task.FromResult(testEntities));

            var sut = new GetDerivedOfTSQueryHandler(derivedOfTRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            DerivedOfTAssertions.AssertEquivalent(results, testEntities);
        }
    }
}