using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.PrimaryKeyLookup;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.ComponentTypes.UpdateComponentTypeProperty
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateComponentTypePropertyCommandHandler : IRequestHandler<UpdateComponentTypePropertyCommand>
    {
        private readonly IComponentTypeRepository _componentTypeRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateComponentTypePropertyCommandHandler(IComponentTypeRepository componentTypeRepository)
        {
            _componentTypeRepository = componentTypeRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateComponentTypePropertyCommand request, CancellationToken cancellationToken)
        {
            var componentType = await _componentTypeRepository.FindByIdAsync(request.ComponentTypeId, cancellationToken);
            if (componentType is null)
            {
                throw new NotFoundException($"Could not find ComponentType '{request.ComponentTypeId}'");
            }

            var componentPropertyGroup = componentType.ComponentPropertyGroups.SingleOrDefault(x => x.PropertyGroupId == request.PropertyGroupId);
            if (componentPropertyGroup is null)
            {
                throw new NotFoundException($"Could not find ComponentPropertyGroup '{request.PropertyGroupId}'");
            }

            var componentTypeProperty = componentPropertyGroup.ComponentTypeProperties.FirstOrDefault(x => x.PropertyId == request.PropertyId);
            if (componentTypeProperty is null)
            {
                throw new NotFoundException($"Could not find ComponentTypeProperty '{request.PropertyId}'");
            }

            componentTypeProperty.PropertyName = request.PropertyName;
            componentTypeProperty.ComponentPropertyGroupPropertyGroupId = request.PropertyGroupId;
        }
    }
}