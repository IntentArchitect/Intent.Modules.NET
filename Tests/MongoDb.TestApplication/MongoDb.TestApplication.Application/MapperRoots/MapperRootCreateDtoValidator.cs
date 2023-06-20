using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MapperRoots
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MapperRootCreateDtoValidator : AbstractValidator<MapperRootCreateDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public MapperRootCreateDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.No)
                .NotNull();

            RuleFor(v => v.MapAggChildrenIds)
                .NotNull();

            RuleFor(v => v.MapAggPeerId)
                .NotNull();
        }
    }
}