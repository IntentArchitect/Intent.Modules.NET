using System.Collections.Generic;
using AspNetCore.Controllers.Secured.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AspNetCore.Controllers.Secured.Application.Buyers.GetBuyers
{
    public class GetBuyersQuery : IRequest<List<BuyerDto>>, IQuery
    {
        public GetBuyersQuery()
        {
        }
    }
}