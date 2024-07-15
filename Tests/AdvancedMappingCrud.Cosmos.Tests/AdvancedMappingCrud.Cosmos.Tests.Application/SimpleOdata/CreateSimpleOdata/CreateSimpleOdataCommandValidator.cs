using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.SimpleOdata.CreateSimpleOdata
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateSimpleOdataCommandValidator : AbstractValidator<CreateSimpleOdataCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateSimpleOdataCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Surname)
                .NotNull();
        }
    }
}