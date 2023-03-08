using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.GetImplicitKeyAggrRoots;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.GetAllQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ImplicitKeyAggrRoots
{
    public class GetImplicitKeyAggrRootsQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetImplicitKeyAggrRootsQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetImplicitKeyAggrRootsQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public async Task Handle_WithValidQuery_RetrievesImplicitKeyAggrRoots(List<ImplicitKeyAggrRoot> testEntities)
        {
            // Arrange
            var expectedDtos = testEntities.Select(CreateExpectedImplicitKeyAggrRootDto).ToArray();

            var query = new GetImplicitKeyAggrRootsQuery();
            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindAllAsync(CancellationToken.None).Returns(Task.FromResult(testEntities));

            var sut = new GetImplicitKeyAggrRootsQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedDtos);
        }

        public static IEnumerable<object[]> GetTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<ImplicitKeyAggrRoot>(comp => comp.Without(x => x.DomainEvents));
            yield return new object[] { fixture.CreateMany<ImplicitKeyAggrRoot>().ToList() };
            yield return new object[] { fixture.CreateMany<ImplicitKeyAggrRoot>(0).ToList() };
        }

        private static ImplicitKeyAggrRootDto CreateExpectedImplicitKeyAggrRootDto(ImplicitKeyAggrRoot entity)
        {
            return new ImplicitKeyAggrRootDto
            {
                Id = entity.Id,
                Attribute = entity.Attribute,
                ImplicitKeyNestedCompositions = entity.ImplicitKeyNestedCompositions?.Select(CreateExpectedImplicitKeyNestedComposition).ToList() ?? new List<ImplicitKeyAggrRootImplicitKeyNestedCompositionDto>(),
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