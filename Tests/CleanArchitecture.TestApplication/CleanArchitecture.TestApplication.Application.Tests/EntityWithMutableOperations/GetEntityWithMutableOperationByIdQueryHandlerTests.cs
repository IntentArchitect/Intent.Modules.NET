using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.EntityWithMutableOperations.GetEntityWithMutableOperationById;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetByIdQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.EntityWithMutableOperations
{
    public class GetEntityWithMutableOperationByIdQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetEntityWithMutableOperationByIdQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetEntityWithMutableOperationByIdQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<EntityWithMutableOperation>(comp => comp.Without(x => x.DomainEvents));
            var existingEntity = fixture.Create<EntityWithMutableOperation>();
            fixture.Customize<GetEntityWithMutableOperationByIdQuery>(comp => comp.With(p => p.Id, existingEntity.Id));
            var testQuery = fixture.Create<GetEntityWithMutableOperationByIdQuery>();
            yield return new object[] { testQuery, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesEntityWithMutableOperation(
            GetEntityWithMutableOperationByIdQuery testQuery,
            EntityWithMutableOperation existingEntity)
        {
            // Arrange
            var repository = Substitute.For<IEntityWithMutableOperationRepository>();
            repository.FindByIdAsync(testQuery.Id, CancellationToken.None).Returns(Task.FromResult(existingEntity));

            var sut = new GetEntityWithMutableOperationByIdQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            EntityWithMutableOperationAssertions.AssertEquivalent(result, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ReturnsEmptyResult()
        {
            // Arrange
            var fixture = new Fixture();
            var query = fixture.Create<GetEntityWithMutableOperationByIdQuery>();

            var repository = Substitute.For<IEntityWithMutableOperationRepository>();
            repository.FindByIdAsync(query.Id, CancellationToken.None).Returns(Task.FromResult<EntityWithMutableOperation>(default));

            var sut = new GetEntityWithMutableOperationByIdQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(null);
        }
    }
}