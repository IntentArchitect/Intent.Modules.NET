using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Tests.CompositeKeys.WithCompositeKeys;
using CleanArchitecture.TestApplication.Application.WithCompositeKeys;
using CleanArchitecture.TestApplication.Application.WithCompositeKeys.GetWithCompositeKeyById;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Entities.CompositeKeys;
using CleanArchitecture.TestApplication.Domain.Repositories.CompositeKeys;
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
            fixture.Register<DomainEvent>(() => null!);

            var existingEntity = fixture.Create<WithCompositeKey>();
            fixture.Customize<GetWithCompositeKeyByIdQuery>(comp => comp
                .With(x => x.Key1Id, existingEntity.Key1Id)
                .With(x => x.Key2Id, existingEntity.Key2Id));
            var testQuery = fixture.Create<GetWithCompositeKeyByIdQuery>();
            yield return new object[] { testQuery, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesWithCompositeKey(
            GetWithCompositeKeyByIdQuery testQuery,
            WithCompositeKey existingEntity)
        {
            // Arrange
            var withCompositeKeyRepository = Substitute.For<IWithCompositeKeyRepository>();
            withCompositeKeyRepository.FindByIdAsync((testQuery.Key1Id, testQuery.Key2Id), CancellationToken.None)!.Returns(Task.FromResult(existingEntity));


            var sut = new GetWithCompositeKeyByIdQueryHandler(withCompositeKeyRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            WithCompositeKeyAssertions.AssertEquivalent(results, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ThrowsNotFoundException()
        {
            // Arrange
            var fixture = new Fixture();
            var query = fixture.Create<GetWithCompositeKeyByIdQuery>();
            var withCompositeKeyRepository = Substitute.For<IWithCompositeKeyRepository>();
            withCompositeKeyRepository.FindByIdAsync((query.Key1Id, query.Key2Id), CancellationToken.None)!.Returns(Task.FromResult<WithCompositeKey>(default));

            var sut = new GetWithCompositeKeyByIdQueryHandler(withCompositeKeyRepository, _mapper);

            // Act
            var act = async () => await sut.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}