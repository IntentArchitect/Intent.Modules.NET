using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Tests.UniqueIndexConstraint.AggregateWithUniqueConstraintIndexElements;
using CleanArchitecture.TestApplication.Application.UniqueIndexConstraint;
using CleanArchitecture.TestApplication.Application.UniqueIndexConstraint.GetAggregateWithUniqueConstraintIndexElements;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities.UniqueIndexConstraint;
using CleanArchitecture.TestApplication.Domain.Repositories.UniqueIndexConstraint;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetAllQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.UniqueIndexConstraint
{
    public class GetAggregateWithUniqueConstraintIndexElementsQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetAggregateWithUniqueConstraintIndexElementsQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetAggregateWithUniqueConstraintIndexElementsQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            yield return new object[] { fixture.CreateMany<AggregateWithUniqueConstraintIndexElement>().ToList() };
            yield return new object[] { fixture.CreateMany<AggregateWithUniqueConstraintIndexElement>(0).ToList() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesAggregateWithUniqueConstraintIndexElements(List<AggregateWithUniqueConstraintIndexElement> testEntities)
        {
            // Arrange
            var fixture = new Fixture();
            var testQuery = fixture.Create<GetAggregateWithUniqueConstraintIndexElementsQuery>();
            var aggregateWithUniqueConstraintIndexElementRepository = Substitute.For<IAggregateWithUniqueConstraintIndexElementRepository>();
            aggregateWithUniqueConstraintIndexElementRepository.FindAllAsync(CancellationToken.None).Returns(Task.FromResult(testEntities));

            var sut = new GetAggregateWithUniqueConstraintIndexElementsQueryHandler(aggregateWithUniqueConstraintIndexElementRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            AggregateWithUniqueConstraintIndexElementAssertions.AssertEquivalent(results, testEntities);
        }
    }
}