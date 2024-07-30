using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Basics.GetBasicsNullable
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetBasicsNullableQueryValidator : AbstractValidator<GetBasicsNullableQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetBasicsNullableQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}