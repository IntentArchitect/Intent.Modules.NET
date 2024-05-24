using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.NestingParents.GetNestingParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetNestingParentsQueryValidator : AbstractValidator<GetNestingParentsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetNestingParentsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}