using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.ClassWithEnums.GetClassWithEnumsById;
using CleanArchitecture.TestApplication.Application.Tests.Enums.ClassWithEnums;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities.Enums;
using CleanArchitecture.TestApplication.Domain.Repositories.Enums;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetByIdQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ClassWithEnums
{
    public class GetClassWithEnumsByIdQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetClassWithEnumsByIdQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetClassWithEnumsByIdQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<Domain.Entities.Enums.ClassWithEnums>(comp => comp.Without(x => x.DomainEvents));
            var existingEntity = fixture.Create<Domain.Entities.Enums.ClassWithEnums>();
            fixture.Customize<GetClassWithEnumsByIdQuery>(comp => comp.With(p => p.Id, existingEntity.Id));
            var testQuery = fixture.Create<GetClassWithEnumsByIdQuery>();
            yield return new object[] { testQuery, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesClassWithEnums(
            GetClassWithEnumsByIdQuery testQuery,
            Domain.Entities.Enums.ClassWithEnums existingEntity)
        {
            // Arrange
            var repository = Substitute.For<IClassWithEnumsRepository>();
            repository.FindByIdAsync(testQuery.Id, CancellationToken.None).Returns(Task.FromResult(existingEntity));

            var sut = new GetClassWithEnumsByIdQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            ClassWithEnumsAssertions.AssertEquivalent(result, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ReturnsEmptyResult()
        {
            // Arrange
            var fixture = new Fixture();
            var query = fixture.Create<GetClassWithEnumsByIdQuery>();

            var repository = Substitute.For<IClassWithEnumsRepository>();
            repository.FindByIdAsync(query.Id, CancellationToken.None).Returns(Task.FromResult<Domain.Entities.Enums.ClassWithEnums>(default));

            var sut = new GetClassWithEnumsByIdQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(null);
        }
    }
}