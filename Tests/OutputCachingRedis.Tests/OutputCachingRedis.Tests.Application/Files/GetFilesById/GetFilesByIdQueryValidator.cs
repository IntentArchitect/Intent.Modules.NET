using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace OutputCachingRedis.Tests.Application.Files.GetFilesById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetFilesByIdQueryValidator : AbstractValidator<GetFilesByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetFilesByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}