using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Basics.DeleteBasic
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
            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}