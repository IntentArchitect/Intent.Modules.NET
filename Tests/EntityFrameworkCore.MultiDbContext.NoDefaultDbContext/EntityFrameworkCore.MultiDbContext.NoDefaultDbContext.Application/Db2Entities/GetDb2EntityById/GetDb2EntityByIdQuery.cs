using System;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db2Entities.GetDb2EntityById
{
    public class GetDb2EntityByIdQuery : IRequest<Db2EntityDto>, IQuery
    {
        public GetDb2EntityByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}