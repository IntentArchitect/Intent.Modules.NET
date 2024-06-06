using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.Common.Behaviours;
using CleanArchitecture.Comprehensive.Application.Common.Validation;
using CleanArchitecture.Comprehensive.Application.DDD;
using CleanArchitecture.Comprehensive.Application.DDD.CreateCustomer;
using FluentAssertions;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.FluentValidation.FluentValidationTest", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.DDD
{
    public class CreateCustomerCommandValidatorTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<CreateCustomerCommand>();
            testCommand.Name = $"{string.Join(string.Empty, fixture.CreateMany<char>(100))}";
            testCommand.Surname = $"{string.Join(string.Empty, fixture.CreateMany<char>(100))}";
            testCommand.Email = $"{string.Join(string.Empty, fixture.CreateMany<char>(100))}";
            yield return new object[] { testCommand };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Validate_WithValidCommand_PassesValidation(CreateCustomerCommand testCommand)
        {
            // Arrange
            var validator = GetValidationBehaviour();
            var expectedId = new Fixture().Create<System.Guid>();
            // Act
            var result = await validator.Handle(testCommand, () => Task.FromResult(expectedId), CancellationToken.None);

            // Assert
            result.Should().Be(expectedId);
        }

        public static IEnumerable<object[]> GetFailedResultTestData()
        {
            var fixture = new Fixture();
            fixture.Customize<CreateCustomerCommand>(comp => comp
                .With(x => x.Name, () => default)
                .With(x => x.Surname, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(99))}")
                .With(x => x.Email, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(99))}"));
            var testCommand = fixture.Create<CreateCustomerCommand>();
            yield return new object[] { testCommand, "Name", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<CreateCustomerCommand>(comp => comp
                .With(x => x.Name, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(101))}")
                .With(x => x.Surname, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(99))}")
                .With(x => x.Email, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(99))}"));
            testCommand = fixture.Create<CreateCustomerCommand>();
            yield return new object[] { testCommand, "Name", "must be 100 characters or fewer" };

            fixture = new Fixture();
            fixture.Customize<CreateCustomerCommand>(comp => comp
                .With(x => x.Surname, () => default)
                .With(x => x.Name, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(99))}")
                .With(x => x.Email, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(99))}"));
            testCommand = fixture.Create<CreateCustomerCommand>();
            yield return new object[] { testCommand, "Surname", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<CreateCustomerCommand>(comp => comp
                .With(x => x.Surname, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(101))}")
                .With(x => x.Name, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(99))}")
                .With(x => x.Email, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(99))}"));
            testCommand = fixture.Create<CreateCustomerCommand>();
            yield return new object[] { testCommand, "Surname", "must be 100 characters or fewer" };

            fixture = new Fixture();
            fixture.Customize<CreateCustomerCommand>(comp => comp
                .With(x => x.Address, () => default)
                .With(x => x.Name, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(99))}")
                .With(x => x.Surname, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(99))}")
                .With(x => x.Email, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(99))}"));
            testCommand = fixture.Create<CreateCustomerCommand>();
            yield return new object[] { testCommand, "Address", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<CreateCustomerCommand>(comp => comp
                .With(x => x.Email, () => default)
                .With(x => x.Name, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(99))}")
                .With(x => x.Surname, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(99))}"));
            testCommand = fixture.Create<CreateCustomerCommand>();
            yield return new object[] { testCommand, "Email", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<CreateCustomerCommand>(comp => comp
                .With(x => x.Email, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(101))}")
                .With(x => x.Name, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(99))}")
                .With(x => x.Surname, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(99))}"));
            testCommand = fixture.Create<CreateCustomerCommand>();
            yield return new object[] { testCommand, "Email", "must be 100 characters or fewer" };
        }

        [Theory]
        [MemberData(nameof(GetFailedResultTestData))]
        public async Task Validate_WithInvalidCommand_FailsValidation(
            CreateCustomerCommand testCommand,
            string expectedPropertyName,
            string expectedPhrase)
        {
            // Arrange
            var validator = GetValidationBehaviour();
            var expectedId = new Fixture().Create<System.Guid>();
            // Act
            var act = async () => await validator.Handle(testCommand, () => Task.FromResult(expectedId), CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<ValidationException>().Result
                .Which.Errors.Should().Contain(x => x.PropertyName == expectedPropertyName && x.ErrorMessage.Contains(expectedPhrase));
        }

        private ValidationBehaviour<CreateCustomerCommand, System.Guid> GetValidationBehaviour()
        {
            var validatorProvider = Substitute.For<IValidatorProvider>();
            validatorProvider.GetValidator<CreateCustomerAddressDto>().Returns(c => new CreateCustomerAddressDtoValidator());
            return new ValidationBehaviour<CreateCustomerCommand, System.Guid>(new[] { new CreateCustomerCommandValidator(validatorProvider) });
        }
    }
}