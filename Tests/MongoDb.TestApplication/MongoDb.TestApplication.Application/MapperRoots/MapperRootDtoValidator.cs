using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MapperRoots
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MapperRootDtoValidator : AbstractValidator<MapperRootDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public MapperRootDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.No)
                .NotNull();

            RuleFor(v => v.MapAggChildrenIds)
                .NotNull();

            RuleFor(v => v.MapAggPeerId)
                .NotNull();

            RuleFor(v => v.CompChildAtt)
                .NotNull();

            RuleFor(v => v.CompChildAggAtt)
                .NotNull();

            RuleFor(v => v.PeerAtt)
                .NotNull();

            RuleFor(v => v.MapAggPeerAggId)
                .NotNull();

            RuleFor(v => v.PeerCompChildAtt)
                .NotNull();

            RuleFor(v => v.MapPeerCompChildAggAtt)
                .NotNull();

            RuleFor(v => v.MapAggPeerAggAtt)
                .NotNull();

            RuleFor(v => v.MapAggPeerAggMoreAtt)
                .NotNull();

            RuleFor(v => v.MapAggChildren)
                .NotNull();

            RuleFor(v => v.MapMapMe)
                .NotNull();
        }
    }
}