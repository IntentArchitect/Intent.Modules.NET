using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MapperRoots
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MapAggChildDtoValidator : AbstractValidator<MapAggChildDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public MapAggChildDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.ChildName)
                .NotNull();

            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}