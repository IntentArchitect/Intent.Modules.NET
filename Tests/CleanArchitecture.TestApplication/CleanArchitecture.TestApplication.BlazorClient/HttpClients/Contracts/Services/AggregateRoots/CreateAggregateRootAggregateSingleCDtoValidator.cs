using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "1.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.Contracts.Services.AggregateRoots
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAggregateRootAggregateSingleCDtoValidator : AbstractValidator<CreateAggregateRootAggregateSingleCDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public CreateAggregateRootAggregateSingleCDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.AggregationAttr)
                .NotNull();
        }
    }
}