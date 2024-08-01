using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.GeometryTypes.GetGeometryTypeById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetGeometryTypeByIdQueryValidator : AbstractValidator<GetGeometryTypeByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetGeometryTypeByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}