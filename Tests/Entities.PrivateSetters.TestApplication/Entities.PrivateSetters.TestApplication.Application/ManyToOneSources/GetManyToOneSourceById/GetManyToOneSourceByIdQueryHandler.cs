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

namespace Entities.PrivateSetters.TestApplication.Application.ManyToOneSources.GetManyToOneSourceById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetManyToOneSourceByIdQueryHandler : IRequestHandler<GetManyToOneSourceByIdQuery, ManyToOneSourceDto>
    {
        private readonly IManyToOneSourceRepository _manyToOneSourceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetManyToOneSourceByIdQueryHandler(IManyToOneSourceRepository manyToOneSourceRepository, IMapper mapper)
        {
            _manyToOneSourceRepository = manyToOneSourceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ManyToOneSourceDto> Handle(
            GetManyToOneSourceByIdQuery request,
            CancellationToken cancellationToken)
        {
            var manyToOneSource = await _manyToOneSourceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (manyToOneSource is null)
            {
                throw new NotFoundException($"Could not find ManyToOneSource '{request.Id}'");
            }

            return manyToOneSource.MapToManyToOneSourceDto(_mapper);
        }
    }
}