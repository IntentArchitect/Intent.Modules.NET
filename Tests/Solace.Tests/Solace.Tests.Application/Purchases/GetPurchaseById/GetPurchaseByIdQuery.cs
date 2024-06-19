using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Solace.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Solace.Tests.Application.Purchases.GetPurchaseById
{
    public class GetPurchaseByIdQuery : IRequest<PurchaseDto>, IQuery
    {
        public GetPurchaseByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}