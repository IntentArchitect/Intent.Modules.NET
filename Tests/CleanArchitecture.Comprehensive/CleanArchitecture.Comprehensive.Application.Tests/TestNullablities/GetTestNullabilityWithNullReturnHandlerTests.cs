using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.TestNullablities;
using CleanArchitecture.Comprehensive.Application.TestNullablities.GetTestNullabilityWithNullReturn;
using CleanArchitecture.Comprehensive.Application.Tests.Nullability.TestNullablities;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Entities.Nullability;
using CleanArchitecture.Comprehensive.Domain.Repositories.Nullability;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetByIdQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.TestNullablities
{
    public class GetTestNullabilityWithNullReturnHandlerTests
    {
        private readonly IMapper _mapper;

        public GetTestNullabilityWithNullReturnHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetTestNullabilityWithNullReturnHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);

            var existingEntity = fixture.Create<TestNullablity>();
            fixture.Customize<GetTestNullabilityWithNullReturn>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testQuery = fixture.Create<GetTestNullabilityWithNullReturn>();
            yield return new object[] { testQuery, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesTestNullablity(
            GetTestNullabilityWithNullReturn testQuery,
            TestNullablity existingEntity)
        {
            // Arrange
            var testNullablityRepository = Substitute.For<ITestNullablityRepository>();
            testNullablityRepository.FindByIdAsync(testQuery.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));


            var sut = new GetTestNullabilityWithNullReturnHandler(testNullablityRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            TestNullablityAssertions.AssertEquivalent(results, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ExpectsDefaultReturned()
        {
            // Arrange
            var fixture = new Fixture();
            var query = fixture.Create<GetTestNullabilityWithNullReturn>();
            var testNullablityRepository = Substitute.For<ITestNullablityRepository>();
            testNullablityRepository.FindByIdAsync(query.Id, CancellationToken.None)!.Returns(Task.FromResult<TestNullablity>(default));

            var sut = new GetTestNullabilityWithNullReturnHandler(testNullablityRepository, _mapper);

            // Act
            var results = await sut.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(default, results);
        }
    }
}