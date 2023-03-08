using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs.GetAggregateRootLongs;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetAllQueryHandlerTests", Version = "1.0")]

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

        [Theory]
        [MemberData(nameof(GetTestData))]
        public async Task Handle_WithValidQuery_RetrievesAggregateRootLongs(List<AggregateRootLong> testEntities)
        {
            // Arrange
            var expectedDtos = testEntities.Select(CreateExpectedAggregateRootLongDto).ToArray();

            var testQuery = new GetAggregateRootLongsQuery();
            var repository = Substitute.For<IAggregateRootLongRepository>();
            repository.FindAllAsync(CancellationToken.None).Returns(Task.FromResult(testEntities));

            var sut = new GetAggregateRootLongsQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedDtos);
        }

        public static IEnumerable<object[]> GetTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<AggregateRootLong>(comp => comp.Without(x => x.DomainEvents));
            yield return new object[] { fixture.CreateMany<AggregateRootLong>().ToList() };
            yield return new object[] { fixture.CreateMany<AggregateRootLong>(0).ToList() };
        }

        private static AggregateRootLongDto CreateExpectedAggregateRootLongDto(AggregateRootLong entity)
        {
            return new AggregateRootLongDto
            {
                Id = entity.Id,
                Attribute = entity.Attribute,
                CompositeOfAggrLong = entity.CompositeOfAggrLong != null ? CreateExpectedCompositeOfAggrLong(entity.CompositeOfAggrLong) : null,
            };
        }

        private static AggregateRootLongCompositeOfAggrLongDto CreateExpectedCompositeOfAggrLong(CompositeOfAggrLong entity)
        {
            return new AggregateRootLongCompositeOfAggrLongDto
            {
                Attribute = entity.Attribute,
                Id = entity.Id,
            };
        }
    }
}