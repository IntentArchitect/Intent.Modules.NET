using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Scalar.Application.Documents.CreateDocument
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateDocumentCommandValidator : AbstractValidator<CreateDocumentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDocumentCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Status)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.Title)
                .NotNull()
                .SetValidator(provider.GetValidator<CreateDocumentTitleDto>()!);

            RuleFor(v => v.Content)
                .NotNull()
                .SetValidator(provider.GetValidator<CreateDocumentContentDto>()!);

            RuleFor(v => v.Changes)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateDocumentCommandChangesDto>()!));

            RuleFor(v => v.Permissions)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateDocumentCommandPermissionsDto>()!));

            RuleFor(v => v.Revisions)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateDocumentCommandRevisionsDto>()!));

            RuleFor(v => v.Sessions)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateDocumentCommandSessionsDto>()!));
        }
    }
}