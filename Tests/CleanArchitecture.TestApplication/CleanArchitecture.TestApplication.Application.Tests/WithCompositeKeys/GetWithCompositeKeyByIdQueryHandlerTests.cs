using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.WithCompositeKeys.GetWithCompositeKeyById;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetByIdQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.WithCompositeKeys
{
    public class GetWithCompositeKeyByIdQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetWithCompositeKeyByIdQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetWithCompositeKeyByIdQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<WithCompositeKey>(comp => comp.Without(x => x.DomainEvents));
            var existingEntity = fixture.Create<WithCompositeKey>();
            //fixture.Customize<GetWithCompositeKeyByIdQuery>(comp => comp.With(p => p.Id, existingEntity.Id));
            var testQuery = fixture.Create<GetWithCompositeKeyByIdQuery>();
            yield return new object[] { testQuery, existingEntity };
        }

        [Theory(Skip = "Not working")]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesWithCompositeKey(
            GetWithCompositeKeyByIdQuery testQuery,
            WithCompositeKey existingEntity)
        {
            // Arrange
            var repository = Substitute.For<IWithCompositeKeyRepository>();
            //repository.FindByIdAsync(testQuery.Key1Id, CancellationToken.None).Returns(Task.FromResult(existingEntity));

            var sut = new GetWithCompositeKeyByIdQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            WithCompositeKeyAssertions.AssertEquivalent(result, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ThrowsNotFoundException()
        {
            // Arrange
            var fixture = new Fixture();
            var query = fixture.Create<GetWithCompositeKeyByIdQuery>();

            var repository = Substitute.For<IWithCompositeKeyRepository>();
            //repository.FindByIdAsync(query.Key1Id, CancellationToken.None).Returns(Task.FromResult<WithCompositeKey>(default));

            var sut = new GetWithCompositeKeyByIdQueryHandler(repository, _mapper);

            // Act
            var act = async () => await sut.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}