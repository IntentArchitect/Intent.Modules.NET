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

namespace CleanArchitecture.Comprehensive.Application.ComponentTypes.DeleteComponentTypeProperty
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteComponentTypePropertyCommandHandler : IRequestHandler<DeleteComponentTypePropertyCommand>
    {
        private readonly IComponentTypeRepository _componentTypeRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteComponentTypePropertyCommandHandler(IComponentTypeRepository componentTypeRepository)
        {
            _componentTypeRepository = componentTypeRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteComponentTypePropertyCommand request, CancellationToken cancellationToken)
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


            componentPropertyGroup.ComponentTypeProperties.Remove(componentTypeProperty);
        }
    }
}