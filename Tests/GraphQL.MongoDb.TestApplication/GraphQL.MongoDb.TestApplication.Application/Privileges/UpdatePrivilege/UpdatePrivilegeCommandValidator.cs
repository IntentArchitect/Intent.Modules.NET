using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Privileges.UpdatePrivilege
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdatePrivilegeCommandValidator : AbstractValidator<UpdatePrivilegeCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdatePrivilegeCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}