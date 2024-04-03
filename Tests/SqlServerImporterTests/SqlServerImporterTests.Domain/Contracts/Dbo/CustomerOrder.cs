using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace SqlServerImporterTests.Domain.Contracts.Dbo
{
    public record CustomerOrder
    {
        public CustomerOrder(DateTime orderDate, string refNo)
        {
            OrderDate = orderDate;
            RefNo = refNo;
        }

        public DateTime OrderDate { get; init; }
        public string RefNo { get; init; }
    }
}