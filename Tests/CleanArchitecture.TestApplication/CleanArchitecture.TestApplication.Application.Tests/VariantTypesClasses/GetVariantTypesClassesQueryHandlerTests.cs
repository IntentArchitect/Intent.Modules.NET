using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.VariantTypesClasses.GetVariantTypesClasses;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetAllQueryHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.VariantTypesClasses
{
    public class GetVariantTypesClassesQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetVariantTypesClassesQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetVariantTypesClassesQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<VariantTypesClass>(comp => comp.Without(x => x.DomainEvents));
            yield return new object[] { fixture.CreateMany<VariantTypesClass>().ToList() };
            yield return new object[] { fixture.CreateMany<VariantTypesClass>(0).ToList() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesVariantTypesClasses(List<VariantTypesClass> testEntities)
        {
            // Arrange
            var testQuery = new GetVariantTypesClassesQuery();
            var repository = Substitute.For<IVariantTypesClassRepository>();
            repository.FindAllAsync(CancellationToken.None).Returns(Task.FromResult(testEntities));

            var sut = new GetVariantTypesClassesQueryHandler(repository, _mapper);

            // Act
            var result = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            VariantTypesClassAssertions.AssertEquivalent(result, testEntities);
        }
    }
}