using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs.GetAggregateRootLongs;
using CleanArchitecture.TestApplication.Application.Common.Pagination;
using CleanArchitecture.TestApplication.Application.Tests.CRUD.AggregateRootLongs;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using CleanArchitecture.TestApplication.Domain.Repositories;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetAllPaginationQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRootLongs
{
    public class GetAggregateRootLongsQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetAggregateRootLongsQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetAggregateRootLongsQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            yield return new object[] { fixture.CreateMany<AggregateRootLong>(5).ToList() };
            yield return new object[] { fixture.CreateMany<AggregateRootLong>(0).ToList() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesAggregateRootLongs(List<AggregateRootLong> testEntities)
        {
            // Arrange
            var fixture = new Fixture();
            var testQuery = fixture.Create<GetAggregateRootLongsQuery>();
            testQuery.PageNo = 1;
            testQuery.PageSize = 5;
            var aggregateRootLongRepository = Substitute.For<IAggregateRootLongRepository>();
            var fetchedResults = Substitute.For<IPagedList<AggregateRootLong>>();
            fetchedResults.GetEnumerator().Returns(c => testEntities.GetEnumerator());
            aggregateRootLongRepository.FindAllAsync(1, 5, CancellationToken.None).Returns(Task.FromResult(fetchedResults));

            var sut = new GetAggregateRootLongsQueryHandler(aggregateRootLongRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            AggregateRootLongAssertions.AssertEquivalent(results, fetchedResults);

        }
    }
}