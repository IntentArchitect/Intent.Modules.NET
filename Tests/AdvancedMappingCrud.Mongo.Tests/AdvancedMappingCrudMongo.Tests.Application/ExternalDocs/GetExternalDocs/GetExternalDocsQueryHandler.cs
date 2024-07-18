using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.ExternalDocs.GetExternalDocs
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetExternalDocsQueryHandler : IRequestHandler<GetExternalDocsQuery, List<ExternalDocDto>>
    {
        private readonly IExternalDocRepository _externalDocRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetExternalDocsQueryHandler(IExternalDocRepository externalDocRepository, IMapper mapper)
        {
            _externalDocRepository = externalDocRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ExternalDocDto>> Handle(GetExternalDocsQuery request, CancellationToken cancellationToken)
        {
            var externalDocs = await _externalDocRepository.FindAllAsync(cancellationToken);
            return externalDocs.MapToExternalDocDtoList(_mapper);
        }
    }
}