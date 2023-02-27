using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.AggregateRoots.GetAggregateRootById;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots;

public class GetAggregateRootByIdQueryHandlerTests
{
    private readonly IMapper _mapper;

    public GetAggregateRootByIdQueryHandlerTests()
    {
        var mapperConfiguration = new MapperConfiguration(config =>
        {
            config.AddMaps(typeof(GetAggregateRootByIdQueryHandler));
        });
        _mapper = mapperConfiguration.CreateMapper();
    }

    [Theory]
    [MemberData(nameof(GetValidAutoFixtures))]
    public async Task Handle_WithValidQuery_RetrievesAggregateRoot(IFixture fixture)
    {
        // Arrange
        var entity = fixture.Create<AggregateRoot>();
        var query = new GetAggregateRootByIdQuery { Id = entity.Id };
        var repository = Substitute.For<IAggregateRootRepository>();
        (await repository.FindByIdAsync(query.Id, CancellationToken.None)).Returns(entity);

        var sut = new GetAggregateRootByIdQueryHandler(repository, _mapper);

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert

    }

    public static IEnumerable<object[]> GetValidAutoFixtures()
    {
        var plainFixture = new Fixture();
        plainFixture.Customize<AggregateRoot>(comp => comp.Without(x => x.DomainEvents));
        yield return new object[] { plainFixture };
    }
}