using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.GeometryTypes.GetGeometryTypes
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetGeometryTypesQueryValidator : AbstractValidator<GetGeometryTypesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetGeometryTypesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}