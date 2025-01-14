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

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.GetCheckNewCompChildCrudById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCheckNewCompChildCrudByIdQueryHandler : IRequestHandler<GetCheckNewCompChildCrudByIdQuery, CheckNewCompChildCrudDto>
    {
        private readonly ICheckNewCompChildCrudRepository _checkNewCompChildCrudRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCheckNewCompChildCrudByIdQueryHandler(ICheckNewCompChildCrudRepository checkNewCompChildCrudRepository,
            IMapper mapper)
        {
            _checkNewCompChildCrudRepository = checkNewCompChildCrudRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CheckNewCompChildCrudDto> Handle(
            GetCheckNewCompChildCrudByIdQuery request,
            CancellationToken cancellationToken)
        {
            var checkNewCompChildCrud = await _checkNewCompChildCrudRepository.FindByIdAsync(request.Id, cancellationToken);
            if (checkNewCompChildCrud is null)
            {
                throw new NotFoundException($"Could not find CheckNewCompChildCrud '{request.Id}'");
            }
            return checkNewCompChildCrud.MapToCheckNewCompChildCrudDto(_mapper);
        }
    }
}