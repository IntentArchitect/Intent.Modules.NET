using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Children.GetChildById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetChildByIdQueryValidator : AbstractValidator<GetChildByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetChildByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}