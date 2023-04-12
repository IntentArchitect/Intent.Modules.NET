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
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetByIdQueryHandlerTests", Version = "1.0")]

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

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<AggregateRootLong>(comp => comp.Without(x => x.DomainEvents));
            var existingEntity = fixture.Create<AggregateRootLong>();
            fixture.Customize<GetAggregateRootLongByIdQuery>(comp => comp.With(p => p.Id, existingEntity.Id));
            var testQuery = fixture.Create<GetAggregateRootLongByIdQuery>();
            yield return new object[] { testQuery, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesAggregateRootLong(
            GetAggregateRootLongByIdQuery testQuery,
            AggregateRootLong existingEntity)
        {
            // Arrange
            var repository = Substitute.For<IAggregateRootLongRepository>();
            repository.FindByIdAsync(testQuery.Id, CancellationToken.None).Returns(Task.FromResult(existingEntity));

            var sut = new GetAggregateRootLongByIdQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            AggregateRootLongAssertions.AssertEquivalent(result, existingEntity);
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
    }
}