using System;
using FluentValidation;
using GraphQL.MongoDb.TestApplication.Application.Common.Validation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Users.CreateUser
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateUserCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Surname)
                .NotNull();

            RuleFor(v => v.Email)
                .NotNull();

            RuleFor(v => v.AssignedPrivileges)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateUserAssignedPrivilegeDto>()!));
        }
    }
}