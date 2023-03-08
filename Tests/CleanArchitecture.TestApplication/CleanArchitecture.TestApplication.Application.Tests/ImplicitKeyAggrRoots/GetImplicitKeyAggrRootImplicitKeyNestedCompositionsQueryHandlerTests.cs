using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.GetImplicitKeyAggrRootImplicitKeyNestedCompositions;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedGetAllQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ImplicitKeyAggrRoots
{
    public class GetImplicitKeyAggrRootImplicitKeyNestedCompositionsQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetImplicitKeyAggrRootImplicitKeyNestedCompositionsQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetImplicitKeyAggrRootImplicitKeyNestedCompositionsQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public async Task Handle_WithValidQuery_RetrievesImplicitKeyNestedCompositions(ImplicitKeyAggrRoot owner)
        {
            // Arrange
            var testQuery = new GetImplicitKeyAggrRootImplicitKeyNestedCompositionsQuery();
            testQuery.ImplicitKeyAggrRootId = owner.Id;
            var repository = Substitute.For<IImplicitKeyAggrRootRepository>();
            repository.FindByIdAsync(testQuery.ImplicitKeyAggrRootId, CancellationToken.None).Returns(Task.FromResult(owner));

            var sut = new GetImplicitKeyAggrRootImplicitKeyNestedCompositionsQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(owner.ImplicitKeyNestedCompositions.Select(CreateExpectedImplicitKeyAggrRootImplicitKeyNestedCompositionDto));
        }

        public static IEnumerable<object[]> GetTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            yield return new object[] { fixture.Create<ImplicitKeyAggrRoot>() };

            fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<ImplicitKeyAggrRoot>(comp => comp.With(p => p.ImplicitKeyNestedCompositions, new List<ImplicitKeyNestedComposition>()));
            yield return new object[] { fixture.Create<ImplicitKeyAggrRoot>() };
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