using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.PrimaryKeyLookup;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ComponentTypes.GetComponentTypePropertyById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetComponentTypePropertyByIdQueryHandler : IRequestHandler<GetComponentTypePropertyByIdQuery, ComponentTypePropertyDto>
    {
        private readonly IComponentTypeRepository _componentTypeRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetComponentTypePropertyByIdQueryHandler(IComponentTypeRepository componentTypeRepository, IMapper mapper)
        {
            _componentTypeRepository = componentTypeRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ComponentTypePropertyDto> Handle(
            GetComponentTypePropertyByIdQuery request,
            CancellationToken cancellationToken)
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
            return componentTypeProperty.MapToComponentTypePropertyDto(_mapper);
        }
    }
}