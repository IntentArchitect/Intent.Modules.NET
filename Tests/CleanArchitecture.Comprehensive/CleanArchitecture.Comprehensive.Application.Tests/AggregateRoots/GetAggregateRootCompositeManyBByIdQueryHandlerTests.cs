using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.AggregateRoots;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.GetAggregateRootCompositeManyBById;
using CleanArchitecture.Comprehensive.Application.Tests.CRUD.AggregateRoots;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedGetByIdQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.AggregateRoots
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

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingOwnerEntity = fixture.Create<AggregateRoot>();
            var expectedEntity = existingOwnerEntity.Composites.First();
            fixture.Customize<GetAggregateRootCompositeManyBByIdQuery>(comp => comp
                .With(x => x.AggregateRootId, existingOwnerEntity.Id)
                .With(x => x.Id, expectedEntity.Id));
            var testQuery = fixture.Create<GetAggregateRootCompositeManyBByIdQuery>();
            yield return new object[] { testQuery, existingOwnerEntity, expectedEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesCompositeManyB(
            GetAggregateRootCompositeManyBByIdQuery testQuery,
            AggregateRoot existingOwnerEntity,
            CompositeManyB existingEntity)
        {
            // Arrange
            var aggregateRootRepository = Substitute.For<IAggregateRootRepository>();
            aggregateRootRepository.FindByIdAsync(testQuery.AggregateRootId, CancellationToken.None)!.Returns(Task.FromResult(existingOwnerEntity));

            var sut = new GetAggregateRootCompositeManyBByIdQueryHandler(aggregateRootRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            AggregateRootAssertions.AssertEquivalent(results, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ThrowsNotFoundException()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            fixture.Customize<AggregateRoot>(comp => comp.With(p => p.Composites, new List<CompositeManyB>()));
            var existingOwnerEntity = fixture.Create<AggregateRoot>();
            fixture.Customize<GetAggregateRootCompositeManyBByIdQuery>(comp => comp
                .With(p => p.AggregateRootId, existingOwnerEntity.Id));
            var testQuery = fixture.Create<GetAggregateRootCompositeManyBByIdQuery>();
            var aggregateRootRepository = Substitute.For<IAggregateRootRepository>();
            aggregateRootRepository.FindByIdAsync(testQuery.AggregateRootId, CancellationToken.None)!.Returns(Task.FromResult(existingOwnerEntity));


            var sut = new GetAggregateRootCompositeManyBByIdQueryHandler(aggregateRootRepository, _mapper);

            // Act
            var act = async () => await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}