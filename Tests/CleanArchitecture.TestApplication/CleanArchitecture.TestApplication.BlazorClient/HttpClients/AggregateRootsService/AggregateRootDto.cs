using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.AggregateRootsService
{
    public class AggregateRootDto
    {
        public Guid Id { get; set; }
        public string AggregateAttr { get; set; }
        public List<AggregateRootCompositeManyBDto> Composites { get; set; }
        public AggregateRootCompositeSingleADto Composite { get; set; }
        public AggregateRootAggregateSingleCDto Aggregate { get; set; }
        public EnumWithoutValues EnumType1 { get; set; }
        public EnumWithDefaultLiteral EnumType2 { get; set; }
        public EnumWithoutDefaultLiteral EnumType3 { get; set; }

        public static AggregateRootDto Create(
            Guid id,
            string aggregateAttr,
            List<AggregateRootCompositeManyBDto> composites,
            AggregateRootCompositeSingleADto composite,
            AggregateRootAggregateSingleCDto aggregate,
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