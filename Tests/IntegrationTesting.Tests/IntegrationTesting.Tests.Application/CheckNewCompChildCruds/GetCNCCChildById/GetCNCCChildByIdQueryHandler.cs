using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.GetCNCCChildById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCNCCChildByIdQueryHandler : IRequestHandler<GetCNCCChildByIdQuery, CNCCChildDto>
    {
        private readonly ICheckNewCompChildCrudRepository _checkNewCompChildCrudRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCNCCChildByIdQueryHandler(ICheckNewCompChildCrudRepository checkNewCompChildCrudRepository,
            IMapper mapper)
        {
            _checkNewCompChildCrudRepository = checkNewCompChildCrudRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CNCCChildDto> Handle(GetCNCCChildByIdQuery request, CancellationToken cancellationToken)
        {
            var checkNewCompChildCrud = await _checkNewCompChildCrudRepository.FindByIdAsync(request.CheckNewCompChildCrudId, cancellationToken);
            if (checkNewCompChildCrud is null)
            {
                throw new NotFoundException($"Could not find CheckNewCompChildCrud '{request.CheckNewCompChildCrudId}'");
            }

            var cNCCChild = checkNewCompChildCrud.CNCCChildren.FirstOrDefault(x => x.Id == request.Id && x.CheckNewCompChildCrudId == request.CheckNewCompChildCrudId);
            if (cNCCChild is null)
            {
                throw new NotFoundException($"Could not find CNCCChild '({request.Id}, {request.CheckNewCompChildCrudId})'");
            }
            return cNCCChild.MapToCNCCChildDto(_mapper);
        }
    }
}