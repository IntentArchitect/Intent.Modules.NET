using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Supers.DeleteSuper
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteSuperCommandValidator : AbstractValidator<DeleteSuperCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteSuperCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}