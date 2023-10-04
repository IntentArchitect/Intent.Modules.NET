using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using RichDomain.Application.People;
using RichDomain.Application.People.GetPersonById;
using RichDomain.Domain.Common;
using RichDomain.Domain.Common.Exceptions;
using RichDomain.Domain.Entities;
using RichDomain.Domain.Repositories;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetByIdQueryHandlerTests", Version = "1.0")]

namespace RichDomain.Application.Tests.People
{
    public class GetPersonByIdQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetPersonByIdQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetPersonByIdQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);

            var existingEntity = fixture.Create<Person>();
            fixture.Customize<GetPersonByIdQuery>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testQuery = fixture.Create<GetPersonByIdQuery>();
            yield return new object[] { testQuery, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesPerson(GetPersonByIdQuery testQuery, Person existingEntity)
        {
            // Arrange
            var personRepository = Substitute.For<IPersonRepository>();
            personRepository.FindByIdAsync(testQuery.Id, CancellationToken.None)!.Returns(Task.FromResult<IPerson>(existingEntity));


            var sut = new GetPersonByIdQueryHandler(personRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            PersonAssertions.AssertEquivalent(results, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ThrowsNotFoundException()
        {
            // Arrange
            var fixture = new Fixture();
            var query = fixture.Create<GetPersonByIdQuery>();
            var personRepository = Substitute.For<IPersonRepository>();
            personRepository.FindByIdAsync(query.Id, CancellationToken.None)!.Returns(Task.FromResult<IPerson>(default));

            var sut = new GetPersonByIdQueryHandler(personRepository, _mapper);

            // Act
            var act = async () => await sut.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}