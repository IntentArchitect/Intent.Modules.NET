using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.AggregateRoots.GetAggregateRootCompositeManyBById;
using CleanArchitecture.TestApplication.Application.Common.Exceptions;
using CleanArchitecture.TestApplication.Application.Tests.CRUD.AggregateRoots;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using CleanArchitecture.TestApplication.Domain.Repositories;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
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

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<AggregateRoot>(comp => comp.Without(x => x.DomainEvents));
            var existingOwnerEntity = fixture.Create<AggregateRoot>();
            var expectedEntity = existingOwnerEntity.Composites.First();
            fixture.Customize<GetAggregateRootCompositeManyBByIdQuery>(comp => comp
            .With(x => x.Id, expectedEntity.Id)
            .With(x => x.AggregateRootId, existingOwnerEntity.Id));
            var testCommand = fixture.Create<GetAggregateRootCompositeManyBByIdQuery>();
            yield return new object[] { testCommand, existingOwnerEntity, expectedEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesCompositeManyB(
            GetAggregateRootCompositeManyBByIdQuery testQuery,
            AggregateRoot existingOwnerEntity,
            CompositeManyB existingEntity)
        {
            // Arrange
            var repository = Substitute.For<IAggregateRootRepository>();
            repository.FindByIdAsync(testQuery.AggregateRootId, CancellationToken.None).Returns(Task.FromResult(existingOwnerEntity));

            var sut = new GetAggregateRootCompositeManyBByIdQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            AggregateRootAssertions.AssertEquivalent(result, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ThrowsNotFoundException()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<AggregateRoot>(comp => comp.With(p => p.Composites, new List<CompositeManyB>()));
            var existingOwnerEntity = fixture.Create<AggregateRoot>();
            var testQuery = fixture.Create<GetAggregateRootCompositeManyBByIdQuery>();

            var repository = Substitute.For<IAggregateRootRepository>();
            repository.FindByIdAsync(testQuery.AggregateRootId, CancellationToken.None).Returns(Task.FromResult(existingOwnerEntity));

            var sut = new GetAggregateRootCompositeManyBByIdQueryHandler(repository, _mapper);

            // Act
            var act = async () => await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}