using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Parents.GetParentById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetParentByIdQueryHandler : IRequestHandler<GetParentByIdQuery, ParentDto>
    {
        private readonly IParentRepository _parentRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetParentByIdQueryHandler(IParentRepository parentRepository, IMapper mapper)
        {
            _parentRepository = parentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ParentDto> Handle(GetParentByIdQuery request, CancellationToken cancellationToken)
        {
            var parent = await _parentRepository.FindByIdAsync(request.Id, cancellationToken);
            if (parent is null)
            {
                throw new NotFoundException($"Could not find Parent '{request.Id}'");
            }
            return parent.MapToParentDto(_mapper);
        }
    }
}