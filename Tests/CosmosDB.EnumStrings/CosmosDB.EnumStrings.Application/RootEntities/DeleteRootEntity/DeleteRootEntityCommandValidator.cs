using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities.DeleteRootEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteRootEntityCommandValidator : AbstractValidator<DeleteRootEntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteRootEntityCommandValidator()
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