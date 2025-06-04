using AspNetCoreCleanArchitecture.Sample.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Buyers.GetBuyerById
{
    public class GetBuyerByIdQuery : IRequest<BuyerDto>, IQuery
    {
        public GetBuyerByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}