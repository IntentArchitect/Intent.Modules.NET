using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.EntityWithCtors.GetEntityWithCtors;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetAllQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.EntityWithCtors
{
    public class GetEntityWithCtorsQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetEntityWithCtorsQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetEntityWithCtorsQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<EntityWithCtor>(comp => comp.Without(x => x.DomainEvents));
            yield return new object[] { fixture.CreateMany<EntityWithCtor>().ToList() };
            yield return new object[] { fixture.CreateMany<EntityWithCtor>(0).ToList() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesEntityWithCtors(List<EntityWithCtor> testEntities)
        {
            // Arrange
            var testQuery = new GetEntityWithCtorsQuery();
            var repository = Substitute.For<IEntityWithCtorRepository>();
            repository.FindAllAsync(CancellationToken.None).Returns(Task.FromResult(testEntities));

            var sut = new GetEntityWithCtorsQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            EntityWithCtorAssertions.AssertEquivalent(result, testEntities);
        }
    }
}