using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.BasicMappingMapToValueObjects;
using CleanArchitecture.TestApplication.Application.BasicMappingMapToValueObjects.CreateSubmission;
using CleanArchitecture.TestApplication.Application.Tests.BasicMappingMapToValueObjects.Submissions;
using CleanArchitecture.TestApplication.Application.Tests.Extensions;
using CleanArchitecture.TestApplication.Domain.BasicMappingMapToValueObjects;
using CleanArchitecture.TestApplication.Domain.Entities.BasicMappingMapToValueObjects;
using CleanArchitecture.TestApplication.Domain.Repositories.BasicMappingMapToValueObjects;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.BasicMappingMapToValueObjects
{
    public class CreateSubmissionCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateSubmissionCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsSubmissionToRepository(CreateSubmissionCommand testCommand)
        {
            // Arrange
            var submissionRepository = Substitute.For<ISubmissionRepository>();
            var expectedSubmissionId = new Fixture().Create<System.Guid>();
            Submission addedSubmission = null;
            submissionRepository.OnAdd(ent => addedSubmission = ent);
            submissionRepository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ => addedSubmission.Id = expectedSubmissionId);

            var sut = new CreateSubmissionCommandHandler(submissionRepository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedSubmissionId);
            await submissionRepository.UnitOfWork.Received(1).SaveChangesAsync();
            SubmissionAssertions.AssertEquivalent(testCommand, addedSubmission);
        }
    }
}