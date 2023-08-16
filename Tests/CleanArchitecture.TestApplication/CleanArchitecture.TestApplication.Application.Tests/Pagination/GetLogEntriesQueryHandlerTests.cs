using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.AggregateRoots.GetAggregateRoots;
using CleanArchitecture.TestApplication.Application.Common.Pagination;
using CleanArchitecture.TestApplication.Application.Pagination.GetLogEntries;
using CleanArchitecture.TestApplication.Application.Tests.CRUD.AggregateRoots;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities.Pagination;
using CleanArchitecture.TestApplication.Domain.Repositories;
using CleanArchitecture.TestApplication.Domain.Repositories.Pagination;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetAllQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.Pagination
{
    public class GetLogEntriesQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetLogEntriesQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetLogEntriesHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            yield return new object[] { fixture.CreateMany<LogEntry>(5).ToList() };
            yield return new object[] { fixture.CreateMany<LogEntry>(0).ToList() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesLogEntries(List<LogEntry> testEntities)
        {
            // Arrange
            var fixture = new Fixture();
            var testQuery = fixture.Create<GetLogEntries>();
            testQuery.PageNo = 1;
            testQuery.PageSize = 5;
            var logEntryRepository = Substitute.For<ILogEntryRepository>();
            var fetchedResults = Substitute.For<IPagedResult<LogEntry>>();
            fetchedResults.GetEnumerator().Returns(c => testEntities.GetEnumerator());
            logEntryRepository.FindAllAsync(1, 5, CancellationToken.None).Returns(Task.FromResult(fetchedResults));

            var sut = new GetLogEntriesHandler(logEntryRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            AggregateRootAssertions.AssertEquivalent(results, fetchedResults);
        }
    }
}