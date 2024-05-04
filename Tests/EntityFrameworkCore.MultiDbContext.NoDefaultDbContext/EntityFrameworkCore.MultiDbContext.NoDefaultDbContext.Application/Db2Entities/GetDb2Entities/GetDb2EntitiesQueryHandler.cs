using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db2Entities.GetDb2Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDb2EntitiesQueryHandler : IRequestHandler<GetDb2EntitiesQuery, List<Db2EntityDto>>
    {
        private readonly IDb2EntityRepository _db2EntityRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetDb2EntitiesQueryHandler(IDb2EntityRepository db2EntityRepository, IMapper mapper)
        {
            _db2EntityRepository = db2EntityRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<Db2EntityDto>> Handle(GetDb2EntitiesQuery request, CancellationToken cancellationToken)
        {
            var db2Entities = await _db2EntityRepository.FindAllAsync(cancellationToken);
            return db2Entities.MapToDb2EntityDtoList(_mapper);
        }
    }
}