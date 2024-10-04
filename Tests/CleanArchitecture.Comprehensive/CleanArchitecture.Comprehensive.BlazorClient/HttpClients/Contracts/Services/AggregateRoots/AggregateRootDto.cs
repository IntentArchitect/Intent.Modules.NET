using CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Domain.Enums;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.AggregateRoots
{
    public class AggregateRootDto
    {
        public AggregateRootDto()
        {
            AggregateAttr = null!;
            Composites = [];
            LimitedDomain = null!;
            LimitedService = null!;
        }

        public Guid Id { get; set; }
        public string AggregateAttr { get; set; }
        public List<AggregateRootCompositeManyBDto> Composites { get; set; }
        public AggregateRootCompositeSingleADto? Composite { get; set; }
        public AggregateRootAggregateSingleCDto? Aggregate { get; set; }
        public EnumWithoutValues EnumType1 { get; set; }
        public EnumWithDefaultLiteral EnumType2 { get; set; }
        public EnumWithoutDefaultLiteral EnumType3 { get; set; }
        public string LimitedDomain { get; set; }
        public string LimitedService { get; set; }

        public static AggregateRootDto Create(
            Guid id,
            string aggregateAttr,
            List<AggregateRootCompositeManyBDto> composites,
            AggregateRootCompositeSingleADto? composite,
            AggregateRootAggregateSingleCDto? aggregate,
            EnumWithoutValues enumType1,
            EnumWithDefaultLiteral enumType2,
            EnumWithoutDefaultLiteral enumType3,
            string limitedDomain,
            string limitedService)
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
                EnumType3 = enumType3,
                LimitedDomain = limitedDomain,
                LimitedService = limitedService
            };
        }
    }
}