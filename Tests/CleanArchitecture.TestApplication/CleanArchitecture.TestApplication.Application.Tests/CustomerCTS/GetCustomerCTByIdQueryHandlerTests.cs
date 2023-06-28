using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.CustomerCTS.GetCustomerCTById;
using CleanArchitecture.TestApplication.Application.Tests.ComplexTypes.CustomerCTs;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Entities.ComplexTypes;
using CleanArchitecture.TestApplication.Domain.Repositories.ComplexTypes;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetByIdQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.CustomerCTS
{
    public class GetCustomerCTByIdQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetCustomerCTByIdQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetCustomerCTByIdQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<CustomerCT>(comp => comp.Without(x => x.DomainEvents));
            var existingEntity = fixture.Create<CustomerCT>();
            fixture.Customize<GetCustomerCTByIdQuery>(comp => comp.With(p => p.Id, existingEntity.Id));
            var testQuery = fixture.Create<GetCustomerCTByIdQuery>();
            yield return new object[] { testQuery, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesCustomerCT(
            GetCustomerCTByIdQuery testQuery,
            CustomerCT existingEntity)
        {
            // Arrange
            var repository = Substitute.For<ICustomerCTRepository>();
            repository.FindByIdAsync(testQuery.Id, CancellationToken.None).Returns(Task.FromResult(existingEntity));

            var sut = new GetCustomerCTByIdQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            CustomerCTAssertions.AssertEquivalent(result, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ThrowsNotFoundException()
        {
            // Arrange
            var fixture = new Fixture();
            var query = fixture.Create<GetCustomerCTByIdQuery>();

            var repository = Substitute.For<ICustomerCTRepository>();
            repository.FindByIdAsync(query.Id, CancellationToken.None).Returns(Task.FromResult<CustomerCT>(default));

            var sut = new GetCustomerCTByIdQueryHandler(repository, _mapper);

            // Act
            var act = async () => await sut.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}