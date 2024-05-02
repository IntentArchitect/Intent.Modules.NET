using System.Collections.Generic;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db1Entities.GetDb1Entities
{
    public class GetDb1EntitiesQuery : IRequest<List<Db1EntityDto>>, IQuery
    {
        public GetDb1EntitiesQuery()
        {
        }
    }
}