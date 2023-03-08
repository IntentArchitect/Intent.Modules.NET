using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.GetImplicitKeyAggrRootById;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.GetByIdQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ImplicitKeyAggrRoots
{
    public class GetImplicitKeyAggrRootByIdQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetImplicitKeyAggrRootByIdQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetImplicitKeyAggrRootByIdQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public async Task Handle_WithValidQuery_RetrievesImplicitKeyAggrRoot(ImplicitKeyAggrRoot testEntity)
        {
            // Arrange
            var expectedDto = CreateExpectedImplicitKeyAggrRootDto(testEntity);

            var query = new GetImplicitKeyAggrRootByIdQuery { Id = testEntity.Id };
            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(query.Id, CancellationToken.None).Returns(Task.FromResult(testEntity));

            var sut = new GetImplicitKeyAggrRootByIdQueryHandler(repository, _mapper);

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
            var query = fixture.Create<GetImplicitKeyAggrRootByIdQuery>();

            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(query.Id, CancellationToken.None).Returns(Task.FromResult<ImplicitKeyAggrRoot>(default));

            var sut = new GetImplicitKeyAggrRootByIdQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(null);
        }

        public static IEnumerable<object[]> GetTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<ImplicitKeyAggrRoot>(comp => comp.Without(x => x.DomainEvents));
            yield return new object[] { fixture.Create<ImplicitKeyAggrRoot>() };
        }

        private static ImplicitKeyAggrRootDto CreateExpectedImplicitKeyAggrRootDto(ImplicitKeyAggrRoot entity)
        {
            return new ImplicitKeyAggrRootDto
            {
                Id = entity.Id,
                Attribute = entity.Attribute,
                ImplicitKeyNestedCompositions = entity.ImplicitKeyNestedCompositions.Select(CreateExpectedImplicitKeyNestedComposition).ToList(),
            };
        }

        private static ImplicitKeyAggrRootImplicitKeyNestedCompositionDto CreateExpectedImplicitKeyNestedComposition(ImplicitKeyNestedComposition entity)
        {
            return new ImplicitKeyAggrRootImplicitKeyNestedCompositionDto
            {
                Attribute = entity.Attribute,
                Id = entity.Id,
            };
        }
    }
}