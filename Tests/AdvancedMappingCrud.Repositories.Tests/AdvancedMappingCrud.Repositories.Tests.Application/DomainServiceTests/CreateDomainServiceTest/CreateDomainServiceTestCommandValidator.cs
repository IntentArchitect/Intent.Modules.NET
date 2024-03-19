using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.DomainServiceTests.CreateDomainServiceTest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateDomainServiceTestCommandValidator : AbstractValidator<CreateDomainServiceTestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDomainServiceTestCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}