using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.AggregateRoots;
using CleanArchitecture.TestApplication.Application.AggregateRoots.GetAggregateRootCompositeManyBS;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedGetAllQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots
{
    public class GetAggregateRootCompositeManyBSQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetAggregateRootCompositeManyBSQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetAggregateRootCompositeManyBSQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }
        
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            yield return new object[] { fixture.Create<AggregateRoot>() };

            fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<AggregateRoot>(comp => comp.With(p => p.Composites, new List<CompositeManyB>()));
            yield return new object[] { fixture.Create<AggregateRoot>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesCompositeManyBs(AggregateRoot existingOwnerEntity)
        {
            // Arrange
            var testQuery = new GetAggregateRootCompositeManyBSQuery();
            testQuery.AggregateRootId = existingOwnerEntity.Id;
            var repository = Substitute.For<IAggregateRootRepository>();
            repository.FindByIdAsync(testQuery.AggregateRootId, CancellationToken.None).Returns(Task.FromResult(existingOwnerEntity));

            var sut = new GetAggregateRootCompositeManyBSQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            AggregateRootAssertions.AssertEquivalent(existingOwnerEntity.Composites, result);
        }
    }
}