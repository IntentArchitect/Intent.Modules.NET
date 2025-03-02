using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.MultiKeyParents.GetMultiKeyParentById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetMultiKeyParentByIdQueryHandler : IRequestHandler<GetMultiKeyParentByIdQuery, MultiKeyParentDto>
    {
        private readonly IMultiKeyParentRepository _multiKeyParentRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetMultiKeyParentByIdQueryHandler(IMultiKeyParentRepository multiKeyParentRepository, IMapper mapper)
        {
            _multiKeyParentRepository = multiKeyParentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<MultiKeyParentDto> Handle(
            GetMultiKeyParentByIdQuery request,
            CancellationToken cancellationToken)
        {
            var multiKeyParent = await _multiKeyParentRepository.FindByIdAsync(request.Id, cancellationToken);
            if (multiKeyParent is null)
            {
                throw new NotFoundException($"Could not find MultiKeyParent '{request.Id}'");
            }
            return multiKeyParent.MapToMultiKeyParentDto(_mapper);
        }
    }
}