using Intent.RoslynWeaver.Attributes;
using MediatR;
using Redis.Om.Repositories.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Redis.Om.Repositories.Application.DerivedTypes.GetDerivedTypeById
{
    public class GetDerivedTypeByIdQuery : IRequest<DerivedTypeDto>, IQuery
    {
        public GetDerivedTypeByIdQuery(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}