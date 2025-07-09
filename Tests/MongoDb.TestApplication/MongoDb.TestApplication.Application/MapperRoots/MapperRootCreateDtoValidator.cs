using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.MapperRoots
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MapperRootCreateDtoValidator : AbstractValidator<MapperRootCreateDto>
    {
        [IntentManaged(Mode.Merge)]
        public MapperRootCreateDtoValidator()
        {
            ConfigureValidationRules();
        }

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