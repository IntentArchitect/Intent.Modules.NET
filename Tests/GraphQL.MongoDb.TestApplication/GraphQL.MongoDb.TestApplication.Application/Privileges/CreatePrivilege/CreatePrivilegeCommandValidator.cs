using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Privileges.CreatePrivilege
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreatePrivilegeCommandValidator : AbstractValidator<CreatePrivilegeCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreatePrivilegeCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}