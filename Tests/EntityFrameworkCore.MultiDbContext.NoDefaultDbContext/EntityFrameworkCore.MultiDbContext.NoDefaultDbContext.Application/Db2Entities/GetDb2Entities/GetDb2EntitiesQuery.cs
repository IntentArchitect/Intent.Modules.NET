using System.Collections.Generic;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db2Entities.GetDb2Entities
{
    public class GetDb2EntitiesQuery : IRequest<List<Db2EntityDto>>, IQuery
    {
        public GetDb2EntitiesQuery()
        {
        }
    }
}