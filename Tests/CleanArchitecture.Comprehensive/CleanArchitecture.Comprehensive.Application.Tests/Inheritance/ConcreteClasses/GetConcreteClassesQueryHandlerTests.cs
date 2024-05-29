using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses;
using CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses.GetConcreteClasses;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Entities.Inheritance;
using CleanArchitecture.Comprehensive.Domain.Repositories.Inheritance;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetAllQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.Inheritance.ConcreteClasses
{
    public class GetConcreteClassesQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetConcreteClassesQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetConcreteClassesQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            yield return new object[] { fixture.CreateMany<ConcreteClass>().ToList() };
            yield return new object[] { fixture.CreateMany<ConcreteClass>(0).ToList() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesConcreteClasses(List<ConcreteClass> testEntities)
        {
            // Arrange
            var fixture = new Fixture();
            var testQuery = fixture.Create<GetConcreteClassesQuery>();
            var concreteClassRepository = Substitute.For<IConcreteClassRepository>();
            concreteClassRepository.FindAllAsync(CancellationToken.None).Returns(Task.FromResult(testEntities));

            var sut = new GetConcreteClassesQueryHandler(concreteClassRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            ConcreteClassAssertions.AssertEquivalent(results, testEntities);
        }
    }
}