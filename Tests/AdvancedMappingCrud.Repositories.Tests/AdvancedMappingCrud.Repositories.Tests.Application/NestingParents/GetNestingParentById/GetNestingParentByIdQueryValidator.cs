using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.NestingParents.GetNestingParentById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetNestingParentByIdQueryValidator : AbstractValidator<GetNestingParentByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetNestingParentByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}