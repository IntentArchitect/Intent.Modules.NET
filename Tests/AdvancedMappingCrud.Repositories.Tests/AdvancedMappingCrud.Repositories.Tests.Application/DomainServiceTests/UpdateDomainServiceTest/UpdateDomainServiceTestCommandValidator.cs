using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.DomainServiceTests.UpdateDomainServiceTest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateDomainServiceTestCommandValidator : AbstractValidator<UpdateDomainServiceTestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateDomainServiceTestCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}