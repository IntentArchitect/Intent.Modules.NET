using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.EntityWithMutableOperations.GetEntityWithMutableOperations;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetAllQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.EntityWithMutableOperations
{
    public class GetEntityWithMutableOperationsQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetEntityWithMutableOperationsQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetEntityWithMutableOperationsQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<EntityWithMutableOperation>(comp => comp.Without(x => x.DomainEvents));
            yield return new object[] { fixture.CreateMany<EntityWithMutableOperation>().ToList() };
            yield return new object[] { fixture.CreateMany<EntityWithMutableOperation>(0).ToList() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesEntityWithMutableOperations(List<EntityWithMutableOperation> testEntities)
        {
            // Arrange
            var testQuery = new GetEntityWithMutableOperationsQuery();
            var repository = Substitute.For<IEntityWithMutableOperationRepository>();
            repository.FindAllAsync(CancellationToken.None).Returns(Task.FromResult(testEntities));

            var sut = new GetEntityWithMutableOperationsQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            EntityWithMutableOperationAssertions.AssertEquivalent(result, testEntities);
        }
    }
}