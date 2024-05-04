using System.Collections.Generic;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAppDefaults.GetEntityAppDefaults
{
    public class GetEntityAppDefaultsQuery : IRequest<List<EntityAppDefaultDto>>, IQuery
    {
        public GetEntityAppDefaultsQuery()
        {
        }
    }
}