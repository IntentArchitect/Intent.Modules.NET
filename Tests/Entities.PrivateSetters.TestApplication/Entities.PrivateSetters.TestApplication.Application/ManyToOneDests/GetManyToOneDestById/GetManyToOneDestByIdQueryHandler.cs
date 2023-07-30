using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Common.Exceptions;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Aggregational;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToOneDests.GetManyToOneDestById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetManyToOneDestByIdQueryHandler : IRequestHandler<GetManyToOneDestByIdQuery, ManyToOneDestDto>
    {
        private readonly IManyToOneDestRepository _manyToOneDestRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetManyToOneDestByIdQueryHandler(IManyToOneDestRepository manyToOneDestRepository, IMapper mapper)
        {
            _manyToOneDestRepository = manyToOneDestRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ManyToOneDestDto> Handle(GetManyToOneDestByIdQuery request, CancellationToken cancellationToken)
        {
            var manyToOneDest = await _manyToOneDestRepository.FindByIdAsync(request.Id, cancellationToken);
            if (manyToOneDest is null)
            {
                throw new NotFoundException($"Could not find ManyToOneDest '{request.Id}'");
            }

            return manyToOneDest.MapToManyToOneDestDto(_mapper);
        }
    }
}