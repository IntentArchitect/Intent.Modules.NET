using System;
using Entities.PrivateSetters.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.GetOneToManySourceOneToManyDestById
{
    public class GetOneToManySourceOneToManyDestByIdQuery : IRequest<OneToManySourceOneToManyDestDto>, IQuery
    {
        public GetOneToManySourceOneToManyDestByIdQuery(Guid ownerId, Guid id)
        {
            OwnerId = ownerId;
            Id = id;
        }

        public Guid OwnerId { get; set; }
        public Guid Id { get; set; }
    }
}