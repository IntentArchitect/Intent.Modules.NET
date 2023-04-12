using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.EntityWithCtors.GetEntityWithCtorById;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetByIdQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.EntityWithCtors
{
    public class GetEntityWithCtorByIdQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetEntityWithCtorByIdQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetEntityWithCtorByIdQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<EntityWithCtor>(comp => comp.Without(x => x.DomainEvents));
            var existingEntity = fixture.Create<EntityWithCtor>();
            fixture.Customize<GetEntityWithCtorByIdQuery>(comp => comp.With(p => p.Id, existingEntity.Id));
            var testQuery = fixture.Create<GetEntityWithCtorByIdQuery>();
            yield return new object[] { testQuery, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesEntityWithCtor(
            GetEntityWithCtorByIdQuery testQuery,
            EntityWithCtor existingEntity)
        {
            // Arrange
            var repository = Substitute.For<IEntityWithCtorRepository>();
            repository.FindByIdAsync(testQuery.Id, CancellationToken.None).Returns(Task.FromResult(existingEntity));

            var sut = new GetEntityWithCtorByIdQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            EntityWithCtorAssertions.AssertEquivalent(result, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ReturnsEmptyResult()
        {
            // Arrange
            var fixture = new Fixture();
            var query = fixture.Create<GetEntityWithCtorByIdQuery>();

            var repository = Substitute.For<IEntityWithCtorRepository>();
            repository.FindByIdAsync(query.Id, CancellationToken.None).Returns(Task.FromResult<EntityWithCtor>(default));

            var sut = new GetEntityWithCtorByIdQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(null);
        }
    }
}