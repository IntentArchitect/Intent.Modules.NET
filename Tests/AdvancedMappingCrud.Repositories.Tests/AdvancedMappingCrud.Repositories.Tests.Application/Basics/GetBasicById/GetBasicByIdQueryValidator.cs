using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Basics.GetBasicById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetBasicByIdQueryValidator : AbstractValidator<GetBasicByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetBasicByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}