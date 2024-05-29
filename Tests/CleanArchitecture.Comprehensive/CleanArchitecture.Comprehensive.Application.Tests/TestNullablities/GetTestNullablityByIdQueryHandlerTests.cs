using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.TestNullablities;
using CleanArchitecture.Comprehensive.Application.TestNullablities.GetTestNullablityById;
using CleanArchitecture.Comprehensive.Application.Tests.Nullability.TestNullablities;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
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
    public class GetTestNullablityByIdQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetTestNullablityByIdQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetTestNullablityByIdQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);

            var existingEntity = fixture.Create<TestNullablity>();
            fixture.Customize<GetTestNullablityByIdQuery>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testQuery = fixture.Create<GetTestNullablityByIdQuery>();
            yield return new object[] { testQuery, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesTestNullablity(
            GetTestNullablityByIdQuery testQuery,
            TestNullablity existingEntity)
        {
            // Arrange
            var testNullablityRepository = Substitute.For<ITestNullablityRepository>();
            testNullablityRepository.FindByIdAsync(testQuery.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));


            var sut = new GetTestNullablityByIdQueryHandler(testNullablityRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            TestNullablityAssertions.AssertEquivalent(results, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ThrowsNotFoundException()
        {
            // Arrange
            var fixture = new Fixture();
            var query = fixture.Create<GetTestNullablityByIdQuery>();
            var testNullablityRepository = Substitute.For<ITestNullablityRepository>();
            testNullablityRepository.FindByIdAsync(query.Id, CancellationToken.None)!.Returns(Task.FromResult<TestNullablity>(default));

            var sut = new GetTestNullablityByIdQueryHandler(testNullablityRepository, _mapper);

            // Act
            var act = async () => await sut.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}