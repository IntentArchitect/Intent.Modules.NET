using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "1.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.Contracts.Services.AggregateRoots
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAggregateRootCompositeSingleACompositeManyAADtoValidator : AbstractValidator<UpdateAggregateRootCompositeSingleACompositeManyAADto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public UpdateAggregateRootCompositeSingleACompositeManyAADtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.CompositeAttr)
                .NotNull();
        }
    }
}