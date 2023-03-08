using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.GetImplicitKeyAggrRootImplicitKeyNestedCompositionById;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedGetByIdQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ImplicitKeyAggrRoots
{
    public class GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        [Fact]
        public async Task Handle_WithValidQuery_RetrievesImplicitKeyNestedComposition()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            var owner = fixture.Create<ImplicitKeyAggrRoot>();
            var expectedNestedEntityDto = CreateExpectedImplicitKeyAggrRootImplicitKeyNestedCompositionDto(owner.ImplicitKeyNestedCompositions.First());

            var testQuery = new GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQuery();
            testQuery.Id = expectedNestedEntityDto.Id;
            testQuery.ImplicitKeyAggrRootId = owner.Id;

            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testQuery.ImplicitKeyAggrRootId, CancellationToken.None).Returns(Task.FromResult(owner));

            var sut = new GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQueryHandler(repository, _mapper);

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
            fixture.Customize<ImplicitKeyAggrRoot>(comp => comp.With(p => p.ImplicitKeyNestedCompositions, new List<ImplicitKeyNestedComposition>()));
            var owner = fixture.Create<ImplicitKeyAggrRoot>();
            var testQuery = fixture.Create<GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQuery>();

            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testQuery.ImplicitKeyAggrRootId, CancellationToken.None).Returns(Task.FromResult(owner));

            var sut = new GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            result.Should().Be(null);
        }

        private static ImplicitKeyAggrRootImplicitKeyNestedCompositionDto CreateExpectedImplicitKeyAggrRootImplicitKeyNestedCompositionDto(ImplicitKeyNestedComposition entity)
        {
            return new ImplicitKeyAggrRootImplicitKeyNestedCompositionDto
            {
                Attribute = entity.Attribute,
                Id = entity.Id,
            };
        }
    }
}