using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.BasicMappingMapToValueObjects;
using CleanArchitecture.TestApplication.Application.BasicMappingMapToValueObjects.GetSubmissions;
using CleanArchitecture.TestApplication.Application.Tests.BasicMappingMapToValueObjects.Submissions;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities.BasicMappingMapToValueObjects;
using CleanArchitecture.TestApplication.Domain.Repositories.BasicMappingMapToValueObjects;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetAllQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.BasicMappingMapToValueObjects
{
    public class GetSubmissionsQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetSubmissionsQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetSubmissionsQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            yield return new object[] { fixture.CreateMany<Submission>().ToList() };
            yield return new object[] { fixture.CreateMany<Submission>(0).ToList() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesSubmissions(List<Submission> testEntities)
        {
            // Arrange
            var fixture = new Fixture();
            var testQuery = fixture.Create<GetSubmissionsQuery>();
            var submissionRepository = Substitute.For<ISubmissionRepository>();
            submissionRepository.FindAllAsync(CancellationToken.None).Returns(Task.FromResult(testEntities));

            var sut = new GetSubmissionsQueryHandler(submissionRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            SubmissionAssertions.AssertEquivalent(results, testEntities);
        }
    }
}