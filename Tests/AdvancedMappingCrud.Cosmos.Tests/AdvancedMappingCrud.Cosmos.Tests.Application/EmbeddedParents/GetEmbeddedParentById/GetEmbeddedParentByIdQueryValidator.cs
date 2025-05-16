using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.EmbeddedParents.GetEmbeddedParentById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetEmbeddedParentByIdQueryValidator : AbstractValidator<GetEmbeddedParentByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetEmbeddedParentByIdQueryValidator()
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