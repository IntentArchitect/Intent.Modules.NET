using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.ExplicitETags.GetExplicitETagById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetExplicitETagByIdQueryValidator : AbstractValidator<GetExplicitETagByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetExplicitETagByIdQueryValidator()
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