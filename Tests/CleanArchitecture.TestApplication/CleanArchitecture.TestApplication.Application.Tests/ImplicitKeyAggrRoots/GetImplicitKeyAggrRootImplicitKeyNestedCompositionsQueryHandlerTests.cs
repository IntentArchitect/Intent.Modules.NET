using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.GetImplicitKeyAggrRootImplicitKeyNestedCompositions;
using CleanArchitecture.TestApplication.Application.Tests.CRUD.ImplicitKeyAggrRoots;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
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

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);

            var existingOwnerEntity = fixture.Create<ImplicitKeyAggrRoot>();
            yield return new object[] { existingOwnerEntity };
            fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);

            existingOwnerEntity = fixture.Create<ImplicitKeyAggrRoot>();
            yield return new object[] { existingOwnerEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesImplicitKeyNestedCompositions(ImplicitKeyAggrRoot existingOwnerEntity)
        {
            // Arrange
            var fixture = new Fixture();
            var testQuery = fixture.Create<GetImplicitKeyAggrRootImplicitKeyNestedCompositionsQuery>();
            var implicitKeyAggrRootRepository = Substitute.For<IImplicitKeyAggrRootRepository>();
            implicitKeyAggrRootRepository.FindByIdAsync(testQuery.ImplicitKeyAggrRootId, CancellationToken.None)!.Returns(Task.FromResult(existingOwnerEntity));

            var sut = new GetImplicitKeyAggrRootImplicitKeyNestedCompositionsQueryHandler(implicitKeyAggrRootRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            ImplicitKeyAggrRootAssertions.AssertEquivalent(results, existingOwnerEntity.ImplicitKeyNestedCompositions);
        }
    }
}