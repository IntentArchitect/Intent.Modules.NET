using System;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases.GetPurchaseById
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