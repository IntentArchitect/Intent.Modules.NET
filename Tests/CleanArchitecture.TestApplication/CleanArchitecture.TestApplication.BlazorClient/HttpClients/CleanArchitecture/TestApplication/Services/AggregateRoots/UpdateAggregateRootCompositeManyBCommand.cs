using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.CleanArchitecture.TestApplication.Services.AggregateRoots
{
    public class UpdateAggregateRootCompositeManyBCommand
    {
        public Guid AggregateRootId { get; set; }
        public Guid Id { get; set; }
        public string CompositeAttr { get; set; }
        public DateTime? SomeDate { get; set; }
        public UpdateAggregateRootCompositeManyBCompositeSingleBBDto Composite { get; set; }
        public List<UpdateAggregateRootCompositeManyBCompositeManyBBDto> Composites { get; set; }

        public static UpdateAggregateRootCompositeManyBCommand Create(
            Guid aggregateRootId,
            Guid id,
            string compositeAttr,
            DateTime? someDate,
            UpdateAggregateRootCompositeManyBCompositeSingleBBDto composite,
            List<UpdateAggregateRootCompositeManyBCompositeManyBBDto> composites)
        {
            return new UpdateAggregateRootCompositeManyBCommand
            {
                AggregateRootId = aggregateRootId,
                Id = id,
                CompositeAttr = compositeAttr,
                SomeDate = someDate,
                Composite = composite,
                Composites = composites
            };
        }
    }
}