using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs.GetAggregateRootLongById;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.GetByIdQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRootLongs
{
    public class GetAggregateRootLongByIdQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetAggregateRootLongByIdQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetAggregateRootLongByIdQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public async Task Handle_WithValidQuery_RetrievesAggregateRootLong(AggregateRootLong testEntity)
        {
            // Arrange
            var expectedDto = CreateExpectedAggregateRootLongDto(testEntity);

            var query = new GetAggregateRootLongByIdQuery { Id = testEntity.Id };
            var repository = Substitute.For<IAggregateRootLongRepository>();
            repository.FindByIdAsync(query.Id, CancellationToken.None).Returns(Task.FromResult(testEntity));

            var sut = new GetAggregateRootLongByIdQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedDto);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ReturnsEmptyResult()
        {
            // Arrange
            var fixture = new Fixture();
            var query = fixture.Create<GetAggregateRootLongByIdQuery>();

            var repository = Substitute.For<IAggregateRootLongRepository>();
            repository.FindByIdAsync(query.Id, CancellationToken.None).Returns(Task.FromResult<AggregateRootLong>(default));

            var sut = new GetAggregateRootLongByIdQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(null);
        }

        public static IEnumerable<object[]> GetTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<AggregateRootLong>(comp => comp.Without(x => x.DomainEvents));
            yield return new object[] { fixture.Create<AggregateRootLong>() };
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