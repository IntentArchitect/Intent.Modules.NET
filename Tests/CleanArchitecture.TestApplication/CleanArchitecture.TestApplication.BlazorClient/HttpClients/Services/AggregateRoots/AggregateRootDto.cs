using System.ComponentModel.DataAnnotations;
using CleanArchitecture.TestApplication.BlazorClient.HttpClients.Domain.Enums;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.Services.AggregateRoots
{
    public class AggregateRootDto
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Aggregate attr is required.")]
        public string AggregateAttr { get; set; }
        [Required(ErrorMessage = "Composites is required.")]
        public List<AggregateRootCompositeManyBDto> Composites { get; set; }
        public AggregateRootCompositeSingleADto? Composite { get; set; }
        public AggregateRootAggregateSingleCDto? Aggregate { get; set; }
        [Required(ErrorMessage = "Enum type 1 is required.")]
        public EnumWithoutValues EnumType1 { get; set; }
        [Required(ErrorMessage = "Enum type 2 is required.")]
        public EnumWithDefaultLiteral EnumType2 { get; set; }
        [Required(ErrorMessage = "Enum type 3 is required.")]
        public EnumWithoutDefaultLiteral EnumType3 { get; set; }

        public static AggregateRootDto Create(
            Guid id,
            string aggregateAttr,
            List<AggregateRootCompositeManyBDto> composites,
            AggregateRootCompositeSingleADto? composite,
            AggregateRootAggregateSingleCDto? aggregate,
            EnumWithoutValues enumType1,
            EnumWithDefaultLiteral enumType2,
            EnumWithoutDefaultLiteral enumType3)
        {
            return new AggregateRootDto
            {
                Id = id,
                AggregateAttr = aggregateAttr,
                Composites = composites,
                Composite = composite,
                Aggregate = aggregate,
                EnumType1 = enumType1,
                EnumType2 = enumType2,
                EnumType3 = enumType3
            };
        }
    }
}