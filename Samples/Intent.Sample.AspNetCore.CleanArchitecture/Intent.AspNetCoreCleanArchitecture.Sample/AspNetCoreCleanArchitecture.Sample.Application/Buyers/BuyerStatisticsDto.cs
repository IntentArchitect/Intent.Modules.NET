using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Buyers
{
    public class BuyerStatisticsDto
    {
        public BuyerStatisticsDto()
        {
            BuyerName = null!;
        }

        public Guid BuyerId { get; set; }
        public byte[] BuyerName { get; set; }
        public int NoOfOrders { get; set; }
        public decimal AverageCartValue { get; set; }

        public static BuyerStatisticsDto Create(Guid buyerId, byte[] buyerName, int noOfOrders, decimal averageCartValue)
        {
            return new BuyerStatisticsDto
            {
                BuyerId = buyerId,
                BuyerName = buyerName,
                NoOfOrders = noOfOrders,
                AverageCartValue = averageCartValue
            };
        }
    }
}