using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Privileges.GetPrivileges
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetPrivilegesQueryValidator : AbstractValidator<GetPrivilegesQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public GetPrivilegesQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}