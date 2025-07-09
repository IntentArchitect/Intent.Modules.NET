using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Privileges.DeletePrivilege
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeletePrivilegeCommandValidator : AbstractValidator<DeletePrivilegeCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeletePrivilegeCommandValidator()
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