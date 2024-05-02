using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Common.Exceptions;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db1Entities.GetDb1EntityById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDb1EntityByIdQueryHandler : IRequestHandler<GetDb1EntityByIdQuery, Db1EntityDto>
    {
        private readonly IDb1EntityRepository _db1EntityRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetDb1EntityByIdQueryHandler(IDb1EntityRepository db1EntityRepository, IMapper mapper)
        {
            _db1EntityRepository = db1EntityRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Db1EntityDto> Handle(GetDb1EntityByIdQuery request, CancellationToken cancellationToken)
        {
            var db1Entity = await _db1EntityRepository.FindByIdAsync(request.Id, cancellationToken);
            if (db1Entity is null)
            {
                throw new NotFoundException($"Could not find Db1Entity '{request.Id}'");
            }
            return db1Entity.MapToDb1EntityDto(_mapper);
        }
    }
}