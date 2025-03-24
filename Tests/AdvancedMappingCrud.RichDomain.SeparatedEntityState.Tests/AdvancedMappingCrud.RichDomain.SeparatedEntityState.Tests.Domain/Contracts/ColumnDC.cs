using System.ComponentModel.DataAnnotations.Schema;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Contracts
{
    public record ColumnDC
    {
        public ColumnDC(string customColumn)
        {
            CustomColumn = customColumn;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected ColumnDC()
        {
            CustomColumn = null!;
        }

        [Column("CUSTOM_COLUMN")]
        public string CustomColumn { get; init; }
    }
}