using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.AggregateTestNoIdReturns;
using CleanArchitecture.Comprehensive.Application.AggregateTestNoIdReturns.GetAggregateTestNoIdReturns;
using CleanArchitecture.Comprehensive.Application.Tests.CRUD.AggregateTestNoIdReturns;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetAllQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.AggregateTestNoIdReturns
{
    public class GetAggregateTestNoIdReturnsQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetAggregateTestNoIdReturnsQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetAggregateTestNoIdReturnsQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            yield return new object[] { fixture.CreateMany<AggregateTestNoIdReturn>().ToList() };
            yield return new object[] { fixture.CreateMany<AggregateTestNoIdReturn>(0).ToList() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesAggregateTestNoIdReturns(List<AggregateTestNoIdReturn> testEntities)
        {
            // Arrange
            var fixture = new Fixture();
            var testQuery = fixture.Create<GetAggregateTestNoIdReturnsQuery>();
            var aggregateTestNoIdReturnRepository = Substitute.For<IAggregateTestNoIdReturnRepository>();
            aggregateTestNoIdReturnRepository.FindAllAsync(CancellationToken.None).Returns(Task.FromResult(testEntities));

            var sut = new GetAggregateTestNoIdReturnsQueryHandler(aggregateTestNoIdReturnRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            AggregateTestNoIdReturnAssertions.AssertEquivalent(results, testEntities);
        }
    }
}