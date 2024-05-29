using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.ODataAggs;
using CleanArchitecture.Comprehensive.Application.ODataAggs.GetODataAggById;
using CleanArchitecture.Comprehensive.Application.Tests.ODataQuery.ODataAggs;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.ODataQuery;
using CleanArchitecture.Comprehensive.Domain.Repositories.ODataQuery;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetByIdQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.ODataAggs
{
    public class GetODataAggByIdQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetODataAggByIdQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetODataAggByIdQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);

            var existingEntity = fixture.Create<ODataAgg>();
            fixture.Customize<GetODataAggByIdQuery>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testQuery = fixture.Create<GetODataAggByIdQuery>();
            yield return new object[] { testQuery, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesODataAgg(GetODataAggByIdQuery testQuery, ODataAgg existingEntity)
        {
            // Arrange
            var oDataAggRepository = Substitute.For<IODataAggRepository>();
            oDataAggRepository.FindByIdAsync(testQuery.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));


            var sut = new GetODataAggByIdQueryHandler(oDataAggRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            ODataAggAssertions.AssertEquivalent(results, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ThrowsNotFoundException()
        {
            // Arrange
            var fixture = new Fixture();
            var query = fixture.Create<GetODataAggByIdQuery>();
            var oDataAggRepository = Substitute.For<IODataAggRepository>();
            oDataAggRepository.FindByIdAsync(query.Id, CancellationToken.None)!.Returns(Task.FromResult<ODataAgg>(default));

            var sut = new GetODataAggByIdQueryHandler(oDataAggRepository, _mapper);

            // Act
            var act = async () => await sut.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}