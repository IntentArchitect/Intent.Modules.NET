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

namespace IntegrationTesting.Tests.Application.Children.GetChildById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetChildByIdQueryHandler : IRequestHandler<GetChildByIdQuery, ChildDto>
    {
        private readonly IChildRepository _childRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetChildByIdQueryHandler(IChildRepository childRepository, IMapper mapper)
        {
            _childRepository = childRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ChildDto> Handle(GetChildByIdQuery request, CancellationToken cancellationToken)
        {
            var child = await _childRepository.FindByIdAsync(request.Id, cancellationToken);
            if (child is null)
            {
                throw new NotFoundException($"Could not find Child '{request.Id}'");
            }
            return child.MapToChildDto(_mapper);
        }
    }
}