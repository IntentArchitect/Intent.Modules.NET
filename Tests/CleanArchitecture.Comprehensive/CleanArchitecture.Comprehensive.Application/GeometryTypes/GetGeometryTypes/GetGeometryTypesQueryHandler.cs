using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Repositories.Geometry;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.GeometryTypes.GetGeometryTypes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetGeometryTypesQueryHandler : IRequestHandler<GetGeometryTypesQuery, List<GeometryTypeDto>>
    {
        private readonly IGeometryTypeRepository _geometryTypeRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetGeometryTypesQueryHandler(IGeometryTypeRepository geometryTypeRepository, IMapper mapper)
        {
            _geometryTypeRepository = geometryTypeRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<GeometryTypeDto>> Handle(GetGeometryTypesQuery request, CancellationToken cancellationToken)
        {
            var geometryTypes = await _geometryTypeRepository.FindAllAsync(cancellationToken);
            return geometryTypes.MapToGeometryTypeDtoList(_mapper);
        }
    }
}