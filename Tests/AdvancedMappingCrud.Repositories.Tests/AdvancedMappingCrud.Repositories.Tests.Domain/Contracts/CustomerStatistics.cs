using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Contracts
{
    public record CustomerStatistics
    {
        public CustomerStatistics(int noOfOrders, decimal averageCartValue)
        {
            NoOfOrders = noOfOrders;
            AverageCartValue = averageCartValue;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected CustomerStatistics()
        {
        }

        public int NoOfOrders { get; init; }
        public decimal AverageCartValue { get; init; }
    }
}