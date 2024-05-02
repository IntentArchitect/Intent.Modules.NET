using System;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAlternates.GetEntityAlternateById
{
    public class GetEntityAlternateByIdQuery : IRequest<EntityAlternateDto>, IQuery
    {
        public GetEntityAlternateByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}