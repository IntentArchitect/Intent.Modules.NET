using System.ComponentModel.DataAnnotations.Schema;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Contracts
{
    public record ColumnDC
    {
        public ColumnDC(string customColumn)
        {
            CustomColumn = customColumn;
        }

        [Column("CUSTOM_COLUMN")]
        public string CustomColumn { get; init; }
    }
}