using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.ClassWithEnums;
using CleanArchitecture.Comprehensive.Application.ClassWithEnums.GetClassWithEnums;
using CleanArchitecture.Comprehensive.Application.Tests.Enums.ClassWithEnums;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Entities.Enums;
using CleanArchitecture.Comprehensive.Domain.Repositories.Enums;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetAllQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.ClassWithEnums
{
    public class GetClassWithEnumsQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetClassWithEnumsQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetClassWithEnumsQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            yield return new object[] { fixture.CreateMany<Domain.Entities.Enums.ClassWithEnums>().ToList() };
            yield return new object[] { fixture.CreateMany<Domain.Entities.Enums.ClassWithEnums>(0).ToList() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesClassWithEnums(List<Domain.Entities.Enums.ClassWithEnums> testEntities)
        {
            // Arrange
            var fixture = new Fixture();
            var testQuery = fixture.Create<GetClassWithEnumsQuery>();
            var classWithEnumsRepository = Substitute.For<IClassWithEnumsRepository>();
            classWithEnumsRepository.FindAllAsync(CancellationToken.None).Returns(Task.FromResult(testEntities));

            var sut = new GetClassWithEnumsQueryHandler(classWithEnumsRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            ClassWithEnumsAssertions.AssertEquivalent(results, testEntities);
        }
    }
}