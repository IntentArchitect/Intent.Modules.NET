using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.DeleteOneToManySourceOneToManyDest
{
    public class DeleteOneToManySourceOneToManyDestCommandValidator : AbstractValidator<DeleteOneToManySourceOneToManyDestCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public DeleteOneToManySourceOneToManyDestCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}