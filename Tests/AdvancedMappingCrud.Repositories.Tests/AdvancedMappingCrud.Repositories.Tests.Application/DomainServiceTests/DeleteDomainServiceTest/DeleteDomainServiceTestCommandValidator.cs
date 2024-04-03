using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.DomainServiceTests.DeleteDomainServiceTest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteDomainServiceTestCommandValidator : AbstractValidator<DeleteDomainServiceTestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteDomainServiceTestCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}