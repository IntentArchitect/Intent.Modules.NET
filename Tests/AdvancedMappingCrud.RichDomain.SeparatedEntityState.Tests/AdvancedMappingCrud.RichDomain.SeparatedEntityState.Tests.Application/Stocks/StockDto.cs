using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Stocks
{
    public class StockDto : BaseStockDto, IMapFrom<Stock>
    {
        public StockDto()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public int Total { get; set; }

        public static StockDto Create(string name, int total)
        {
            return new StockDto
            {
                Name = name,
                Total = total
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Stock, StockDto>();
        }
    }
}