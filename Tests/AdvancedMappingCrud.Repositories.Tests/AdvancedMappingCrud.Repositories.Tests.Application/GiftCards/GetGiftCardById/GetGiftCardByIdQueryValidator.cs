using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.GiftCards.GetGiftCardById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetGiftCardByIdQueryValidator : AbstractValidator<GetGiftCardByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetGiftCardByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.CardCode)
                .NotNull();
        }
    }
}