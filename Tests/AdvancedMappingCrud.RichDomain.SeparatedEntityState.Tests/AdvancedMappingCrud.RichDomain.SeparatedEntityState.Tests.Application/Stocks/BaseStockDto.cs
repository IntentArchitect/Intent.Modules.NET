using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Stocks
{
    public class BaseStockDto : TimeDto
    {
        public BaseStockDto()
        {
            User = null!;
        }

        public Guid Id { get; set; }
        public string User { get; set; }

        public static BaseStockDto Create(DateTime currentDateTime, Guid id, string user)
        {
            return new BaseStockDto
            {
                CurrentDateTime = currentDateTime,
                Id = id,
                User = user
            };
        }
    }
}