using System;
using System.Collections.Generic;
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

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.GetCNCCChildren
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCNCCChildrenQueryHandler : IRequestHandler<GetCNCCChildrenQuery, List<CNCCChildDto>>
    {
        private readonly ICheckNewCompChildCrudRepository _checkNewCompChildCrudRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCNCCChildrenQueryHandler(ICheckNewCompChildCrudRepository checkNewCompChildCrudRepository,
            IMapper mapper)
        {
            _checkNewCompChildCrudRepository = checkNewCompChildCrudRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CNCCChildDto>> Handle(GetCNCCChildrenQuery request, CancellationToken cancellationToken)
        {
            var checkNewCompChildCrud = await _checkNewCompChildCrudRepository.FindByIdAsync(request.CheckNewCompChildCrudId, cancellationToken);
            if (checkNewCompChildCrud is null)
            {
                throw new NotFoundException($"Could not find CheckNewCompChildCrud '{request.CheckNewCompChildCrudId}'");
            }

            var cNCCChildren = checkNewCompChildCrud.CNCCChildren.Where(x => x.CheckNewCompChildCrudId == request.CheckNewCompChildCrudId);
            return cNCCChildren.MapToCNCCChildDtoList(_mapper);
        }
    }
}