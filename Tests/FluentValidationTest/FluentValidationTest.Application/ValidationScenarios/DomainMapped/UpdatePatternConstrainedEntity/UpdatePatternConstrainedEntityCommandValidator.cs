using System.Buffers.Text;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.UpdatePatternConstrainedEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdatePatternConstrainedEntityCommandValidator : AbstractValidator<UpdatePatternConstrainedEntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdatePatternConstrainedEntityCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.EmailAddress)
                .NotNull()
                .EmailAddress();

            RuleFor(v => v.WebsiteUrl)
                .Must(value => Uri.TryCreate(value, UriKind.Absolute, out _))
                .WithMessage("WebsiteUrl must be a valid URL.");

            RuleFor(v => v.Slug)
                .NotNull()
                .Matches(@"^[a-z0-9-]+$")
                .WithMessage(@"Slug must contain lowercase letters, digits or hyphens only.");

            RuleFor(v => v.ReferenceNumber)
                .NotNull()
                .Matches(@"^[A-Z]{3}-[0-9]{4}$")
                .WithMessage(@"ReferenceNumber must match AAA-1234 format.");

            RuleFor(v => v.Base64)
                .NotNull()
                .Must(value => Base64.IsValid(value))
                .WithMessage("Base64 must be a valid Base64 string.");
        }
    }
}