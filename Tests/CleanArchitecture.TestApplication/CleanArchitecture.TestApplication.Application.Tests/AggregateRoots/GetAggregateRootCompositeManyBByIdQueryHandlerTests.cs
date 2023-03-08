using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.AggregateRoots;
using CleanArchitecture.TestApplication.Application.AggregateRoots.GetAggregateRootCompositeManyBById;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedGetByIdQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots
{
    public class GetAggregateRootCompositeManyBByIdQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetAggregateRootCompositeManyBByIdQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetAggregateRootCompositeManyBByIdQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        [Fact]
        public async Task Handle_WithValidQuery_RetrievesCompositeManyB()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            var owner = fixture.Create<AggregateRoot>();
            var expectedNestedEntityDto = CreateExpectedAggregateRootCompositeManyBDto(owner.Composites.First());

            var testQuery = new GetAggregateRootCompositeManyBByIdQuery();
            testQuery.Id = expectedNestedEntityDto.Id;
            testQuery.AggregateRootId = owner.Id;

            var repository = Substitute.For<IAggregateRootRepository>();
            repository.FindByIdAsync(testQuery.AggregateRootId, CancellationToken.None).Returns(Task.FromResult(owner));

            var sut = new GetAggregateRootCompositeManyBByIdQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedNestedEntityDto);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ReturnsEmptyResult()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<AggregateRoot>(comp => comp.With(p => p.Composites, new List<CompositeManyB>()));
            var owner = fixture.Create<AggregateRoot>();
            var testQuery = fixture.Create<GetAggregateRootCompositeManyBByIdQuery>();

            var repository = Substitute.For<IAggregateRootRepository>();
            repository.FindByIdAsync(testQuery.AggregateRootId, CancellationToken.None).Returns(Task.FromResult(owner));

            var sut = new GetAggregateRootCompositeManyBByIdQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            result.Should().Be(null);
        }

        private static AggregateRootCompositeManyBDto CreateExpectedAggregateRootCompositeManyBDto(CompositeManyB entity)
        {
            return new AggregateRootCompositeManyBDto
            {
                CompositeAttr = entity.CompositeAttr,
                SomeDate = entity.SomeDate,
                AggregateRootId = entity.AggregateRootId,
                Id = entity.Id,
                Composite = entity.Composite != null ? CreateExpectedCompositeSingleBB(entity.Composite) : null,
                Composites = entity.Composites?.Select(CreateExpectedCompositeManyBB).ToList() ?? new List<AggregateRootCompositeManyBCompositeManyBBDto>(),
            };
        }

        private static AggregateRootCompositeManyBCompositeSingleBBDto CreateExpectedCompositeSingleBB(CompositeSingleBB entity)
        {
            return new AggregateRootCompositeManyBCompositeSingleBBDto
            {
                CompositeAttr = entity.CompositeAttr,
                Id = entity.Id,
            };
        }

        private static AggregateRootCompositeManyBCompositeManyBBDto CreateExpectedCompositeManyBB(CompositeManyBB entity)
        {
            return new AggregateRootCompositeManyBCompositeManyBBDto
            {
                CompositeAttr = entity.CompositeAttr,
                CompositeManyBId = entity.CompositeManyBId,
                Id = entity.Id,
            };
        }
    }
}