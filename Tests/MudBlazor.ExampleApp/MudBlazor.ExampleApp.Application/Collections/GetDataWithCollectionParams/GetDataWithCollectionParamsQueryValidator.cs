using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace MudBlazor.ExampleApp.Application.Collections.GetDataWithCollectionParams
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDataWithCollectionParamsQueryValidator : AbstractValidator<GetDataWithCollectionParamsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDataWithCollectionParamsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.IntCollection)
                .NotNull();

            RuleFor(v => v.StringCollection)
                .NotNull();
        }
    }
}