using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.AggregateTestNoIdReturns.GetAggregateTestNoIdReturns;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetAllQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateTestNoIdReturns
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
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<AggregateTestNoIdReturn>(comp => comp.Without(x => x.DomainEvents));
            yield return new object[] { fixture.CreateMany<AggregateTestNoIdReturn>().ToList() };
            yield return new object[] { fixture.CreateMany<AggregateTestNoIdReturn>(0).ToList() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesAggregateTestNoIdReturns(List<AggregateTestNoIdReturn> testEntities)
        {
            // Arrange
            var testQuery = new GetAggregateTestNoIdReturnsQuery();
            var repository = Substitute.For<IAggregateTestNoIdReturnRepository>();
            repository.FindAllAsync(CancellationToken.None).Returns(Task.FromResult(testEntities));

            var sut = new GetAggregateTestNoIdReturnsQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            AggregateTestNoIdReturnAssertions.AssertEquivalent(result, testEntities);
        }
    }
}