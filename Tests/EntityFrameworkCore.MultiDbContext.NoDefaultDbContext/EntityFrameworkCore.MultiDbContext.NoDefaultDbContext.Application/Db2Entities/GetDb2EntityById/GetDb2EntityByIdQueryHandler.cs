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

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db2Entities.GetDb2EntityById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDb2EntityByIdQueryHandler : IRequestHandler<GetDb2EntityByIdQuery, Db2EntityDto>
    {
        private readonly IDb2EntityRepository _db2EntityRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetDb2EntityByIdQueryHandler(IDb2EntityRepository db2EntityRepository, IMapper mapper)
        {
            _db2EntityRepository = db2EntityRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Db2EntityDto> Handle(GetDb2EntityByIdQuery request, CancellationToken cancellationToken)
        {
            var db2Entity = await _db2EntityRepository.FindByIdAsync(request.Id, cancellationToken);
            if (db2Entity is null)
            {
                throw new NotFoundException($"Could not find Db2Entity '{request.Id}'");
            }
            return db2Entity.MapToDb2EntityDto(_mapper);
        }
    }
}