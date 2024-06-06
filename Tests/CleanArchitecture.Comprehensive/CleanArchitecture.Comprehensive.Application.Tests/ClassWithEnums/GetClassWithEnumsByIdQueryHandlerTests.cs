using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.ClassWithEnums;
using CleanArchitecture.Comprehensive.Application.ClassWithEnums.GetClassWithEnumsById;
using CleanArchitecture.Comprehensive.Application.Tests.Enums.ClassWithEnums;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.Enums;
using CleanArchitecture.Comprehensive.Domain.Repositories.Enums;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetByIdQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.ClassWithEnums
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
            fixture.Register<DomainEvent>(() => null!);

            var existingEntity = fixture.Create<Domain.Entities.Enums.ClassWithEnums>();
            fixture.Customize<GetClassWithEnumsByIdQuery>(comp => comp.With(x => x.Id, existingEntity.Id));
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
            var classWithEnumsRepository = Substitute.For<IClassWithEnumsRepository>();
            classWithEnumsRepository.FindByIdAsync(testQuery.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));


            var sut = new GetClassWithEnumsByIdQueryHandler(classWithEnumsRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            ClassWithEnumsAssertions.AssertEquivalent(results, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ThrowsNotFoundException()
        {
            // Arrange
            var fixture = new Fixture();
            var query = fixture.Create<GetClassWithEnumsByIdQuery>();
            var classWithEnumsRepository = Substitute.For<IClassWithEnumsRepository>();
            classWithEnumsRepository.FindByIdAsync(query.Id, CancellationToken.None)!.Returns(Task.FromResult<Domain.Entities.Enums.ClassWithEnums>(default));

            var sut = new GetClassWithEnumsByIdQueryHandler(classWithEnumsRepository, _mapper);

            // Act
            var act = async () => await sut.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}