using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Children.GetChildren
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetChildrenQueryValidator : AbstractValidator<GetChildrenQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetChildrenQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}