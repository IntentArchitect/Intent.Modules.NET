using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Scalar.Application.Documents.PatchDocumentChange
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PatchDocumentChangeCommandValidator : AbstractValidator<PatchDocumentChangeCommand>
    {
        [IntentManaged(Mode.Merge)]
        public PatchDocumentChangeCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.DocumentId)
                .NotNull();

            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.PatchJson)
                .NotNull();

            RuleFor(v => v.Actor)
                .NotNull()
                .SetValidator(provider.GetValidator<PatchDocumentChangeActorDto>()!);

            RuleFor(v => v.ClientChangeId)
                .NotNull();
        }
    }
}