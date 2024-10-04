using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Application.Common.Pagination;
using Ardalis.Domain.Repositories;
using Ardalis.Domain.Specifications;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Ardalis.Application.Clients.GetClientsPaginated
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetClientsPaginatedQueryHandler : IRequestHandler<GetClientsPaginatedQuery, PagedResult<ClientDto>>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetClientsPaginatedQueryHandler(IClientRepository clientRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<ClientDto>> Handle(
            GetClientsPaginatedQuery request,
            CancellationToken cancellationToken)
        {
            var clients = await _clientRepository.FindAllAsync(new ClientSpec(), request.PageNo, request.PageSize, cancellationToken);
            return clients.MapToPagedResult(x => x.MapToClientDto(_mapper));
        }
    }
}