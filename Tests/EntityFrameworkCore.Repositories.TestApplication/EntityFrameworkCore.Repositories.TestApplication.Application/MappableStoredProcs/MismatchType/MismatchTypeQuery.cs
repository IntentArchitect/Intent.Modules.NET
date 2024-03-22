using System;
using EntityFrameworkCore.Repositories.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs.MismatchType
{
    public class MismatchTypeQuery : IRequest<int>, IQuery
    {
        public MismatchTypeQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}