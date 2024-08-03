using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.Geometry;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.GeometryTypes.GetGeometryTypeById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetGeometryTypeByIdQueryHandler : IRequestHandler<GetGeometryTypeByIdQuery, GeometryTypeDto>
    {
        private readonly IGeometryTypeRepository _geometryTypeRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetGeometryTypeByIdQueryHandler(IGeometryTypeRepository geometryTypeRepository, IMapper mapper)
        {
            _geometryTypeRepository = geometryTypeRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<GeometryTypeDto> Handle(GetGeometryTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var geometryType = await _geometryTypeRepository.FindByIdAsync(request.Id, cancellationToken);
            if (geometryType is null)
            {
                throw new NotFoundException($"Could not find GeometryType '{request.Id}'");
            }
            return geometryType.MapToGeometryTypeDto(_mapper);
        }
    }
}