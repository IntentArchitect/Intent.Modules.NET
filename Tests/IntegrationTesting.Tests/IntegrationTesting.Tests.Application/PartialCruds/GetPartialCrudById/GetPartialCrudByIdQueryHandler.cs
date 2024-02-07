using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.PartialCruds.GetPartialCrudById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetPartialCrudByIdQueryHandler : IRequestHandler<GetPartialCrudByIdQuery, PartialCrudDto>
    {
        private readonly IPartialCrudRepository _partialCrudRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetPartialCrudByIdQueryHandler(IPartialCrudRepository partialCrudRepository, IMapper mapper)
        {
            _partialCrudRepository = partialCrudRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PartialCrudDto> Handle(GetPartialCrudByIdQuery request, CancellationToken cancellationToken)
        {
            var partialCrud = await _partialCrudRepository.FindByIdAsync(request.Id, cancellationToken);
            if (partialCrud is null)
            {
                throw new NotFoundException($"Could not find PartialCrud '{request.Id}'");
            }
            return partialCrud.MapToPartialCrudDto(_mapper);
        }
    }
}