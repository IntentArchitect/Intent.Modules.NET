using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.BasicOrderBies.UpdateBasicOrderBy
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateBasicOrderByCommandValidator : AbstractValidator<UpdateBasicOrderByCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateBasicOrderByCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Surname)
                .NotNull();

            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}