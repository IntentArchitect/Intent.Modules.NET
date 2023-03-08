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

        [Theory]
        [MemberData(nameof(GetTestData))]
        public async Task Handle_WithValidQuery_RetrievesCompositeManyBs(AggregateRoot owner)
        {
            // Arrange
            var testQuery = new GetAggregateRootCompositeManyBSQuery();
            testQuery.AggregateRootId = owner.Id;
            var repository = Substitute.For<IAggregateRootRepository>();
            repository.FindByIdAsync(testQuery.AggregateRootId, CancellationToken.None).Returns(Task.FromResult(owner));

            var sut = new GetAggregateRootCompositeManyBSQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(owner.Composites.Select(CreateExpectedAggregateRootCompositeManyBDto));
        }

        public static IEnumerable<object[]> GetTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            yield return new object[] { fixture.Create<AggregateRoot>() };

            fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<AggregateRoot>(comp => comp.With(p => p.Composites, new List<CompositeManyB>()));
            yield return new object[] { fixture.Create<AggregateRoot>() };
        }

        private static AggregateRootCompositeManyBDto CreateExpectedAggregateRootCompositeManyBDto(CompositeManyB entity)
        {
            return new AggregateRootCompositeManyBDto
            {
                CompositeAttr = entity.CompositeAttr,
                SomeDate = entity.SomeDate,
                AggregateRootId = entity.AggregateRootId,
                Id = entity.Id,
                Composite = entity.Composite != null ? CreateExpectedCompositeSingleBB(entity.Composite) : null,
                Composites = entity.Composites?.Select(CreateExpectedCompositeManyBB).ToList() ?? new List<AggregateRootCompositeManyBCompositeManyBBDto>(),
            };
        }

        private static AggregateRootCompositeManyBCompositeSingleBBDto CreateExpectedCompositeSingleBB(CompositeSingleBB entity)
        {
            return new AggregateRootCompositeManyBCompositeSingleBBDto
            {
                CompositeAttr = entity.CompositeAttr,
                Id = entity.Id,
            };
        }

        private static AggregateRootCompositeManyBCompositeManyBBDto CreateExpectedCompositeManyBB(CompositeManyBB entity)
        {
            return new AggregateRootCompositeManyBCompositeManyBBDto
            {
                CompositeAttr = entity.CompositeAttr,
                CompositeManyBId = entity.CompositeManyBId,
                Id = entity.Id,
            };
        }
    }
}