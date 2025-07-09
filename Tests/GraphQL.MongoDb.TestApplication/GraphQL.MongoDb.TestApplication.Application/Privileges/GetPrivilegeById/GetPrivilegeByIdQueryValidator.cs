using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Privileges.GetPrivilegeById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetPrivilegeByIdQueryValidator : AbstractValidator<GetPrivilegeByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetPrivilegeByIdQueryValidator()
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