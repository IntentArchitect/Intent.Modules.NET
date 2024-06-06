using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Solace.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Solace.Tests.Application.Purchases.GetPurchases
{
    public class GetPurchasesQuery : IRequest<List<PurchaseDto>>, IQuery
    {
        public GetPurchasesQuery()
        {
        }
    }
}