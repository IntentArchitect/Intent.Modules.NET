using CleanArchitecture.Comprehensive.BlazorClient.Client.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.AggregateRoots
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateAggregateRootCompositeSingleADtoValidator : AbstractValidator<CreateAggregateRootCompositeSingleADto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateAggregateRootCompositeSingleADtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.CompositeAttr)
                .NotNull();

            RuleFor(v => v.Composite)
                .SetValidator(provider.GetValidator<CreateAggregateRootCompositeSingleACompositeSingleAADto>()!);

            RuleFor(v => v.Composites)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateAggregateRootCompositeSingleACompositeManyAADto>()!));
        }
    }
}