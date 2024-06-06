using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.DeleteAggregateWithUniqueConstraintIndexElement
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteAggregateWithUniqueConstraintIndexElementCommandValidator : AbstractValidator<DeleteAggregateWithUniqueConstraintIndexElementCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteAggregateWithUniqueConstraintIndexElementCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}