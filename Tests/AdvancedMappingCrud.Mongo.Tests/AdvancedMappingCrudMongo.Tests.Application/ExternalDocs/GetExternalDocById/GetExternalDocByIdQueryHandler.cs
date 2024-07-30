using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrudMongo.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.ExternalDocs.GetExternalDocById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetExternalDocByIdQueryHandler : IRequestHandler<GetExternalDocByIdQuery, ExternalDocDto>
    {
        private readonly IExternalDocRepository _externalDocRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetExternalDocByIdQueryHandler(IExternalDocRepository externalDocRepository, IMapper mapper)
        {
            _externalDocRepository = externalDocRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ExternalDocDto> Handle(GetExternalDocByIdQuery request, CancellationToken cancellationToken)
        {
            var externalDoc = await _externalDocRepository.FindByIdAsync(request.Id, cancellationToken);
            if (externalDoc is null)
            {
                throw new NotFoundException($"Could not find ExternalDoc '{request.Id}'");
            }
            return externalDoc.MapToExternalDocDto(_mapper);
        }
    }
}