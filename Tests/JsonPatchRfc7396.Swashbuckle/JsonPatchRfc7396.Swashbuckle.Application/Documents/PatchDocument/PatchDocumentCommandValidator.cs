using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents.PatchDocument
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PatchDocumentCommandValidator : AbstractValidator<PatchDocumentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public PatchDocumentCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.Status)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.Title)
                .NotNull()
                .SetValidator(provider.GetValidator<UpdateDocumentTitleDto>()!);

            RuleFor(v => v.Content)
                .NotNull()
                .SetValidator(provider.GetValidator<UpdateDocumentContentDto>()!);

            RuleFor(v => v.Changes)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<PatchDocumentCommandChangesDto>()!));

            RuleFor(v => v.Permissions)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<PatchDocumentCommandPermissionsDto>()!));

            RuleFor(v => v.Revisions)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<PatchDocumentCommandRevisionsDto>()!));

            RuleFor(v => v.Sessions)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<PatchDocumentCommandSessionsDto>()!));
        }
    }
}