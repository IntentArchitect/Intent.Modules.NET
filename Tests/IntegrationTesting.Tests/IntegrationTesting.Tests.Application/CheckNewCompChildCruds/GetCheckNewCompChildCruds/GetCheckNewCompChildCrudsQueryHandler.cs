using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.GetCheckNewCompChildCruds
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCheckNewCompChildCrudsQueryHandler : IRequestHandler<GetCheckNewCompChildCrudsQuery, List<CheckNewCompChildCrudDto>>
    {
        private readonly ICheckNewCompChildCrudRepository _checkNewCompChildCrudRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCheckNewCompChildCrudsQueryHandler(ICheckNewCompChildCrudRepository checkNewCompChildCrudRepository,
            IMapper mapper)
        {
            _checkNewCompChildCrudRepository = checkNewCompChildCrudRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CheckNewCompChildCrudDto>> Handle(
            GetCheckNewCompChildCrudsQuery request,
            CancellationToken cancellationToken)
        {
            var checkNewCompChildCruds = await _checkNewCompChildCrudRepository.FindAllAsync(cancellationToken);
            return checkNewCompChildCruds.MapToCheckNewCompChildCrudDtoList(_mapper);
        }
    }
}