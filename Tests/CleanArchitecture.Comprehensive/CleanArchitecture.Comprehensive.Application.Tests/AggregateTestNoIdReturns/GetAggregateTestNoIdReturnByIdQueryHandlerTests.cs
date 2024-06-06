using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.AggregateTestNoIdReturns;
using CleanArchitecture.Comprehensive.Application.AggregateTestNoIdReturns.GetAggregateTestNoIdReturnById;
using CleanArchitecture.Comprehensive.Application.Tests.CRUD.AggregateTestNoIdReturns;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetByIdQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.AggregateTestNoIdReturns
{
    public class GetAggregateTestNoIdReturnByIdQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetAggregateTestNoIdReturnByIdQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetAggregateTestNoIdReturnByIdQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);

            var existingEntity = fixture.Create<AggregateTestNoIdReturn>();
            fixture.Customize<GetAggregateTestNoIdReturnByIdQuery>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testQuery = fixture.Create<GetAggregateTestNoIdReturnByIdQuery>();
            yield return new object[] { testQuery, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesAggregateTestNoIdReturn(
            GetAggregateTestNoIdReturnByIdQuery testQuery,
            AggregateTestNoIdReturn existingEntity)
        {
            // Arrange
            var aggregateTestNoIdReturnRepository = Substitute.For<IAggregateTestNoIdReturnRepository>();
            aggregateTestNoIdReturnRepository.FindByIdAsync(testQuery.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));


            var sut = new GetAggregateTestNoIdReturnByIdQueryHandler(aggregateTestNoIdReturnRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            AggregateTestNoIdReturnAssertions.AssertEquivalent(results, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ThrowsNotFoundException()
        {
            // Arrange
            var fixture = new Fixture();
            var query = fixture.Create<GetAggregateTestNoIdReturnByIdQuery>();
            var aggregateTestNoIdReturnRepository = Substitute.For<IAggregateTestNoIdReturnRepository>();
            aggregateTestNoIdReturnRepository.FindByIdAsync(query.Id, CancellationToken.None)!.Returns(Task.FromResult<AggregateTestNoIdReturn>(default));

            var sut = new GetAggregateTestNoIdReturnByIdQueryHandler(aggregateTestNoIdReturnRepository, _mapper);

            // Act
            var act = async () => await sut.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}