using System;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Stocks.GetStockById
{
    public class GetStockByIdQuery : IRequest<StockDto>, IQuery
    {
        public GetStockByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}