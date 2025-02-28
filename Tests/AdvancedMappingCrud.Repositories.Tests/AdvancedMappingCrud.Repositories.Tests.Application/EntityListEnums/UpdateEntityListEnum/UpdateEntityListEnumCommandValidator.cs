using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.EntityListEnums.UpdateEntityListEnum
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateEntityListEnumCommandValidator : AbstractValidator<UpdateEntityListEnumCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateEntityListEnumCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.OrderStatuses)
                .NotNull()
                .ForEach(x => x.IsInEnum());
        }
    }
}