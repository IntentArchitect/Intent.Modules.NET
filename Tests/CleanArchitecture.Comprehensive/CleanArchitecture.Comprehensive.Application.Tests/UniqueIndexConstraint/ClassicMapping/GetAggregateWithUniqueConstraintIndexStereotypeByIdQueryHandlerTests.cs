using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Tests.UniqueIndexConstraint.AggregateWithUniqueConstraintIndexStereotypes;
using CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint;
using CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.GetAggregateWithUniqueConstraintIndexStereotypeById;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.UniqueIndexConstraint;
using CleanArchitecture.Comprehensive.Domain.Repositories.UniqueIndexConstraint;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetByIdQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.UniqueIndexConstraint.ClassicMapping
{
    public class GetAggregateWithUniqueConstraintIndexStereotypeByIdQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetAggregateWithUniqueConstraintIndexStereotypeByIdQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetAggregateWithUniqueConstraintIndexStereotypeByIdQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);

            var existingEntity = fixture.Create<AggregateWithUniqueConstraintIndexStereotype>();
            fixture.Customize<GetAggregateWithUniqueConstraintIndexStereotypeByIdQuery>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testQuery = fixture.Create<GetAggregateWithUniqueConstraintIndexStereotypeByIdQuery>();
            yield return new object[] { testQuery, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesAggregateWithUniqueConstraintIndexStereotype(
            GetAggregateWithUniqueConstraintIndexStereotypeByIdQuery testQuery,
            AggregateWithUniqueConstraintIndexStereotype existingEntity)
        {
            // Arrange
            var aggregateWithUniqueConstraintIndexStereotypeRepository = Substitute.For<IAggregateWithUniqueConstraintIndexStereotypeRepository>();
            aggregateWithUniqueConstraintIndexStereotypeRepository.FindByIdAsync(testQuery.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));


            var sut = new GetAggregateWithUniqueConstraintIndexStereotypeByIdQueryHandler(aggregateWithUniqueConstraintIndexStereotypeRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            AggregateWithUniqueConstraintIndexStereotypeAssertions.AssertEquivalent(results, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ThrowsNotFoundException()
        {
            // Arrange
            var fixture = new Fixture();
            var query = fixture.Create<GetAggregateWithUniqueConstraintIndexStereotypeByIdQuery>();
            var aggregateWithUniqueConstraintIndexStereotypeRepository = Substitute.For<IAggregateWithUniqueConstraintIndexStereotypeRepository>();
            aggregateWithUniqueConstraintIndexStereotypeRepository.FindByIdAsync(query.Id, CancellationToken.None)!.Returns(Task.FromResult<AggregateWithUniqueConstraintIndexStereotype>(default));

            var sut = new GetAggregateWithUniqueConstraintIndexStereotypeByIdQueryHandler(aggregateWithUniqueConstraintIndexStereotypeRepository, _mapper);

            // Act
            var act = async () => await sut.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}