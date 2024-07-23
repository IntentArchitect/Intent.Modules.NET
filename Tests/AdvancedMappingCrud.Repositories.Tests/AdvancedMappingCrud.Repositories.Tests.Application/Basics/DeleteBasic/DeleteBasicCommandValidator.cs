using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Basics.DeleteBasic
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteBasicCommandValidator : AbstractValidator<DeleteBasicCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteBasicCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}