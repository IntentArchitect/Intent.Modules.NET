using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.SimpleOdata.DeleteSimpleOdata
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteSimpleOdataCommandValidator : AbstractValidator<DeleteSimpleOdataCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteSimpleOdataCommandValidator()
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