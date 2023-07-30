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

namespace Entities.PrivateSetters.TestApplication.Application.ManyToManySources.GetManyToManySourceById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetManyToManySourceByIdQueryHandler : IRequestHandler<GetManyToManySourceByIdQuery, ManyToManySourceDto>
    {
        private readonly IManyToManySourceRepository _manyToManySourceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetManyToManySourceByIdQueryHandler(IManyToManySourceRepository manyToManySourceRepository, IMapper mapper)
        {
            _manyToManySourceRepository = manyToManySourceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ManyToManySourceDto> Handle(
            GetManyToManySourceByIdQuery request,
            CancellationToken cancellationToken)
        {
            var manyToManySource = await _manyToManySourceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (manyToManySource is null)
            {
                throw new NotFoundException($"Could not find ManyToManySource '{request.Id}'");
            }

            return manyToManySource.MapToManyToManySourceDto(_mapper);
        }
    }
}