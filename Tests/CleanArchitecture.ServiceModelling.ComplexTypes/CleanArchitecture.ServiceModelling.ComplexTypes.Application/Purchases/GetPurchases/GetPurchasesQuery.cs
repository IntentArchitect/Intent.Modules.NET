using System.Collections.Generic;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases.GetPurchases
{
    public class GetPurchasesQuery : IRequest<List<PurchaseDto>>, IQuery
    {
        public GetPurchasesQuery()
        {
        }
    }
}