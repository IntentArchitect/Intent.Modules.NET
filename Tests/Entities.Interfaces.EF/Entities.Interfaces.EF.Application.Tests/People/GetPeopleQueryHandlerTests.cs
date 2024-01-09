using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Entities.Interfaces.EF.Application.People;
using Entities.Interfaces.EF.Application.People.GetPeople;
using Entities.Interfaces.EF.Domain.Entities;
using Entities.Interfaces.EF.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetAllQueryHandlerTests", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.Tests.People
{
    public class GetPeopleQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetPeopleQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetPeopleQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.CreateMany<Person>().ToList() };
            yield return new object[] { fixture.CreateMany<Person>(0).ToList() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesPeople(List<Person> testEntities)
        {
            // Arrange
            var fixture = new Fixture();
            var testQuery = fixture.Create<GetPeopleQuery>();
            var personRepository = Substitute.For<IPersonRepository>();
            personRepository.FindAllAsync(CancellationToken.None).Returns(Task.FromResult(testEntities.Cast<IPerson>().ToList()));

            var sut = new GetPeopleQueryHandler(personRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            PersonAssertions.AssertEquivalent(results, testEntities);
        }
    }
}