using System;
using EntityFrameworkCore.Repositories.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs.GetEntityName
{
    public class GetEntityName : IRequest<string>, IQuery
    {
        public GetEntityName(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}