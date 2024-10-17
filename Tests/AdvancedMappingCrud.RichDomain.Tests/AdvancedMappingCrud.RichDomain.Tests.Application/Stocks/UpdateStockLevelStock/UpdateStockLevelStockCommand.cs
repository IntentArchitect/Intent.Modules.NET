using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Stocks.UpdateStockLevelStock
{
    public class UpdateStockLevelStockCommand : IRequest, ICommand
    {
        public UpdateStockLevelStockCommand(StockDto data)
        {
            Data = data;
        }

        public StockDto Data { get; set; }
    }
}