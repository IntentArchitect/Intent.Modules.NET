using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using MongoDb.TestApplication.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.MapperRoots
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MapperRootDtoValidator : AbstractValidator<MapperRootDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public MapperRootDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules(IValidatorProvider provider)
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
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<MapAggChildDto>()!));

            RuleFor(v => v.MapMapMe)
                .NotNull()
                .SetValidator(provider.GetValidator<MapMapMeDto>()!);
        }
    }
}