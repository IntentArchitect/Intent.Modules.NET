using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.BasicOrderBies.DeleteBasicOrderBy
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteBasicOrderByCommandValidator : AbstractValidator<DeleteBasicOrderByCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteBasicOrderByCommandValidator()
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