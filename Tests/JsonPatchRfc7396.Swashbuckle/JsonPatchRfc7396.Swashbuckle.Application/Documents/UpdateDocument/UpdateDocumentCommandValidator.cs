using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents.UpdateDocument
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateDocumentCommandValidator : AbstractValidator<UpdateDocumentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateDocumentCommandValidator(IValidatorProvider provider)
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
                .ForEach(x => x.SetValidator(provider.GetValidator<UpdateDocumentCommandChangesDto>()!));

            RuleFor(v => v.Permissions)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<UpdateDocumentCommandPermissionsDto>()!));

            RuleFor(v => v.Revisions)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<UpdateDocumentCommandRevisionsDto>()!));

            RuleFor(v => v.Sessions)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<UpdateDocumentCommandSessionsDto>()!));
        }
    }
}