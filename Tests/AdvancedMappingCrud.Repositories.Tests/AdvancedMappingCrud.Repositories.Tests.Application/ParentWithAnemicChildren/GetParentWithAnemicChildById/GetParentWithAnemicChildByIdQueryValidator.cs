using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren.GetParentWithAnemicChildById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetParentWithAnemicChildByIdQueryValidator : AbstractValidator<GetParentWithAnemicChildByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetParentWithAnemicChildByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}