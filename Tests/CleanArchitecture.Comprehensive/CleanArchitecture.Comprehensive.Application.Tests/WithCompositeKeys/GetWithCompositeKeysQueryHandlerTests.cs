using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Tests.CompositeKeys.WithCompositeKeys;
using CleanArchitecture.Comprehensive.Application.WithCompositeKeys;
using CleanArchitecture.Comprehensive.Application.WithCompositeKeys.GetWithCompositeKeys;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Entities.CompositeKeys;
using CleanArchitecture.Comprehensive.Domain.Repositories.CompositeKeys;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetAllQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.WithCompositeKeys
{
    public class GetWithCompositeKeysQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetWithCompositeKeysQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetWithCompositeKeysQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            yield return new object[] { fixture.CreateMany<WithCompositeKey>().ToList() };
            yield return new object[] { fixture.CreateMany<WithCompositeKey>(0).ToList() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesWithCompositeKeys(List<WithCompositeKey> testEntities)
        {
            // Arrange
            var fixture = new Fixture();
            var testQuery = fixture.Create<GetWithCompositeKeysQuery>();
            var withCompositeKeyRepository = Substitute.For<IWithCompositeKeyRepository>();
            withCompositeKeyRepository.FindAllAsync(CancellationToken.None).Returns(Task.FromResult(testEntities));

            var sut = new GetWithCompositeKeysQueryHandler(withCompositeKeyRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            WithCompositeKeyAssertions.AssertEquivalent(results, testEntities);
        }
    }
}