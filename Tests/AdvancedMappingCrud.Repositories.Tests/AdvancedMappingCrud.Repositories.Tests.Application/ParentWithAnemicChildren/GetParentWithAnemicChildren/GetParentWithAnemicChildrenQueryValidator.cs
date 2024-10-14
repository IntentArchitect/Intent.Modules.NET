using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren.GetParentWithAnemicChildren
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetParentWithAnemicChildrenQueryValidator : AbstractValidator<GetParentWithAnemicChildrenQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetParentWithAnemicChildrenQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}