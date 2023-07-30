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

namespace Entities.PrivateSetters.TestApplication.Application.ManyToManyDests.GetManyToManyDestById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetManyToManyDestByIdQueryHandler : IRequestHandler<GetManyToManyDestByIdQuery, ManyToManyDestDto>
    {
        private readonly IManyToManyDestRepository _manyToManyDestRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetManyToManyDestByIdQueryHandler(IManyToManyDestRepository manyToManyDestRepository, IMapper mapper)
        {
            _manyToManyDestRepository = manyToManyDestRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ManyToManyDestDto> Handle(
            GetManyToManyDestByIdQuery request,
            CancellationToken cancellationToken)
        {
            var manyToManyDest = await _manyToManyDestRepository.FindByIdAsync(request.Id, cancellationToken);
            if (manyToManyDest is null)
            {
                throw new NotFoundException($"Could not find ManyToManyDest '{request.Id}'");
            }

            return manyToManyDest.MapToManyToManyDestDto(_mapper);
        }
    }
}