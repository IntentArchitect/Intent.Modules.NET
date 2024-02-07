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

namespace IntegrationTesting.Tests.Application.Parents.GetParentById
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