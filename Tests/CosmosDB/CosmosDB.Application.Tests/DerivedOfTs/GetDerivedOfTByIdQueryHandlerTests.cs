using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CosmosDB.Application.DerivedOfTS;
using CosmosDB.Application.DerivedOfTS.GetDerivedOfTById;
using CosmosDB.Application.Tests.DerivedOfTs;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetByIdQueryHandlerTests", Version = "1.0")]

namespace CosmosDB.Application.Tests.DerivedOfTS
{
    public class GetDerivedOfTByIdQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetDerivedOfTByIdQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetDerivedOfTByIdQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);

            var existingEntity = fixture.Create<DerivedOfT>();
            fixture.Customize<GetDerivedOfTByIdQuery>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testQuery = fixture.Create<GetDerivedOfTByIdQuery>();
            yield return new object[] { testQuery, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesDerivedOfT(
            GetDerivedOfTByIdQuery testQuery,
            DerivedOfT existingEntity)
        {
            // Arrange
            var derivedOfTRepository = Substitute.For<IDerivedOfTRepository>();
            derivedOfTRepository.FindByIdAsync(testQuery.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));


            var sut = new GetDerivedOfTByIdQueryHandler(derivedOfTRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            DerivedOfTAssertions.AssertEquivalent(results, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ThrowsNotFoundException()
        {
            // Arrange
            var fixture = new Fixture();
            var query = fixture.Create<GetDerivedOfTByIdQuery>();
            var derivedOfTRepository = Substitute.For<IDerivedOfTRepository>();
            derivedOfTRepository.FindByIdAsync(query.Id, CancellationToken.None)!.Returns(Task.FromResult<DerivedOfT>(default));

            var sut = new GetDerivedOfTByIdQueryHandler(derivedOfTRepository, _mapper);

            // Act
            var act = async () => await sut.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}