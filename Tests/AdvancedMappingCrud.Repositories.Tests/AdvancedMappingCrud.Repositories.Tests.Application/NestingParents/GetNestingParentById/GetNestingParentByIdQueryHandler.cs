using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.MappingTests;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.NestingParents.GetNestingParentById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetNestingParentByIdQueryHandler : IRequestHandler<GetNestingParentByIdQuery, NestingParentDto>
    {
        private readonly INestingParentRepository _nestingParentRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetNestingParentByIdQueryHandler(INestingParentRepository nestingParentRepository, IMapper mapper)
        {
            _nestingParentRepository = nestingParentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<NestingParentDto> Handle(GetNestingParentByIdQuery request, CancellationToken cancellationToken)
        {
            var nestingParent = await _nestingParentRepository.FindByIdAsync(request.Id, cancellationToken);
            if (nestingParent is null)
            {
                throw new NotFoundException($"Could not find NestingParent '{request.Id}'");
            }
            return nestingParent.MapToNestingParentDto(_mapper);
        }
    }
}