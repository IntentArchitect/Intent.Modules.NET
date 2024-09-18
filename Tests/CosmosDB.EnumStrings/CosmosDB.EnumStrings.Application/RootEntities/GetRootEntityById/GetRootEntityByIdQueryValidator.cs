using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities.GetRootEntityById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetRootEntityByIdQueryValidator : AbstractValidator<GetRootEntityByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetRootEntityByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}