using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace MudBlazor.ExampleApp.Application.Collections.GetDataSingleCollection
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDataSingleCollectionQueryValidator : AbstractValidator<GetDataSingleCollectionQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDataSingleCollectionQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.IntCollection)
                .NotNull();

            RuleFor(v => v.StringValue)
                .NotNull();
        }
    }
}