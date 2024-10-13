using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.AnemicChild;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren.GetParentWithAnemicChildById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetParentWithAnemicChildByIdQueryHandler : IRequestHandler<GetParentWithAnemicChildByIdQuery, ParentWithAnemicChildDto>
    {
        private readonly IParentWithAnemicChildRepository _parentWithAnemicChildRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetParentWithAnemicChildByIdQueryHandler(IParentWithAnemicChildRepository parentWithAnemicChildRepository,
            IMapper mapper)
        {
            _parentWithAnemicChildRepository = parentWithAnemicChildRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ParentWithAnemicChildDto> Handle(
            GetParentWithAnemicChildByIdQuery request,
            CancellationToken cancellationToken)
        {
            var parentWithAnemicChild = await _parentWithAnemicChildRepository.FindByIdAsync(request.Id, cancellationToken);
            if (parentWithAnemicChild is null)
            {
                throw new NotFoundException($"Could not find ParentWithAnemicChild '{request.Id}'");
            }
            return parentWithAnemicChild.MapToParentWithAnemicChildDto(_mapper);
        }
    }
}