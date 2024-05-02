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

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db1Entities.GetDb1Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDb1EntitiesQueryHandler : IRequestHandler<GetDb1EntitiesQuery, List<Db1EntityDto>>
    {
        private readonly IDb1EntityRepository _db1EntityRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetDb1EntitiesQueryHandler(IDb1EntityRepository db1EntityRepository, IMapper mapper)
        {
            _db1EntityRepository = db1EntityRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<Db1EntityDto>> Handle(GetDb1EntitiesQuery request, CancellationToken cancellationToken)
        {
            var db1Entities = await _db1EntityRepository.FindAllAsync(cancellationToken);
            return db1Entities.MapToDb1EntityDtoList(_mapper);
        }
    }
}