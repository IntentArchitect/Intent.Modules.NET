using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.ExternalDocs.UpdateExternalDoc
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateExternalDocCommandValidator : AbstractValidator<UpdateExternalDocCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateExternalDocCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Thing)
                .NotNull();
        }
    }
}